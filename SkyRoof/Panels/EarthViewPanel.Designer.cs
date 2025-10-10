namespace SkyRoof
{
  partial class EarthViewPanel
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
      openglControl1 = new SharpGL.OpenGLControl();
      label1 = new Label();
      ((System.ComponentModel.ISupportInitialize)openglControl1).BeginInit();
      SuspendLayout();
      // 
      // openglControl1
      // 
      openglControl1.Dock = DockStyle.Fill;
      openglControl1.DrawFPS = false;
      openglControl1.Location = new Point(0, 0);
      openglControl1.Margin = new Padding(4, 3, 4, 3);
      openglControl1.Name = "openglControl1";
      openglControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL3_3;
      openglControl1.RenderContextType = SharpGL.RenderContextType.NativeWindow;
      openglControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
      openglControl1.Size = new Size(400, 379);
      openglControl1.TabIndex = 1;
      openglControl1.OpenGLInitialized += openglControl1_OpenGLInitialized;
      openglControl1.OpenGLDraw += openglControl1_OpenGLDraw;
      openglControl1.Resize += openglControl1_Resize;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
      label1.Location = new Point(4, 4);
      label1.Name = "label1";
      label1.Size = new Size(27, 19);
      label1.TabIndex = 2;
      label1.Text = "___";
      // 
      // EarthViewPanel
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(400, 379);
      Controls.Add(label1);
      Controls.Add(openglControl1);
      Name = "EarthViewPanel";
      Text = "Earth View";
      FormClosing += EarthViewPanel_FormClosing;
      ((System.ComponentModel.ISupportInitialize)openglControl1).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private SharpGL.OpenGLControl openglControl1;
    private Label label1;
  }
}