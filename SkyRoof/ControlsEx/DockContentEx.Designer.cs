namespace SkyRoof
{
  partial class DockContentEx
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
      components = new System.ComponentModel.Container();
      contextMenuStrip1 = new ContextMenuStrip(components);
      SelectSatelliteMNU = new ToolStripMenuItem();
      SatelliteDetailsMNU = new ToolStripMenuItem();
      SatelliteTransmittersMNU = new ToolStripMenuItem();
      EarthViewMNU = new ToolStripMenuItem();
      contextMenuStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // contextMenuStrip1
      // 
      contextMenuStrip1.Items.AddRange(new ToolStripItem[] { SelectSatelliteMNU, SatelliteDetailsMNU, SatelliteTransmittersMNU, EarthViewMNU });
      contextMenuStrip1.Name = "contextMenuStrip1";
      contextMenuStrip1.Size = new Size(212, 114);
      contextMenuStrip1.Opening += contextMenuStrip1_Opening;
      // 
      // SelectSatelliteMNU
      // 
      SelectSatelliteMNU.Name = "SelectSatelliteMNU";
      SelectSatelliteMNU.ShortcutKeyDisplayString = "Dbl-Click";
      SelectSatelliteMNU.Size = new Size(211, 22);
      SelectSatelliteMNU.Text = "Select Satellite";
      SelectSatelliteMNU.Click += SelectSatelliteMNU_Click;
      // 
      // SatelliteDetailsMNU
      // 
      SatelliteDetailsMNU.Name = "SatelliteDetailsMNU";
      SatelliteDetailsMNU.Size = new Size(211, 22);
      SatelliteDetailsMNU.Text = "Satellite Details...";
      SatelliteDetailsMNU.Click += SatelliteDetailsMNU_Click;
      // 
      // SatelliteTransmittersMNU
      // 
      SatelliteTransmittersMNU.Name = "SatelliteTransmittersMNU";
      SatelliteTransmittersMNU.Size = new Size(211, 22);
      SatelliteTransmittersMNU.Text = "Satellite Transmitters...";
      SatelliteTransmittersMNU.Click += SatelliteTransmittersMNU_Click;
      // 
      // EarthViewMNU
      // 
      EarthViewMNU.Name = "EarthViewMNU";
      EarthViewMNU.Size = new Size(211, 22);
      EarthViewMNU.Text = "Earth View from Satellite...";
      EarthViewMNU.Click += EarthViewMNU_Click;
      // 
      // DockContentEx
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(800, 450);
      Name = "DockContentEx";
      Text = "DockContentEx";
      contextMenuStrip1.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion
    private ToolStripMenuItem SelectSatelliteMNU;
    private ToolStripMenuItem SatelliteDetailsMNU;
    private ToolStripMenuItem SatelliteTransmittersMNU;
    protected ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem EarthViewMNU;
  }
}