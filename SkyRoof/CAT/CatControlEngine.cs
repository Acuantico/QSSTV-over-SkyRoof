using System.Globalization;
using Serilog;
using SkyRoof.Properties;
using VE3NEA;

namespace SkyRoof
{
  public enum OperatingMode { RxOnly, TxOnly, Simplex, Split, Duplex }

  public class CatControlEngine : ControlEngine
  {
    const long NOT_ASSIGNED = 0;

    private bool rx, tx, crossband;
    private OperatingMode CatMode;
    private RadioCapabilities RadioCapabilities;
    private AvailableCommands Caps;
    private readonly int TuningStep;
    private readonly bool IgnoreDialKnob;

    public bool Ptt { get; private set; } = false;
    private bool PttChanged = false;
    private bool DialKnobSpinning = false;
    public long RequestedRxFrequency, LastWrittenRxFrequency, LastReadRxFrequency;
    public long RequestedTxFrequency, LastWrittenTxFrequency, LastReadTxFrequency;
    private Slicer.Mode? RequestedRxMode, LastWrittenRxMode;      
    private Slicer.Mode? RequestedTxMode, LastWrittenTxMode;
    private bool? RequestedPtt;

    public event EventHandler? RxTuned;
    public event EventHandler? TxTuned;

    public CatControlEngine(CatRadioSettings radioSettings, CatSettings catSettings) : base(radioSettings.Host, radioSettings.Port, catSettings)
    {
      TuningStep = catSettings.TuningStep;
      IgnoreDialKnob = catSettings.IgnoreDialKnob;
    }

    private void LogInfo(string msg)
    {
      if (log) Log.Information(msg);
    }

    private void LogFreqs(string msg)
    {
      LogInfo($"{msg}  (RxReq={RequestedRxFrequency}  RxWr={LastWrittenRxFrequency}  RxRd={LastReadRxFrequency})");
    }

    // some radios use 10 Hz steps and some don't, round frequency to the tuning step
    private long RoundToStep(double freq)
    {
      int step = TuningStep;
      return step * (long)Math.Truncate(freq / step);
    }




    //----------------------------------------------------------------------------------------------
    //                                      public methods
    //----------------------------------------------------------------------------------------------
    public void Start(bool rx, bool tx, bool crossband)
    {
      this.rx = rx;
      this.tx = tx;
      this.crossband = crossband;

      StartThread();
    }

    public void SetRxFrequency(double frequency)
    {
      frequency = RoundToStep(frequency);
      LogFreqs($"SetRxFrequency {frequency}");

      if (DialKnobSpinning)
        LogInfo("Ignoring RX frequency change while dial knob is spinning");
      else
        RequestedRxFrequency = (long)frequency;
    }

    public void SetTxFrequency(double frequency)
    {
      frequency = RoundToStep(frequency);
      LogFreqs($"SetTxFrequency {frequency}");

      if (DialKnobSpinning)
        LogInfo("Ignoring TX frequency change while dial knob is spinning");
      else
        RequestedTxFrequency = (long)frequency;
    }

    public void SetRxMode(Slicer.Mode mode)
    {
      RequestedRxMode = mode;
    }

    public void SetTxMode(Slicer.Mode mode)
    {
      RequestedTxMode = mode;
    }

    public void SetPtt(bool ptt)
    {
      LogInfo($"SetPtt {ptt}");
      RequestedPtt = ptt;
    }




    //----------------------------------------------------------------------------------------------
    //                                        thread 
    //----------------------------------------------------------------------------------------------
    protected override void Cycle()
    {
      ReadPtt();
      if (NeedToWriteTxFreqModeBeforePtt()) TryWriteTxFreqModeBeforePtt();
      TryWritePtt();

      if (NeedToReadRxFrequency()) TryReadRxFrequency();
      if (NeedToReadTxFrequency()) TryReadTxFrequency();

      if (NeedToWriteRxFrequency()) TryWriteRxFrequency(RequestedRxFrequency);
      if (NeedToWriteTxFrequency()) TryWriteTxFrequency();

      if (NeedToWriteRxMode()) TryWriteRxMode();
      if (NeedToWriteTxMode()) TryWriteTxMode();
    }

