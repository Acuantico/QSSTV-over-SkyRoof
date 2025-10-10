using Newtonsoft.Json;
using SkyRoof.Properties;
using VE3NEA;

namespace SkyRoof
{
  public class SatelliteNames
  {
    public readonly Dictionary<int, List<string>> Amsat;
    public readonly Dictionary<int, string> Lotw;

    public SatelliteNames()
    {
      string dir = Utils.GetUserDataFolder();
      Directory.CreateDirectory(dir);

      string path = Path.Combine(dir, "amsat_sat_names.json");
      if (!File.Exists(path)) File.WriteAllBytes(path, Resources.amsat_sat_names);
      Amsat = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(File.ReadAllText(path))!;

      path = Path.Combine(dir, "lotw_sat_names.json");
      if (!File.Exists(path)) File.WriteAllBytes(path, Resources.lotw_sat_names);
      Lotw = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText(path))!;
    }



    // this code was used to generate the name lists
    public static void ProcessLotw(Context ctx)
    {
      //foreach (var rec in lotw)
      //{
      //  string name = rec[0] = rec[0].Trim();
      //  string textToSearch = SatnogsDbSatellite.MakeSearchText(rec[0]);
      //
      //  var found = ctx.SatnogsDb.Satellites.Where(s => s.name == rec[1]).ToList();
      //  if (found.Count != 1)
      //    found = ctx.SatnogsDb.Satellites.Where(s => s.name == rec[0]).ToList();
      //  if (found.Count != 1)
      //    found = ctx.SatnogsDb.Satellites.Where(s => s.SearchText.Contains(textToSearch)).ToList();
      //
      //  if (found.Count == 1)
      //    rec[0] = found[0].norad_cat_id.ToString();
      //  else
      //    rec[0] = rec[1];
      //  rec[1] = name;
      //}
      //string result = JsonConvert.SerializeObject(lotw).Replace("],", "],\n");
    }
  }
}