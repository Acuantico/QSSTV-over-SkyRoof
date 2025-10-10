using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkyRoof
{
  public partial class SatelliteDetailsWidget : UserControl
  {
    private SatnogsDbSatellite? Satellite;

    public SatelliteDetailsWidget()
    {
      InitializeComponent();
    }

    internal void ShowSatellite(SatnogsDbSatellite? satellite)
    {
      if (satellite == null) return;

      Satellite = satellite;

      satellite.ComputeOrbitDetails();
      SatNameLabel.Text = satellite.name;
      SatAkaLabel.Visible = satellite.AllNames.Count > 1;
      SatAkaLabel.Text = $"a.k.a. {string.Join(", ", satellite.AllNames.Where(s => s != satellite.name))}";

      ImageLabel.Visible = !string.IsNullOrEmpty(satellite.image);
      WebsiteLabel.Visible = !string.IsNullOrEmpty(satellite.website);

      SatellitePropertyGrid.SelectedObject = satellite;

      CreateTransmitterItems();
    }

    private void CreateTransmitterItems()
    {
      listView1.BeginUpdate();
      listView1.Items.Clear();
      listView1.Groups.Clear();
      listView1.Groups.Add(new ListViewGroup("SatNOGS"));
      listView1.Groups.Add(new ListViewGroup("JE9PEL"));

      // satnogs transmitters
      foreach (var tx in Satellite.Transmitters)
      {
        // columns
        var item = new ListViewItem([
          tx.description,
          SatnogsDbTransmitter.FormatFrequencyRange(tx.downlink_low, tx.downlink_high, tx.invert),
          SatnogsDbTransmitter.FormatFrequencyRange(tx.uplink_low, tx.uplink_high),
        ]);
        item.Group = listView1.Groups[0];

        // highlighting
        if (tx.IsVhf()) item.BackColor = Color.LightGoldenrodYellow;
        if (tx.IsUhf()) item.BackColor = Color.LightCyan;
        if (tx.service == "Amateur") item.Font = new(item.Font, FontStyle.Bold);
        if (!tx.alive || tx.status != "active") item.ForeColor = Color.Silver; 
        
        // tooltip
        item.ToolTipText = tx.GetTooltipText();

        listView1.Items.Add(item);
      }

      // JE9PEL transmitters
      foreach (var t in Satellite.JE9PELtransmitters)
      {
        var item = new ListViewItem([t.Mode, t.Downlink, t.Uplink]);
        item.Group = listView1.Groups[1];
        if (t.Status != "active") item.ForeColor = Color.Silver;
        item.ToolTipText = t.GetTooltipText();

        // band color
        var match = Regex.Match(t.Downlink, "^[0-9.]+");
        if (match.Success && float.TryParse(match.Groups[0].Value, CultureInfo.InvariantCulture, out float freq))
          if (freq >= 144&& freq <= 148) item.BackColor = Color.LightGoldenrodYellow;
          else if (freq >= 430 && freq <= 440) item.BackColor = Color.LightCyan;

        listView1.Items.Add(item);
      }

      listView1.EndUpdate();
    }

    private void ImageLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      ImageLabel.LinkVisited = true;
      Process.Start(new ProcessStartInfo($"https://db-satnogs.freetls.fastly.net/media/{Satellite.image}") { UseShellExecute = true });
    }

    private void WebsiteLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      WebsiteLabel.LinkVisited = true;
      Process.Start(new ProcessStartInfo(Satellite.website) { UseShellExecute = true });
    }

    private void SatnogsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      SatnogsLabel.LinkVisited = true;
      Process.Start(new ProcessStartInfo($"https://db.satnogs.org/satellite/{Satellite.sat_id}") { UseShellExecute = true });
    }
  }
}
