using System.ComponentModel;
using VE3NEA;

namespace SkyRoof
{
  public class AudioSettings
  {
    // non-browsable
    public int SoundcardVolume = -25;
    public bool SpeakerEnabled = true;

    [DisplayName("Speaker Audio Device")]
    [Description("Soundcard for audio output")]
    [TypeConverter(typeof(OutputSoundcardNameConverter))]
    public string? SpeakerSoundcard { get; set; } = Soundcard<float>.GetDefaultSoundcardId();

    public override string ToString() { return string.Empty; }
  }
}
