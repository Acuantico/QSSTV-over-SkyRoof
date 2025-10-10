namespace SkyRoof
{
  partial class DownloadDialog
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
      label1 = new Label();
      progressBar1 = new ProgressBar();
      Button = new Button();
      ErrorLabel = new Label();
      SuspendLayout();
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(12, 18);
      label1.Name = "label1";
      label1.Size = new Size(158, 15);
      label1.TabIndex = 0;
      label1.Text = "Downloading Satellite Data...";
      // 
      // progressBar1
      // 
      progressBar1.Location = new Point(12, 36);
      progressBar1.Name = "progressBar1";
      progressBar1.Size = new Size(340, 25);
      progressBar1.TabIndex = 1;
      // 
      // Button
      // 
      Button.Location = new Point(145, 95);
      Button.Name = "Button";
      Button.Size = new Size(75, 23);
      Button.TabIndex = 2;
      Button.Text = "Cancel";
      Button.UseVisualStyleBackColor = true;
      Button.Click += Button_Click;
      // 
      // ErrorLabel
      // 
      ErrorLabel.AutoSize = true;
      ErrorLabel.ForeColor = Color.Red;
      ErrorLabel.Location = new Point(12, 68);
      ErrorLabel.Name = "ErrorLabel";
      ErrorLabel.Size = new Size(58, 15);
      ErrorLabel.TabIndex = 3;
      ErrorLabel.Text = "                 ";
      // 
      // DownloadDialog
      // 
      AcceptButton = Button;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      CancelButton = Button;
      ClientSize = new Size(364, 130);
      Controls.Add(ErrorLabel);
      Controls.Add(Button);
      Controls.Add(progressBar1);
      Controls.Add(label1);
      FormBorderStyle = FormBorderStyle.FixedToolWindow;
      Name = "DownloadDialog";
      ShowInTaskbar = false;
      StartPosition = FormStartPosition.CenterScreen;
      Text = "Satellite Data";
      Shown += DownloadDialog_Shown;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label label1;
    private ProgressBar progressBar1;
    private Button Button;
    private Label ErrorLabel;
  }
}