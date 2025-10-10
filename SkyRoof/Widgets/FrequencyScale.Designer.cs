namespace SkyRoof
{
  partial class FrequencyScale
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
      SuspendLayout();
      // 
      // FrequencyScale
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Cursor = Cursors.Cross;
      Name = "FrequencyScale";
      Size = new Size(357, 87);
      Paint += FrequencyScale_Paint;
      Resize += FrequencyScale_Resize;
      ResumeLayout(false);
    }

    #endregion
  }
}
