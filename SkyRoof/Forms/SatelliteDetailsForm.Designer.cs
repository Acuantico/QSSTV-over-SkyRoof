namespace SkyRoof
{
  partial class SatelliteDetailsForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      panel1 = new Panel();
      button1 = new Button();
      satelliteDetailsControl1 = new SatelliteDetailsWidget();
      panel1.SuspendLayout();
      SuspendLayout();
      // 
      // panel1
      // 
      panel1.Controls.Add(button1);
      panel1.Dock = DockStyle.Bottom;
      panel1.Location = new Point(0, 575);
      panel1.Name = "panel1";
      panel1.Size = new Size(471, 33);
      panel1.TabIndex = 0;
      // 
      // button1
      // 
      button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      button1.Location = new Point(384, 5);
      button1.Name = "button1";
      button1.Size = new Size(75, 23);
      button1.TabIndex = 0;
      button1.Text = "Close";
      button1.UseVisualStyleBackColor = true;
      // 
      // satelliteDetailsControl1
      // 
      satelliteDetailsControl1.Dock = DockStyle.Fill;
      satelliteDetailsControl1.Location = new Point(0, 0);
      satelliteDetailsControl1.Name = "satelliteDetailsControl1";
      satelliteDetailsControl1.Size = new Size(471, 575);
      satelliteDetailsControl1.TabIndex = 1;
      // 
      // SatelliteDetailsDialog
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      CancelButton = button1;
      ClientSize = new Size(471, 608);
      Controls.Add(satelliteDetailsControl1);
      Controls.Add(panel1);
      FormBorderStyle = FormBorderStyle.SizableToolWindow;
      Name = "SatelliteDetailsDialog";
      ShowIcon = false;
      ShowInTaskbar = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "Satellite Details";
      panel1.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    private Panel panel1;
    private Button button1;
    private SatelliteDetailsWidget satelliteDetailsControl1;
  }
}