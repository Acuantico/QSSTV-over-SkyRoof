using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VE3NEA;

namespace SkyRoof
{
  public partial class GainWidget : UserControl
  {
    public Context ctx;

    public GainWidget()
    {
      InitializeComponent();

    }

    public void ApplyAfGain()
    {
      if (ctx.Settings.Audio.SoundcardVolume != AfGainSlider.Value)
        AfGainSlider.Value = ctx.Settings.Audio.SoundcardVolume;
      else
        AfGainSlider_ValueChanged(this, EventArgs.Empty);
    }

    private void AfGainSlider_ValueChanged(object sender, EventArgs e)
    {
      ctx.Settings.Audio.SoundcardVolume = AfGainSlider.Value;
      ctx.SpeakerSoundcard.Volume = Dsp.FromDb2(ctx.Settings.Audio.SoundcardVolume);
      AfGainLabel.Text = $"{AfGainSlider.Value} dB";
    }

    internal void ApplyRfGain()
    {
      if (ctx.Sdr != null)
      {
        RfGainSlider.Enabled = ctx.Sdr.CanChangeGain;

        if (RfGainSlider.Value != ctx.Sdr.NormalizedGain)
          RfGainSlider.Value = ctx.Sdr.NormalizedGain;
        else
          RfGainSlider_ValueChanged(this, EventArgs.Empty);
      }
    }

    private void RfGainSlider_ValueChanged(object sender, EventArgs e)
    {
      RfGainLabel.Text = RfGainSlider.Value.ToString();
      if (ctx.Sdr != null) ctx.Sdr.NormalizedGain = RfGainSlider.Value;
    }
  }
}
