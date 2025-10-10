namespace SkyRoof
{
  public class SatnogsDbTle
  {
    public string tle0 { get; set; }
    public string tle1 { get; set; }
    public string tle2 { get; set; }
    public string tle_source { get; set; }
    public string sat_id { get; set; }
    public int? norad_cat_id { get; set; }
    public DateTime updated { get; set; }
  }


  public class SatnogsDbTleList : List<SatnogsDbTle> { }
}
