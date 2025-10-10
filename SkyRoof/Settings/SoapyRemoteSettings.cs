using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyRoof
{
  public class SoapyRemoteSettings
  {
    [Description("Enable access to SDR on a remote computer.")]
    [DefaultValue(false)]
    public bool Enabled { get; set; } = false;

    [DisplayName("SoapyRemote Host")]
    [Description("The host name or IP address of the SoapySDRServer.")]
    [DefaultValue("localhost")]
    public string Host { get; set; } = "localhost";

    [DisplayName("SoapyRemote Port")]
    [Description("The port number of the SoapySDRServer.")]
    [DefaultValue((ushort)55132)]
    public ushort Port { get; set; } = 55132;

    public override string ToString() { return string.Empty; }
  }
}
