using VE3NEA;

namespace SkyRoof
{
  internal partial class UserDetailsDialog : Form
  {
    private Context ctx;

    internal UserDetailsDialog()
    {
      InitializeComponent();
    }

    internal UserDetailsDialog(Context ctx)
    {
      this.ctx = ctx;

      InitializeComponent();

      LoadSettings();
      ValidateAll();
    }

    internal static bool UserDetailsAvailable(Context ctx)
    {
      var sett = ctx.Settings.User;
      return !string.IsNullOrEmpty(sett?.Square);
    }

    private void okBtn_Click(object sender, EventArgs e)
    {
      SaveSettings();
    }

    private void LoadSettings()
    {
      textBox1.Text = ctx.Settings.User.Call;
      textBox2.Text = ctx.Settings.User.Square;
      numericUpDown1.Value = ctx.Settings.User.Altitude;
    }

    private void SaveSettings()
    {
      ctx.Settings.User.Call = textBox1.Text.Trim();
      ctx.Settings.User.Square = textBox2.Text.Trim();
      ctx.Settings.User.Altitude = (int)numericUpDown1.Value;
    }

    private void textBoxes_TextChanged(object sender, EventArgs e)
    {
      ValidateAll();
    }

    private void ValidateAll()
    {
      bool ok = textBox2.Text.Length == 6 && GridSquare.IsValid(textBox2.Text.Trim());
      label3.ForeColor = ok ? SystemColors.WindowText : Color.Red;

      okBtn.Enabled = ok;
    }
  }
}