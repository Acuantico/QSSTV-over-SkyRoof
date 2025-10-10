using SkyRoof.Properties;

namespace VE3NEA
{
  internal class PaletteManager
  {
    private readonly string PaletteFolder;
    public Palette[] Palettes;

    public PaletteManager()
    {
      PaletteFolder = Path.Combine(Utils.GetUserDataFolder(), "Palettes");
      Directory.CreateDirectory(PaletteFolder);
      LoadPalettes();
    }

    private void LoadPalettes()
    {
      var paletteFiles = ListOrCreatePaletteFiles().Order();
      Palettes = paletteFiles.Select(f => Palette.CreateFromFile(f)).ToArray();
    }

    private string[] ListOrCreatePaletteFiles()
    {
      string[] paletteFiles = Directory.EnumerateFiles(PaletteFolder).ToArray();
      if (paletteFiles.Length > 0) return paletteFiles;

      foreach (var entry in Entries)
      {
        var fields = entry.Split('=');
        File.WriteAllText(Path.Combine(PaletteFolder, fields[0] + ".txt"), fields[1].Replace(',', '\n'));
      }

      return Directory.EnumerateFiles(PaletteFolder).ToArray();
    }

    string[] Entries = [
      "00=#000d4e,#00d8ff,#b3ff46,#ffe900,#ff0000",
      "01=#132a13,#31572c,#4f772d,#90a955,#ecf39e",
      "02=#000000,#7c0000,#ff7c00,#ffff7c,#fdffe5",
      "03=#e5fffd,#7cffff,#007cff,#00007c,#000000",
      "04=#000000,#00006e,#00786e,#28a500,#d27d00,#ff211e,#ffffff",
      "05=#0d004e,#d800ff,#ffb346,#e9ff00,#00ff00"
      ];

  }
}

