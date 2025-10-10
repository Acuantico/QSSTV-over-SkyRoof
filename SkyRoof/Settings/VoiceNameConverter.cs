using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;
using VE3NEA;

namespace SkyRoof
{
  internal class VoiceNameConverter : StringConverter
  {
    private string[]? Voices;

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
    {
      return true;
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
      ListVoices();
      return new StandardValuesCollection(Voices);
    }

    protected void ListVoices()
    {
      Voices ??= new Announcer().Voices.Select(Announcer.GetVoiceName).ToArray();
    }

    // to object
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      return value as string;
    }

    // to string
    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      ListVoices();

      if (Voices!.Contains(value as string))
        return (string)value!;
      else 
        return "<please select>";
    }
  }
}
