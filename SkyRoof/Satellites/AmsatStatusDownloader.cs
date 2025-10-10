using System.Xml.Linq;
using AngleSharp;
using AngleSharp.Dom;
using Serilog;
using VE3NEA;


namespace SkyRoof
{
  public class AmsatStatusLoader
  {
    private readonly string url = "https://www.amsat.org/status/?app=SkyRoof";
    private readonly string[] activeColors = ["#4169E1", "#9900FF"];
    private readonly string[] inactiveColors = ["yellow", "red"]; 

    public readonly Dictionary<int, bool> Statuses = [];
    public Context ctx;

    public async Task<bool> GetStatusesAsync()
    {
      if (!ctx.Settings.Amsat.Enabled || ctx.GroupViewPanel == null) return true;

      string html;

      try
      {
        html = await DownloadStatusesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error downloading AMSAT status");
        return false;
      }

      try
      {
        ParseAmsatHtml(html);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error parsing AMSAT status");
        File.WriteAllText(Path.Combine(Utils.GetUserDataFolder(), "Downloads", "amsat_status_error.html"), html);
        return false;
      }

      ctx.GroupViewPanel?.ShowAmsatStatuses();
      return true;
    }

    private async void ParseAmsatHtml(string html)
    {
      var config = Configuration.Default.WithDefaultLoader();
      var context = BrowsingContext.New(config);
      var document = await context.OpenAsync(req => req.Content(html));
      var tables = document.QuerySelectorAll("table");
      var rows = tables[2].QuerySelectorAll("tr");

      Statuses.Clear();
      foreach (var row in rows.Skip(1)) ParseRow(row);
    }

    private void ParseRow(IElement row)
    {
      {
        var cells = row.QuerySelectorAll("td").ToArray();
        string satId = cells[0].TextContent;
        int? norad_id = new SatelliteNames().Amsat.FirstOrDefault(s => s.Value.Contains(satId)).Key;

        if (norad_id == null)
        {
          Log.Warning($"Parsing AMSAT status error: no norad_id found for {satId}");
          return;
        }

        string? color = cells.Skip(1).Take(24).Select(cell => cell.GetAttribute("bgcolor"))
          .Where(color => color != "C0C0C0").FirstOrDefault();

        bool status;
        if (activeColors.Contains(color)) status = true;
        else if (inactiveColors.Contains(color)) status = false;
        else return;

        if (Statuses.ContainsKey(norad_id.Value))
          // true if at least one transmitter is active
          Statuses[norad_id.Value] = Statuses[norad_id.Value] || status;
        else
          Statuses[norad_id.Value] = status;
      }
    }

    private async Task<string> DownloadStatusesAsync()
    { 
        using (HttpClient client = new HttpClient())
        {
          // download
          var response = await client.GetAsync(url);
          return await response.Content.ReadAsStringAsync();
        }
    }
  }
}
