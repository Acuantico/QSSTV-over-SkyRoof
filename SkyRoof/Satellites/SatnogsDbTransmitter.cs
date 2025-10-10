namespace SkyRoof
{
  public class SatnogsDbTransmitter
  {
    public string uuid { get; set; }
    public string description { get; set; }
    public bool alive { get; set; }
    public string type { get; set; }
    public long? uplink_low { get; set; }
    public long? uplink_high { get; set; }
    public long? uplink_drift { get; set; }
    public long? downlink_low { get; set; }
    public long? downlink_high { get; set; }
    public long? downlink_drift { get; set; }
    public string mode { get; set; }
    public int? mode_id { get; set; }
    public string uplink_mode { get; set; }
    public bool invert { get; set; }
    public float? baud { get; set; }
    public string sat_id { get; set; }
    public int? norad_cat_id { get; set; }
    public int? norad_follow_id { get; set; }
    public string status { get; set; }
    public DateTime updated { get; set; }
    public string citation { get; set; }
    public string service { get; set; }
    public string iaru_coordination { get; set; }
    public string iaru_coordination_url { get; set; }
    public ItuNotification itu_notification { get; set; }
    public bool frequency_violation { get; set; }
    public bool unconfirmed { get; set; }


    internal SatnogsDbSatellite Satellite;
    internal long DownlinkLow => (long)downlink_low!;


    public string GetTooltipText()
    {
      string mode = this.mode;
      if (baud != null && baud != 0) mode = $"{mode} {baud} Bd";
      string uplink = FormatFrequencyRange(uplink_low, uplink_high);
      string downlink = FormatFrequencyRange(downlink_low, downlink_high, invert);

      string result = $"{description}\nType: {type}\n";
      if (uplink_low != null) result += $"Uplink: {uplink}\n";
      result += $"Downlink: {downlink}\nMode: {mode}\n";
      if (uplink_low != null) result += $"Inverted: {invert}\n";
      result += $"Service: {service}\nUpdated: {updated:yyyy-MM-dd}";

      return result;
    }

    public static string FormatFrequencyRange(long? low, long? high, bool inverted = false)
    {
      if (low == null) return "";
      if (high == null) return $"{low / 1000d:N1}";
      if (inverted) return $"{high / 1000d:N1} - {low / 1000d:N1}";
      return $"{low / 1000d:N1} - {high / 1000d:N1}";
    }

    public bool IsVhf(long? freq = null)
    {
      freq ??= DownlinkLow;
      return IsVhfFrequency((double)freq);
    }

    public bool IsUhf(long? freq = null)
    {
      freq ??= DownlinkLow;
      return IsUhfFrequency((double)freq);
    }

    public static bool IsVhfFrequency(double freq)
    {
      return freq >= 144000000 && freq <= 148000000;
    }

    public static bool IsUhfFrequency(double freq)
    {
      return freq >= 430000000 && freq <= 440000000;
    }

    public static bool IsHamFrequency(double freq)
    {
      return IsVhfFrequency(freq) || IsUhfFrequency(freq);
    }

    internal bool HasUplink()
    {
      return uplink_low != null;
    }
  }


  public class ItuNotification { public List<string> urls { get; set; } = new(); }

  public class SatnogsDbTransmitterList : List<SatnogsDbTransmitter> { }
}
