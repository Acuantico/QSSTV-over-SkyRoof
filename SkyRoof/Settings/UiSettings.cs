using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace SkyRoof
{
  public class UiSettings
  {
    public Size MainWindowSize { get; set; }
    public Point MainWindowLocation { get; set; }
    public string? DockingLayoutString { get; set; }
    public bool ClockUtcMode { get; set; }
    public SatelliteGroupsFormSettings SatelliteGroupsForm { get; set; } = new();
    public SatelliteDetailsFormSettings SatelliteDetailsForm { get; set; } = new();
    public SatelliteDetailsPanelSettings SatelliteDetailsPanel { get; set; } = new();
    public SatellitePassesPanelSettings SatellitePassesPanel { get; set; } = new();

    public void StoreWindowPosition(Form form)
    {
      if (form.WindowState == FormWindowState.Normal)
      {
        MainWindowLocation = form.Location;
        MainWindowSize = form.Size;
      }
      else
      {
        MainWindowLocation = form.RestoreBounds.Location;
        MainWindowSize = form.RestoreBounds.Size;
      }
    }
    public void RestoreWindowPosition(Form form)
    {
      if (MainWindowSize.Width > 0)
      {
        form.Location = MainWindowLocation;
        form.Size = MainWindowSize;
      }
    }

    internal void StoreDockingLayout(DockPanel dockHost)
    {
      using (var stream = new MemoryStream())
      {
        dockHost.SaveAsXml(stream, Encoding.UTF8);
        DockingLayoutString = Encoding.UTF8.GetString(stream.ToArray());
      }
    }

    internal bool RestoreDockingLayout(MainForm form)
    {
      if (string.IsNullOrEmpty(DockingLayoutString)) return false;

      DeserializeDockContent desCont = new DeserializeDockContent(form.MakeDockContentFromPersistString);

      using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(DockingLayoutString)))
        form.DockHost.LoadFromXml(stream, desCont);

      return true;
    }
  }

  public class SatellitePassesPanelSettings
  {
    public int RadioButtoIndex { get; set; }
  }

  public class SatelliteDetailsPanelSettings
  {
    public int SplitterDistance { get; set; } = -1;
  }

  public class SatelliteDetailsFormSettings
  {
    public Size Size { get; set; }
    public int SplitterDistance { get; set; } = -1;
  }

  public class SatelliteGroupsFormSettings
  {
    public Size Size { get; set; }
  }
}
