using System.Diagnostics;

namespace SkyRoof
{
  public class RadioLink
  {
    public SatnogsDbSatellite? Sat;
    public SatnogsDbTransmitter? Tx;
    public SatelliteCustomization? SatCust;
    public TransmitterCustomization? TxCust;

    // non-persistent
    public bool IsTerrestrial = true;
    public bool RitEnabled;
    public double RitOffset;

    // persistent
    public Slicer.Mode DownlinkMode 
    { 
      get => TxCust!.DownlinkMode; 
      set => TxCust!.DownlinkMode = value; 
    }
    public Slicer.Mode UplinkMode 
    { 
      get => TxCust!.UplinkMode; 
      set => TxCust!.UplinkMode = value; 
    }
    public double TransponderOffset
    {
      get => TxCust!.TransponderOffset;
      set => TxCust!.TransponderOffset = value;
    }
    public double DownlinkManualCorrection
    {
      get => SatCust!.DownlinkManualCorrection;
      set => SatCust!.DownlinkManualCorrection = (int)value;
    }
    public double UplinkManualCorrection
    {
      get => SatCust!.UplinkManualCorrection;
      set => SatCust!.UplinkManualCorrection = (int)value;
    }

    public bool DownlinkDopplerCorrectionEnabled 
    { 
      get => SatCust!.DownlinkDopplerCorrectionEnabled; 
      set => SatCust!.DownlinkDopplerCorrectionEnabled = value; 
    }
    public bool UplinkDopplerCorrectionEnabled
    {
      get => SatCust!.UplinkDopplerCorrectionEnabled;
      set => SatCust!.UplinkDopplerCorrectionEnabled = value;
    }
    public bool DownlinkManualCorrectionEnabled
    {
      get => SatCust!.DownlinkManualCorrectionEnabled;
      set => SatCust!.DownlinkManualCorrectionEnabled = value;
    }
    public bool UplinkManualCorrectionEnabled
    {
      get => SatCust!.UplinkManualCorrectionEnabled;
      set => SatCust!.UplinkManualCorrectionEnabled = value;
    }

    // computed
    public double DownlinkFrequency, CorrectedDownlinkFrequency;
    public double UplinkFrequency, CorrectedUplinkFrequency;
    public double DopplerFactor = 0;
    public bool IsAboveHorizon;
    public bool HasUplink => !IsTerrestrial && UplinkFrequency > 0 && SatnogsDbTransmitter.IsHamFrequency(UplinkFrequency);
    public bool IsTransponder => Tx != null &&
      Tx.downlink_high.HasValue && Tx.downlink_high != Tx.downlink_low &&
      Tx.uplink_low.HasValue && Tx.uplink_high.HasValue;
    public bool IsCrossBand => HasUplink && 
      ((SatnogsDbTransmitter.IsUhfFrequency(UplinkFrequency) != SatnogsDbTransmitter.IsUhfFrequency(DownlinkFrequency)) 
      ||
      (SatnogsDbTransmitter.IsVhfFrequency(UplinkFrequency) != SatnogsDbTransmitter.IsVhfFrequency(DownlinkFrequency)));


    public void ObserveSatellite(SatellitePasses engine)
    {
      var observation = engine.ObserveSatellite(Sat, DateTime.UtcNow);

      if (observation == null)
      {
        DopplerFactor = 0;
        IsAboveHorizon = false;
      }
      else
      {
        DopplerFactor = observation.RangeRate / 3e5;
        IsAboveHorizon = observation.Elevation > 0;
      }
    }

    internal void ComputeFrequencies()
    {
      if (IsTerrestrial)
      {
        CorrectedDownlinkFrequency = DownlinkFrequency;
        if (RitEnabled) CorrectedDownlinkFrequency += RitOffset;
        CorrectedUplinkFrequency = UplinkFrequency = 0;
        DopplerFactor = 0;

      }

      else
      {
        // downlink nominal
        DownlinkFrequency = Tx!.DownlinkLow;
        if (IsTransponder) DownlinkFrequency += TransponderOffset;

        // downlink corrected
        CorrectedDownlinkFrequency = DownlinkFrequency;
        if (RitEnabled) CorrectedDownlinkFrequency += RitOffset;
        if (DownlinkDopplerCorrectionEnabled) CorrectedDownlinkFrequency *= 1 - DopplerFactor;
        if (DownlinkManualCorrectionEnabled) CorrectedDownlinkFrequency += DownlinkManualCorrection;

        // uplink nominal
        if (IsTransponder)
          if (Tx.invert) UplinkFrequency = (double)Tx.uplink_high! - TransponderOffset;
          else UplinkFrequency = (double)Tx.uplink_low! + TransponderOffset;
        else if (Tx.uplink_low.HasValue) UplinkFrequency = (double)Tx.uplink_low;
        else UplinkFrequency = 0;

        // uplink corrected
        CorrectedUplinkFrequency = UplinkFrequency;
        if (UplinkDopplerCorrectionEnabled) CorrectedUplinkFrequency *= 1 + DopplerFactor;
        if (UplinkManualCorrectionEnabled) CorrectedUplinkFrequency += UplinkManualCorrection;
      }
    }

    // dragging changes either the absolute frequency (terrestrial),
    // or the transponder offset (transponder),
    // or the manual correction (transmitter)
    internal double GetDraggableFrequency()
    {
      if (IsTerrestrial) return DownlinkFrequency;
      else if (IsTransponder) return TransponderOffset;
      else return DownlinkManualCorrection;
    }

    internal void SetDraggableFrequency(double freq)
    {
      if (IsTerrestrial)
        DownlinkFrequency = freq;

      else if (!IsAboveHorizon)
      {
        Console.Beep();
        return;
      }

      else if (IsTransponder)
      {
        freq = Math.Max(0, Math.Min(freq, (double)(Tx!.uplink_high! - Tx!.uplink_low!)));
        TransponderOffset = freq;
      }

      else
      {
        freq = Math.Max(-25000, Math.Min(25000, freq));
        DownlinkManualCorrection = freq;
      }

      ComputeFrequencies();
    }

    internal void IncrementDownlinkFrequency(int delta)
    {
      // RIT
      if (RitEnabled)
      {
        long newOffset = (long)RitOffset + delta;
        RitOffset = Math.Max(-25000, Math.Min(25000, newOffset));
      }

      // terrestrial
      else if (IsTerrestrial) 
        DownlinkFrequency += delta;

      //else if (!IsAboveHorizon)
      //{
      //  Console.Beep();
      //  return;
      //}

      // transponder
      else if (IsTransponder)
      {
        long newOffset = (long)TransponderOffset + delta;
        long maxOffset = (long)Tx!.uplink_high! - (long)Tx!.uplink_low!;
        TransponderOffset = Math.Max(0, Math.Min(maxOffset, newOffset));
      }

      // transmitter
      else
      {
        double newOffset = DownlinkManualCorrection + delta;
        DownlinkManualCorrection = Math.Max(-25000, Math.Min(25000, newOffset));
      }

      ComputeFrequencies();
    }

    public void IncrementUplinkFrequency(int delta)
    {
      if (IsTerrestrial)
        UplinkFrequency += delta;
      else
      {
        double newOffset = UplinkManualCorrection + delta;
        if (newOffset >= -25000 && newOffset <= 25000)
          UplinkManualCorrection = newOffset;
      }

      ComputeFrequencies();
    }
  }
}
