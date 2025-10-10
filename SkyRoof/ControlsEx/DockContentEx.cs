using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SkyRoof
{
  public partial class DockContentEx : DockContent
  {
    protected readonly Context ctx;
    protected SatnogsDbSatellite? ClickedSat;


    public DockContentEx()
    {
      InitializeComponent();
    }

    public DockContentEx(Context ctx)
    {
      this.ctx = ctx;
      InitializeComponent();
    }


    //----------------------------------------------------------------------------------------------
    //                                    popup menu
    //----------------------------------------------------------------------------------------------
    private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      SelectSatelliteMNU.Enabled = SatelliteDetailsMNU.Enabled = SatelliteTransmittersMNU.Enabled = ClickedSat != null;
    }

    private void SelectSatelliteMNU_Click(object sender, EventArgs e)
    {
      ctx.SatelliteSelector.SetSelectedSatellite(ClickedSat!);
    }

    private void SatelliteDetailsMNU_Click(object sender, EventArgs e)
    {
      ctx.SatelliteSelector.SetSelectedSatellite(ClickedSat!);

      if (ctx.SatelliteDetailsPanel != null)
        ctx.SatelliteDetailsPanel.Activate();
      else
        new SatelliteDetailsPanel(ctx).Show(ctx.MainForm.DockHost, DockState.Float);
    }

    private void SatelliteTransmittersMNU_Click(object sender, EventArgs e)
    {
      ctx.SatelliteSelector.SetSelectedSatellite(ClickedSat!);

      if (ctx.TransmittersPanel != null)
        ctx.TransmittersPanel.Activate();
      else
        new TransmittersPanel(ctx).Show(ctx.MainForm.DockHost, DockState.Float);
    }

    private void EarthViewMNU_Click(object sender, EventArgs e)
    {
      ctx.SatelliteSelector.SetSelectedSatellite(ClickedSat!);

      if (ctx.EarthViewPanel != null)
        ctx.EarthViewPanel.Activate();
      else
        new EarthViewPanel(ctx).Show(ctx.MainForm.DockHost, DockState.Float);
    }
  }
}
