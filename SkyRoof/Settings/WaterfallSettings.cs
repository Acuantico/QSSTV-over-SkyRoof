using System.ComponentModel;

namespace SkyRoof
{
  public class WaterfallSettings
  {
    public float Brightness = -0.2f;
    public float Contrast = 0.5f;
    public int Speed = 16;
    public int PaletteIndex = 0;
    public int SplitterDistance = 80;

    public override string ToString() { return string.Empty; }
  }
}
