using System.Text.RegularExpressions;
using Serilog;

namespace SkyRoof
{
  internal class VersionChecker
  {
    private LatestVersionInfo LatestVersion;
    public event EventHandler? UpdateAvailable;

    public VersionChecker(LatestVersionInfo latestVersion)
    {
      LatestVersion = latestVersion;
    }


    public async Task CheckVersionAsync()
    {
      var currentVersion = typeof(VersionChecker).Assembly.GetName().Version!;
      await GetAvailableVersion();

      if (!LatestVersion.Known)
      {
        Log.Warning("VersionChecker could not determine the latest version.");
        return;
      }

      if (LatestVersion.Major > currentVersion.Major ||
          (LatestVersion.Major == currentVersion.Major && LatestVersion.Minor > currentVersion.Minor))
      {
        Log.Information($"VersionChecker: update available: {LatestVersion.Name})");
         UpdateAvailable?.Invoke(this, EventArgs.Empty);
      }
      else
        Log.Information("VersionChecker: software is up to date: {Name}", LatestVersion.Name);
    }

    private async Task GetAvailableVersion()
    {
      string url = "https://ve3nea.github.io/SkyRoof/download.html?update_check=true";

      try
      {
        string html = await new HttpClient().GetStringAsync(url);

        if (!ParseHtml(html))
          Log.Warning("VersionChecker could not parse downloaded html.");
      }
      catch (Exception ex)
      {
        Log.Warning(ex, "VersionChecker could not download page.");
        return;
      }
    }

    private bool ParseHtml(string html)
    {
      // extract download URL and version name
      string regex = @"Current Version<\/h3>[^a]+a href=""([^""]+)"">([^<]+)<\/a>";
      var match = Regex.Match(html, regex, RegexOptions.Singleline);
      if (!match.Success) return false;
      string url = match.Groups[1].Value;
      string name = match.Groups[2].Value.Trim();
      if (string.IsNullOrEmpty(url) | string.IsNullOrEmpty(name))
        return false;

      // extract major and minor version numbers
      string[] versionParts = name.Split(' ');
      if (versionParts.Length < 2) return false;
      versionParts = versionParts[1].Split('.');
      if (versionParts.Length < 3) return false;
      if (versionParts[0].ToLower() != "v") return false;
      if (!int.TryParse(versionParts[1], out int major)) return false;
      if (!int.TryParse(versionParts[2], out int minor)) return false;

      LatestVersion.Name = name;
      LatestVersion.Url = url;
      LatestVersion.Major = major;
      LatestVersion.Minor = minor;
      LatestVersion.Known = true;

      return true;
    }
  }
}
