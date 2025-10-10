using VE3NEA;

namespace SkyRoof
{
  public class SdrSettings
  {
    public List<SoapySdrDeviceInfo> Devices = new();
    public string? SelectedDeviceName;
    public bool Enabled = true;
  }
}