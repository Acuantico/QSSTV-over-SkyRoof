namespace SkyRoof
{
  public class RigCtldCommands
  {
    // Setup commands
    public string[]? setup_simplex;
    public string[]? setup_split;
    public string[]? setup_duplex;

    // Read commands
    public string? read_rx_frequency;
    public string? read_tx_frequency;
    public string? read_ptt;

    // Write commands
    public string? write_rx_frequency;
    public string? write_tx_frequency;
    public string? write_rx_mode;
    public string? write_tx_mode;
    public string? set_ptt_on;
    public string? set_ptt_off;

    // SkyCAT commands
    public static readonly RigCtldCommands SkyCat = new RigCtldCommands
    {
      setup_simplex = ["U Simplex"],
      setup_split = ["U Split"],
      setup_duplex = ["U Duplex"],

      read_rx_frequency = "f",
      read_tx_frequency = "i",
      read_ptt = "t",
      write_rx_frequency = "F {frequency}",
      write_tx_frequency = "I {frequency}",
      write_rx_mode = "M {mode} 0",
      write_tx_mode = "X {mode} 0",
      set_ptt_on = "T 1",
      set_ptt_off = "T 0",
    };

    // Standard RigCtld commands in the simplex mode. read/write rx and tx frequencies both map to the f/F commands
    public static readonly RigCtldCommands RigCtld = new RigCtldCommands
    {
      //setup_duplex = [ "U SATMODE 1" ],
      //setup_split = [ "U SATMODE 0", "U DUAL_WATCH 0", "V VFOA", "S 1 VFOB", "V VFOA" ],
      //setup_simplex = [ "U SATMODE 0", "U DUAL_WATCH 0", "V VFOA", "S 0 VFOB", "V VFOA" ],

      setup_simplex = ["U DUAL_WATCH 0", "V VFOA", "S 0 VFOB", "V VFOA"],
      

      read_rx_frequency = "f",
      read_tx_frequency = "f",
      read_ptt = "t",
      write_rx_frequency = "F {frequency}",
      write_tx_frequency = "F {frequency}",
      write_rx_mode = "M {mode} 0",
      write_tx_mode = "M {mode} 0",
      set_ptt_on = "T 1",
      set_ptt_off = "T 0",
    };
  }
}
