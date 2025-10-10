using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;

namespace SkyRoof
{
  public partial class DownloadDialog : Form
  {
    private Context ctx;
    private SatnogsDb? db;

    public DownloadDialog()
    {
      InitializeComponent();
    }

    public static bool Download(Form parent, Context ctx)
    {
      var dlg = new DownloadDialog();
      dlg.ctx = ctx;

      var rc = dlg.ShowDialog(parent);
      return rc == DialogResult.OK;
    }

    private void Button_Click(object sender, EventArgs e)
    {
      db?.AbortDownload();
    }

    private async void DownloadDialog_Shown(object sender, EventArgs e)
    {
      db = new();
      db.DownloadProgress += SatnogsDb_DownloadProgress;

      // download files
      try
      {
        await db.DownloadAll();
      }
      catch (Exception ex)
      {
        ErrorLabel.Text = "Download Failed";
        Button.Text = "Close";
        Log.Error(ex, ErrorLabel.Text);
        db = null;
        DialogResult = DialogResult.None;
        return;
      }

      // import files
      try
      {
        db.ImportAll();
        ctx.SatnogsDb.ReplaceSatelliteList(db);
        DialogResult = DialogResult.OK;
      }
      catch (Exception ex)
      {
        ErrorLabel.Text = "Data Import Failed";
        Button.Text = "Close";
        Log.Error(ex, ErrorLabel.Text);
      }
    }

    private void SatnogsDb_DownloadProgress(object? sender, ProgressChangedEventArgs e)
    {
      progressBar1.Value = e.ProgressPercentage;
    }
  }
}
