namespace SkyRoof
{
  partial class FrequencyEntryForm
  {
    private System.ComponentModel.IContainer components = null;

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
      label1 = new Label();
      FrequencyComboBox = new ComboBox();
      TuneBtn = new Button();
      SuspendLayout();
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(13, 13);
      label1.Margin = new Padding(4, 0, 4, 0);
      label1.Name = "label1";
      label1.Size = new Size(82, 15);
      label1.TabIndex = 0;
      label1.Text = "Frequency, Hz";
      // 
      // FrequencyComboBox
      // 
      FrequencyComboBox.Font = new Font("Segoe UI", 11F);
      FrequencyComboBox.FormatString = "N2";
      FrequencyComboBox.FormattingEnabled = true;
      FrequencyComboBox.Location = new Point(13, 31);
      FrequencyComboBox.Margin = new Padding(4, 3, 4, 3);
      FrequencyComboBox.Name = "FrequencyComboBox";
      FrequencyComboBox.Size = new Size(145, 28);
      FrequencyComboBox.TabIndex = 1;
      FrequencyComboBox.KeyDown += ComboBox_KeyDown;
      FrequencyComboBox.KeyPress += ComboBox_KeyPress;
      // 
      // TuneBtn
      // 
      TuneBtn.DialogResult = DialogResult.OK;
      TuneBtn.Location = new Point(176, 28);
      TuneBtn.Margin = new Padding(4, 3, 4, 3);
      TuneBtn.Name = "TuneBtn";
      TuneBtn.Size = new Size(72, 27);
      TuneBtn.TabIndex = 3;
      TuneBtn.Text = "Tune";
      TuneBtn.UseVisualStyleBackColor = true;
      // 
      // FrequencyEntryForm
      // 
      AcceptButton = TuneBtn;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(263, 66);
      Controls.Add(TuneBtn);
      Controls.Add(FrequencyComboBox);
      Controls.Add(label1);
      FormBorderStyle = FormBorderStyle.FixedToolWindow;
      Margin = new Padding(4, 3, 4, 3);
      Name = "FrequencyEntryForm";
      ShowInTaskbar = false;
      StartPosition = FormStartPosition.Manual;
      Text = "Tune to Frequency";
      FormClosing += FrequencyEntryForm_FormClosing;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox FrequencyComboBox;
        internal System.Windows.Forms.Button TuneBtn;
    }
}