namespace SkyRoof
{
  partial class AzElEntryDialog
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
      CancelBtn = new Button();
      OkBtn = new Button();
      label1 = new Label();
      label2 = new Label();
      AzimuthSpinner = new NumericUpDown();
      ElevationSpinner = new NumericUpDown();
      panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)AzimuthSpinner).BeginInit();
      ((System.ComponentModel.ISupportInitialize)ElevationSpinner).BeginInit();
      SuspendLayout();
      // 
      // panel1
      // 
      panel1.Controls.Add(CancelBtn);
      panel1.Controls.Add(OkBtn);
      panel1.Dock = DockStyle.Bottom;
      panel1.Location = new Point(0, 75);
      panel1.Name = "panel1";
      panel1.Size = new Size(213, 36);
      panel1.TabIndex = 0;
      // 
      // CancelBtn
      // 
      CancelBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      CancelBtn.Location = new Point(126, 7);
      CancelBtn.Name = "CancelBtn";
      CancelBtn.Size = new Size(75, 23);
      CancelBtn.TabIndex = 5;
      CancelBtn.Text = "Cancel";
      CancelBtn.UseVisualStyleBackColor = true;
      // 
      // OkBtn
      // 
      OkBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      OkBtn.Location = new Point(41, 7);
      OkBtn.Name = "OkBtn";
      OkBtn.Size = new Size(75, 23);
      OkBtn.TabIndex = 3;
      OkBtn.Text = "Go";
      OkBtn.UseVisualStyleBackColor = true;
      OkBtn.Click += OkBtn_Click;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 12F);
      label1.Location = new Point(19, 9);
      label1.Name = "label1";
      label1.Size = new Size(68, 21);
      label1.TabIndex = 1;
      label1.Text = "Azimuth";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Font = new Font("Segoe UI", 12F);
      label2.Location = new Point(121, 9);
      label2.Name = "label2";
      label2.Size = new Size(73, 21);
      label2.TabIndex = 2;
      label2.Text = "Elevation";
      // 
      // AzimuthSpinner
      // 
      AzimuthSpinner.Font = new Font("Segoe UI", 12F);
      AzimuthSpinner.Location = new Point(20, 35);
      AzimuthSpinner.Name = "AzimuthSpinner";
      AzimuthSpinner.Size = new Size(68, 29);
      AzimuthSpinner.TabIndex = 1;
      // 
      // ElevationSpinner
      // 
      ElevationSpinner.Font = new Font("Segoe UI", 12F);
      ElevationSpinner.Location = new Point(123, 35);
      ElevationSpinner.Name = "ElevationSpinner";
      ElevationSpinner.Size = new Size(68, 29);
      ElevationSpinner.TabIndex = 2;
      // 
      // AzElEntryDialog
      // 
      AcceptButton = OkBtn;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      CancelButton = CancelBtn;
      ClientSize = new Size(213, 111);
      Controls.Add(ElevationSpinner);
      Controls.Add(AzimuthSpinner);
      Controls.Add(label2);
      Controls.Add(label1);
      Controls.Add(panel1);
      FormBorderStyle = FormBorderStyle.FixedToolWindow;
      Name = "AzElEntryDialog";
      ShowInTaskbar = false;
      StartPosition = FormStartPosition.Manual;
      Text = "Manual Rotator Control";
      panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)AzimuthSpinner).EndInit();
      ((System.ComponentModel.ISupportInitialize)ElevationSpinner).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Panel panel1;
    private Button CancelBtn;
    private Button OkBtn;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label label4;
    private NumericUpDown AzimuthSpinner;
    private NumericUpDown ElevationSpinner;
  }
}