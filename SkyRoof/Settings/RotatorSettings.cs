using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyRoof
{
  public class RotatorSettings : IControlEngineSettings
  {
    [DefaultValue(300)]
    [Description("Delay between the command cycles, ms")]
    public int Delay { get; set; } = 300;

    [DisplayName("Log Traffic")]
    [Description("Log command traffic for debugging")]
    [DefaultValue(false)]
    public bool LogTraffic { get; set; }
    
    [DefaultValue(false)]
    public bool Enabled { get; set; } = false;

    [Description("rotctld host")]
    [DefaultValue("127.0.0.1")]
    public string Host { get; set; } = "127.0.0.1";

    [DisplayName("TCP Port")]
    [Description("rotctld port")]
    [DefaultValue((ushort)4533)]
    public ushort Port { get; set; } = 4533;


    [DisplayName("Minimum Azimuth")]
    [DefaultValue(0)]
    public int MinAzimuth { get; set; } = 0;

    [DisplayName("Maximum Azimuth")]
    [DefaultValue(450)]
    public int MaxAzimuth { get; set; } = 450;

    [DisplayName("Azimuth Offset")]
    [DefaultValue(0)]
    public int AzimuthOffset { get; set; } = 0;

    [DisplayName("Minimum Elevation")]
    [DefaultValue(0)]
    public int MinElevation{ get; set; } = 0;

    [DisplayName("Maximum Elevation")]
    [DefaultValue(180)]
    public int MaxElevation { get; set; } = 180;

    [DisplayName("Elevation Offset")]
    [DefaultValue(0)]
    public int ElevationOffset { get; set; } = 0;

    [DisplayName("Step Size")]
    [Description("The tracking step size, in degrees")]
    [DefaultValue(5)]
    public int StepSize { get; set; } = 5;

    public override string ToString() { return string.Empty; }
  }
}
