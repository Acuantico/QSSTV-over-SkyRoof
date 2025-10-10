using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VE3NEA.Clock
{
  public partial class ClockWidget : UserControl
  {
    bool utcMode;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [DisplayName("UTC Mode")]
    public bool UtcMode { get => utcMode; set => SetUtcMode(value); }

    public ClockWidget()
    {
      InitializeComponent();
    }

    private void utcLabel_Click(object sender, EventArgs e)
    {
      SetUtcMode(!utcMode);
    }

    public void SetUtcMode(bool value)
    {
      utcMode = value;
      utcLabel.BackColor = utcMode ? Color.Aqua : Color.Teal;
      localLabel.BackColor = utcMode ? Color.Teal : Color.Aqua;
      ShowTime();
    }

    public void ShowTime()
    {
      DateTime now = utcMode ? DateTime.UtcNow : DateTime.Now;
      timeLabel.Text = now.ToString("HH:mm:ss");
      dateLabel.Text = now.ToString("MMMM dd, yyyy");
    }

    private void dateLabel_Click(object sender, EventArgs e)
    {

    }
  }
}