    protected override bool Setup()
    {
      try
      {
        SelectOperatingMode();

        switch (CatMode)
        {
          case OperatingMode.RxOnly:
          case OperatingMode.TxOnly:
          case OperatingMode.Simplex: return SendWriteCommands(commands.setup_simplex); 
          case OperatingMode.Split:   return SendWriteCommands(commands.setup_split); 
          case OperatingMode.Duplex:  return SendWriteCommands(commands.setup_duplex); 
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Failed to set up radio.");
      }

      return false;
      }

    private RigCtldCommands commands = RigCtldCommands.RigCtld;

    private void SelectOperatingMode()
    {
        // get radio capabilities, either from SkyCAT for from a file
        RadioCapabilities = ReadCapabilitiesFromSkyCat() ?? RadioCapabilities.LoadDefaultCapabilities();
        Log.Information($"Loaded radio capabilities for: {RadioCapabilities.model}");

        // Set appropriate command set
        commands = RadioCapabilities.model == "rigctld.exe" ?
            RigCtldCommands.RigCtld : RigCtldCommands.SkyCat;

        // determmine CatMode, get radio Caps in that mode
        if (rx && !tx)
        {
            CatMode = OperatingMode.RxOnly;
            Caps = RadioCapabilities.simplex!;
        }
        else if (!rx && tx)
        {
            CatMode = OperatingMode.TxOnly;
            Caps = RadioCapabilities.simplex!;
        }
        else if (crossband && RadioCapabilities.duplex != null)
        {
            CatMode = OperatingMode.Duplex;
            Caps = RadioCapabilities.duplex!;
        }
        else if (RadioCapabilities.CanSplitTune(crossband))
        {
            CatMode = OperatingMode.Split;
            Caps = RadioCapabilities.split!;
        }
        else if (RadioCapabilities.simplex != null)
        {
            CatMode = OperatingMode.Simplex;
            Caps = RadioCapabilities.simplex!;
        }
        else
            throw new Exception("Radio does not support any operating modes");
    }

    private RadioCapabilities? ReadCapabilitiesFromSkyCat()
    {
      string json = SendReadCommand("a") ?? string.Empty;

      if (json == "RPRT -18")
        return null;

      if (json == string.Empty) 
        throw new Exception("Failed to read radio capabilities from SkyCAT");

      return RadioCapabilities.LoadFromJson(json) ??
        throw new Exception("Failed to parse radio capabilities from SkyCAT");
    }




    //----------------------------------------------------------------------------------------------
    //                                    read frequencies
    //----------------------------------------------------------------------------------------------
    private void TryReadRxFrequency()
    {
      string command = GetReadRxFrequencyCommand();
      if (command == string.Empty) return;

      long frequency = ReadFrequency(command);

      DialKnobSpinning = LastReadRxFrequency != 0 && // first read - ignore, no previous value
        IsDiff(frequency, LastReadRxFrequency) &&    // same freq as before, no change
        IsDiff(frequency, LastWrittenRxFrequency) && // new freq was written, ingnore
          IsDiff(frequency, LastWrittenTxFrequency); // tx freq read by accident, ignore

      LastReadRxFrequency = frequency;
      
      if (DialKnobSpinning)
      {
        // cancel previously requested frequency changes, they would undo the dial knob change
        if (RequestedRxFrequency != NOT_ASSIGNED && IsDiff(RequestedRxFrequency, frequency))
          LogInfo($"Canceling pending RX frequency change ({RequestedRxFrequency}) due to manual change ({frequency})");
        RequestedRxFrequency = NOT_ASSIGNED;
        RequestedTxFrequency = NOT_ASSIGNED;

        OnRxFrequencyChanged();
      }
    }

    private void TryReadTxFrequency()
    {
      string command = GetReadTxFrequencyCommand();
      if (command == string.Empty) return;

      long frequency = ReadFrequency(command);

      bool changed = LastReadTxFrequency != 0 &&
        IsDiff(frequency, LastReadTxFrequency) &&
        IsDiff(frequency, LastWrittenTxFrequency) &&
        IsDiff(frequency, LastWrittenRxFrequency);

      LastReadTxFrequency = frequency;
      if (changed) OnTxFrequencyChanged();
    }

    private long ReadFrequency(string command)
    {
      var reply = SendReadCommand(command);
      if (reply == null) return 0;
      if (!long.TryParse(reply, CultureInfo.InvariantCulture, out long frequency)) BadReply(reply);
      return RoundToStep(frequency);
    }




    //----------------------------------------------------------------------------------------------
    //                                 write frequencies
    //----------------------------------------------------------------------------------------------
    private void TryWriteRxFrequency(long frequency)
    {
      if (frequency == NOT_ASSIGNED) return;

      string command = GetWriteRxFrequencyCommand(frequency);
      if (command == string.Empty) return;

      SendWriteCommand(command);
      LastWrittenRxFrequency = frequency;
      LogFreqs("Rx frequency written");
    }

    private void TryWriteTxFrequency()
    {
      long frequency = RequestedTxFrequency;
      string command = GetWriteTxFrequencyCommand(frequency);
      if (command == string.Empty) return;

      SendWriteCommand(command);
      LastWrittenTxFrequency = frequency;
      LogFreqs("Tx frequency written");
    }

    // if no commands to set split frequency/mode
    // and cannot set main frequency/mode when transmitting,
    // set it immediately before switching to transmit mode
    private void TryWriteTxFreqModeBeforePtt()
    {
      LogInfo("Writing TX frequency and mode before PTT ON");

      // simplex mode: setting RX frequency, then switching to TX, and it becomes TX frequency
      TryWriteRxFrequency(RequestedTxFrequency);
      TryWriteRxMode(RequestedTxMode);
    }




    //----------------------------------------------------------------------------------------------
    //                                       mode
    //----------------------------------------------------------------------------------------------
    private void TryWriteRxMode(Slicer.Mode? mode = null)
    {
      Slicer.Mode newMode = (mode ?? RequestedRxMode!).Value;

      string command = GetWriteRxModeCommand(newMode);
      if (command == string.Empty) return;

      bool ok = SendWriteCommand(command);

      if (!ok)
      {
        RemoveDataFromMode(ref newMode);
        command = GetWriteRxModeCommand(newMode);
        SendWriteCommand(command);
      }

      LastWrittenRxMode = RequestedRxMode;
    }

    private void TryWriteTxMode()
    {
      Slicer.Mode mode = RequestedTxMode!.Value;

      string command = GetWriteTxModeCommand(mode);
      if (command == string.Empty) return;

      bool ok = SendWriteCommand(command);

      if (!ok)
      {
        RemoveDataFromMode(ref mode);
        command = GetWriteTxModeCommand(mode);
        SendWriteCommand(command);
      }

      LastWrittenTxMode = RequestedTxMode!;
    }


    private void RemoveDataFromMode(ref Slicer.Mode mode)
    {
      mode = mode switch
      {
        Slicer.Mode.USB_D => Slicer.Mode.USB,
        Slicer.Mode.LSB_D => Slicer.Mode.LSB,
        Slicer.Mode.FM_D => Slicer.Mode.FM,
        _ => mode
      };
    }




    //----------------------------------------------------------------------------------------------
    //                                         ptt
    //----------------------------------------------------------------------------------------------
    internal bool CanPtt()
    {
      return CatMode != OperatingMode.RxOnly &&
             commands.set_ptt_on != null &&
             commands.set_ptt_off != null;
    }

    private void ReadPtt()
    {
      PttChanged = false;
      if (commands.read_ptt == null) return;

      var reply = SendReadCommand(commands.read_ptt);
      bool newPtt = reply == "1";

      PttChanged = newPtt != Ptt;
      Ptt = newPtt;

      if (PttChanged)
        if (Ptt == true) LastWrittenTxFrequency = NOT_ASSIGNED; else LastWrittenRxFrequency = NOT_ASSIGNED;

      LogInfo($"ReadPtt: {Ptt} (changed={PttChanged})");
    }

    private void TryWritePtt()
    {
      if (CatMode == OperatingMode.RxOnly) return;
      if (!RequestedPtt.HasValue) return;
      if (RequestedPtt == Ptt) return;

      // Replace these two lines
      if (RequestedPtt == false && commands.set_ptt_off != null) SendWriteCommand(commands.set_ptt_off);
      else if (RequestedPtt == true && commands.set_ptt_on != null) SendWriteCommand(commands.set_ptt_on);

      Ptt = RequestedPtt.Value;
      PttChanged = true;
      if (Ptt == true) LastWrittenTxFrequency = NOT_ASSIGNED; else LastWrittenRxFrequency = NOT_ASSIGNED;
      RequestedPtt = null;
    }




    //----------------------------------------------------------------------------------------------
    //                                    get command
    //----------------------------------------------------------------------------------------------
    private string GetReadRxFrequencyCommand()
    {
      switch (CatMode)
      {
        case OperatingMode.TxOnly:
          return string.Empty;

        case OperatingMode.RxOnly:
          if (Ptt == false && Caps.Can(CatAction.read_rx_frequency, Ptt)) return commands.read_rx_frequency!;
          if (Ptt == true && Caps.Can(CatAction.read_tx_frequency, Ptt)) return commands.read_tx_frequency!;
          break;

        case OperatingMode.Simplex:
          if (Ptt == false && Caps.Can(CatAction.read_rx_frequency, Ptt)) return commands.read_rx_frequency!;
          break;

        // split or duplex
        default:
          if (Caps.Can(CatAction.read_rx_frequency, Ptt)) return commands.read_rx_frequency!;
          break;
      }

      return string.Empty;
    }

    private string GetReadTxFrequencyCommand()
    {
      switch (CatMode)
      {
        case OperatingMode.RxOnly:
          return string.Empty;

        case OperatingMode.TxOnly:
          if (Ptt == false && Caps.Can(CatAction.read_rx_frequency, Ptt)) return commands.read_rx_frequency!;
          if (Ptt == true && Caps.Can(CatAction.read_tx_frequency, Ptt)) return commands.read_tx_frequency!;
          break;

        case OperatingMode.Simplex:
          if (Ptt == true && Caps.Can(CatAction.read_tx_frequency, Ptt)) return commands.read_tx_frequency!;
          break;

        // split or duplex
        default:
          if (Caps.Can(CatAction.read_tx_frequency, Ptt)) return commands.read_tx_frequency!;
          break;
      }

      return string.Empty;
    }

    private string GetWriteRxFrequencyCommand(long frequency)
    {
      switch (CatMode)
      {
        case OperatingMode.TxOnly:
          return string.Empty;

        case OperatingMode.RxOnly:
          if (Ptt == false && Caps.Can(CatAction.write_rx_frequency, Ptt)) return commands.write_rx_frequency.Replace("{frequency}", $"{frequency}");
          else if (Ptt == true && Caps.Can(CatAction.write_tx_frequency, Ptt)) return commands.write_tx_frequency.Replace("{frequency}", $"{frequency}");
          break;

        case OperatingMode.Simplex:
          if (Ptt == false && Caps.Can(CatAction.write_rx_frequency, Ptt)) return commands.write_rx_frequency.Replace("{frequency}", $"{frequency}");
          break;

        default:
          if (Caps.Can(CatAction.write_rx_frequency, Ptt)) return commands.write_rx_frequency.Replace("{frequency}", $"{frequency}");
          break;
      }

      return string.Empty;
    }

    private string GetWriteTxFrequencyCommand(long frequency)
    {
      switch (CatMode)
      {
        case OperatingMode.RxOnly:
          return string.Empty;

        case OperatingMode.TxOnly:
          if (Ptt == false && Caps.Can(CatAction.write_rx_frequency, Ptt)) return commands.write_rx_frequency!.Replace("{frequency}", $"{frequency}");
          if (Ptt == true && Caps.Can(CatAction.write_tx_frequency, Ptt)) return commands.write_tx_frequency!.Replace("{frequency}", $"{frequency}");
          break;

        case OperatingMode.Simplex:
          if (Ptt == true && Caps.Can(CatAction.write_tx_frequency, Ptt)) return commands.write_tx_frequency!.Replace("{frequency}", $"{frequency}");
          break;

        default:
          if (Caps.Can(CatAction.write_tx_frequency, Ptt)) return commands.write_tx_frequency!.Replace("{frequency}", $"{frequency}");
          break;
      }

      return string.Empty;
    }

    private string GetWriteRxModeCommand(Slicer.Mode mode)
    {
      switch (CatMode)
      {
        case OperatingMode.TxOnly:
          return string.Empty;

        case OperatingMode.RxOnly:
          if (Ptt == false && Caps.CanSetup(CatAction.write_rx_mode)) return commands.write_rx_mode!.Replace("{mode}", $"{mode}");
          if (Ptt == true && Caps.Can(CatAction.write_tx_mode, Ptt)) return commands.write_tx_mode!.Replace("{mode}", $"{mode}");
          break;

        case OperatingMode.Simplex:
          if (Ptt == false && Caps.CanSetup(CatAction.write_rx_mode)) return commands.write_rx_mode!.Replace("{mode}", $"{mode}");
          break;

        default:
          if (Ptt == false && Caps.CanSetup(CatAction.write_rx_mode)) return commands.write_rx_mode!.Replace("{mode}", $"{mode}");
          if (Ptt == true && Caps.Can(CatAction.write_rx_mode, Ptt)) return commands.write_rx_mode!.Replace("{mode}", $"{mode}");
          break;
      }

      return string.Empty;
    }

    private string GetWriteTxModeCommand(Slicer.Mode mode)
    {
      switch (CatMode)
      {
        case OperatingMode.RxOnly:
          return string.Empty;

        case OperatingMode.TxOnly:
          if (Ptt == false && Caps.CanSetup(CatAction.write_rx_mode)) return commands.write_rx_mode!.Replace("{mode}", $"{mode}");
          if (Ptt == true && Caps.Can(CatAction.write_tx_mode, Ptt)) return commands.write_tx_mode!.Replace("{mode}", $"{mode}");
          break;

        case OperatingMode.Simplex:
          if (Ptt == false && Caps.CanSetup(CatAction.write_tx_mode)) return commands.write_tx_mode!.Replace("{mode}", $"{mode}");
          if (Ptt == true && Caps.Can(CatAction.write_tx_mode, Ptt)) return commands.write_tx_mode!.Replace("{mode}", $"{mode}");
          break;

        default:
          if (Ptt == false && Caps.CanSetup(CatAction.write_tx_mode)) return commands.write_tx_mode!.Replace("{mode}", $"{mode}");
          if (Ptt == true && Caps.Can(CatAction.write_tx_mode, Ptt)) return commands.write_tx_mode!.Replace("{mode}", $"{mode}");
          break;
      }

      return string.Empty;
    }




    //----------------------------------------------------------------------------------------------
    //                                  check conditions
    //----------------------------------------------------------------------------------------------
    private bool NeedToWriteTxFreqModeBeforePtt()
    {
      // not simplex
      if (CatMode != OperatingMode.Simplex) return false;

      // not swsitching to tx
      if (RequestedPtt != true) return false;

      // nothing to write
      if (RequestedTxFrequency == NOT_ASSIGNED && !RequestedTxMode.HasValue) return false;

      return true;
    }

    private bool NeedToReadRxFrequency()
    {
      if (CatMode == OperatingMode.TxOnly) return false;
      return !IgnoreDialKnob && (Ptt == false || CatMode == OperatingMode.RxOnly);
    }

    private bool NeedToReadTxFrequency()
    {
      if (CatMode == OperatingMode.RxOnly) return false;
      return !IgnoreDialKnob && (Ptt == true || CatMode == OperatingMode.TxOnly);
    }

    private bool NeedToWriteRxFrequency()
    {
      if (CatMode == OperatingMode.TxOnly) return false;
      if (RequestedRxFrequency == NOT_ASSIGNED) return false; // never assigned
      if (CatMode == OperatingMode.Simplex && PttChanged) return true;
      return IsDiff(RequestedRxFrequency, LastWrittenRxFrequency);
    }

    private bool NeedToWriteTxFrequency()
    {
      if (CatMode == OperatingMode.RxOnly) return false;
      if (RequestedTxFrequency == NOT_ASSIGNED) return false;
      if (CatMode == OperatingMode.Simplex && PttChanged) return true;
      return IsDiff(RequestedTxFrequency, LastWrittenTxFrequency);
    }

    private bool NeedToWriteRxMode()
    {
      if (CatMode == OperatingMode.TxOnly) return false;
      if (!RequestedRxMode.HasValue) return false;
      if (CatMode == OperatingMode.Simplex && PttChanged) return true;
      return RequestedRxMode != LastWrittenRxMode;
    }

    private bool NeedToWriteTxMode()
    {
      if (CatMode == OperatingMode.RxOnly) return false;
      if (!RequestedTxMode.HasValue) return false;
      if (CatMode == OperatingMode.Simplex && PttChanged) return true;
      return RequestedTxMode != LastWrittenTxMode;
    }

    // may be changed to allow some slack
    private bool IsDiff(long freq1, long freq2)
    {
      return Math.Abs(freq1 - freq2) > 0;
    }




    //----------------------------------------------------------------------------------------------
    //                                      events
    //----------------------------------------------------------------------------------------------
    protected void OnRxFrequencyChanged()
    {
      LogFreqs($"OnRxFrequencyChanged");
      syncContext.Post(s => RxTuned?.Invoke(this, EventArgs.Empty), null);
    }

    protected void OnTxFrequencyChanged()
    {
      LogFreqs($"OnTxFrequencyChanged");
      syncContext.Post(s => TxTuned?.Invoke(this, EventArgs.Empty), null);
    }
  }
}
