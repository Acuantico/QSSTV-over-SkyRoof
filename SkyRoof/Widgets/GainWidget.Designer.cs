namespace SkyRoof
{
  partial class GainWidget
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      label1 = new Label();
      RfGainSlider = new TrackBar();
      AfGainLabel = new Label();
      label2 = new Label();
      AfGainSlider = new TrackBar();
      RfGainLabel = new Label();
      ((System.ComponentModel.ISupportInitialize)RfGainSlider).BeginInit();
      ((System.ComponentModel.ISupportInitialize)AfGainSlider).BeginInit();
      SuspendLayout();
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(4, 11);
      label1.Name = "label1";
      label1.Size = new Size(47, 15);
      label1.TabIndex = 25;
      label1.Text = "RF Gain";
      // 
      // RfGainSlider
      // 
      RfGainSlider.AutoSize = false;
      RfGainSlider.Location = new Point(46, 10);
      RfGainSlider.Maximum = 100;
      RfGainSlider.Name = "RfGainSlider";
      RfGainSlider.Size = new Size(121, 32);
      RfGainSlider.TabIndex = 24;
      RfGainSlider.TickFrequency = 10;
      RfGainSlider.Value = 100;
      RfGainSlider.ValueChanged += RfGainSlider_ValueChanged;
      // 
      // AfGainLabel
      // 
      AfGainLabel.BackColor = SystemColors.Control;
      AfGainLabel.Location = new Point(164, 37);
      AfGainLabel.MinimumSize = new Size(40, 0);
      AfGainLabel.Name = "AfGainLabel";
      AfGainLabel.Size = new Size(43, 24);
      AfGainLabel.TabIndex = 23;
      AfGainLabel.Text = "-44 dB";
      AfGainLabel.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(4, 39);
      label2.Name = "label2";
      label2.Size = new Size(48, 15);
      label2.TabIndex = 22;
      label2.Text = "AF Gain";
      // 
      // AfGainSlider
      // 
      AfGainSlider.AutoSize = false;
      AfGainSlider.LargeChange = 1;
      AfGainSlider.Location = new Point(46, 39);
      AfGainSlider.Maximum = 0;
      AfGainSlider.Minimum = -50;
      AfGainSlider.Name = "AfGainSlider";
      AfGainSlider.Size = new Size(121, 32);
      AfGainSlider.TabIndex = 21;
      AfGainSlider.TickFrequency = 10;
      AfGainSlider.Value = -25;
      AfGainSlider.ValueChanged += AfGainSlider_ValueChanged;
      // 
      // RfGainLabel
      // 
      RfGainLabel.BackColor = SystemColors.Control;
      RfGainLabel.Location = new Point(164, 8);
      RfGainLabel.MinimumSize = new Size(40, 0);
      RfGainLabel.Name = "RfGainLabel";
      RfGainLabel.Size = new Size(43, 24);
      RfGainLabel.TabIndex = 26;
      RfGainLabel.Text = "0";
      RfGainLabel.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // GainControl
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      BorderStyle = BorderStyle.FixedSingle;
      Controls.Add(RfGainLabel);
      Controls.Add(label1);
      Controls.Add(RfGainSlider);
      Controls.Add(AfGainLabel);
      Controls.Add(label2);
      Controls.Add(AfGainSlider);
      Name = "GainControl";
      Size = new Size(206, 75);
      ((System.ComponentModel.ISupportInitialize)RfGainSlider).EndInit();
      ((System.ComponentModel.ISupportInitialize)AfGainSlider).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    public TrackBar RfGainSlider;
    public TrackBar AfGainSlider;
    public Label label1;
    public Label label2;
    public Label AfGainLabel;
    public Label RfGainLabel;
  }
}
