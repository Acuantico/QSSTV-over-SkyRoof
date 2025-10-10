using VE3NEA;

namespace SkyRoof
{
  public partial class AzElEntryDialog : Form
  {
    private Context ctx;

    public AzElEntryDialog()
    {
      InitializeComponent();
    }

    public void Open(Context ctx)
    {
      this.ctx = ctx;
      SetupSpinners();
      Location = Cursor.Position;
      ShowDialog();
    }

    private void SetupSpinners()
    {
      AzimuthSpinner.Minimum = ctx.Settings.Rotator.MinAzimuth;
      AzimuthSpinner.Maximum = ctx.Settings.Rotator.MaxAzimuth;
      ElevationSpinner.Minimum = ctx.Settings.Rotator.MinElevation;
      ElevationSpinner.Maximum = ctx.Settings.Rotator.MaxElevation;
    }

    private void OkBtn_Click(object sender, EventArgs e)
    {
      StartRotation();
      Close();
    }

    private void StartRotation()
    {
      ctx.RotatorControl.TrackCheckbox.Checked = false;
      var bearing = new Bearing((double)AzimuthSpinner.Value * Trig.RinD, (double)ElevationSpinner.Value * Trig.RinD);
      ctx.RotatorControl.RotateTo(bearing);
    }
  }
}
