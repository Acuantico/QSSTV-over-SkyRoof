namespace SkyRoof
{
  partial class SstvPanel
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private ToolStrip toolStrip;
    private ToolStripLabel modeLabel;
    private ToolStripComboBox modeCombo;
    private ToolStripButton clearButton;
    private ToolStripButton saveButton;
    private ToolStripButton clearHistoryButton;
    private SplitContainer splitContainer;
    private PictureBox pictureBox;
    private ListView historyListView;
    private ColumnHeader columnHeaderFile;
    private ColumnHeader columnHeaderTime;
    private ColumnHeader columnHeaderStatus;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        modeCombo.SelectedIndexChanged -= ModeCombo_SelectedIndexChanged;
        historyListView.SelectedIndexChanged -= HistoryListView_SelectedIndexChanged;
        historyListView.DoubleClick -= HistoryListView_DoubleClick;
        components?.Dispose();
        currentImage?.Dispose();
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
      toolStrip = new ToolStrip();
      modeLabel = new ToolStripLabel();
      modeCombo = new ToolStripComboBox();
      clearButton = new ToolStripButton();
      saveButton = new ToolStripButton();
      clearHistoryButton = new ToolStripButton();
      splitContainer = new SplitContainer();
      pictureBox = new PictureBox();
      historyListView = new ListView();
      columnHeaderFile = new ColumnHeader();
      columnHeaderTime = new ColumnHeader();
      columnHeaderStatus = new ColumnHeader();
      statusStrip = new StatusStrip();
      statusLabel = new ToolStripStatusLabel();
      toolStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
      splitContainer.Panel1.SuspendLayout();
      splitContainer.Panel2.SuspendLayout();
      splitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
      statusStrip.SuspendLayout();
      SuspendLayout();
      // 
      // toolStrip
      // 
      toolStrip.ImageScalingSize = new Size(20, 20);
      toolStrip.Items.AddRange(new ToolStripItem[] { modeLabel, modeCombo, clearButton, saveButton, clearHistoryButton });
      toolStrip.Location = new Point(0, 0);
      toolStrip.Name = "toolStrip";
      toolStrip.Size = new Size(784, 27);
      toolStrip.TabIndex = 0;
      toolStrip.Text = "toolStrip";
      // 
      // modeLabel
      // 
      modeLabel.Name = "modeLabel";
      modeLabel.Size = new Size(49, 24);
      modeLabel.Text = "Mode:";
      // 
      // modeCombo
      // 
      modeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
      modeCombo.Name = "modeCombo";
      modeCombo.Size = new Size(220, 27);
      // 
      // clearButton
      // 
      clearButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
      clearButton.Name = "clearButton";
      clearButton.Size = new Size(43, 24);
      clearButton.Text = "Clear";
      clearButton.Click += ClearButton_Click;
      // 
      // saveButton
      // 
      saveButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
      saveButton.Name = "saveButton";
      saveButton.Size = new Size(35, 24);
      saveButton.Text = "Save";
      saveButton.Enabled = false;
      saveButton.Click += SaveButton_Click;
      // 
      // clearHistoryButton
      // 
      clearHistoryButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
      clearHistoryButton.Name = "clearHistoryButton";
      clearHistoryButton.Size = new Size(89, 24);
      clearHistoryButton.Text = "Clear Photos";
      clearHistoryButton.Click += ClearHistoryButton_Click;
      // 
      // splitContainer
      // 
      splitContainer.Dock = DockStyle.Fill;
      splitContainer.Location = new Point(0, 27);
      splitContainer.Name = "splitContainer";
      splitContainer.Orientation = Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      splitContainer.Panel1.Controls.Add(pictureBox);
      // 
      // splitContainer.Panel2
      // 
      splitContainer.Panel2.Controls.Add(historyListView);
      splitContainer.Size = new Size(784, 512);
      splitContainer.SplitterDistance = 360;
      splitContainer.TabIndex = 3;
      // 
      // pictureBox
      // 
      pictureBox.Dock = DockStyle.Fill;
      pictureBox.Location = new Point(0, 0);
      pictureBox.Name = "pictureBox";
      pictureBox.Size = new Size(784, 360);
      pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
      pictureBox.TabIndex = 0;
      pictureBox.TabStop = false;
      // 
      // historyListView
      // 
      historyListView.Columns.AddRange(new ColumnHeader[] { columnHeaderFile, columnHeaderTime, columnHeaderStatus });
      historyListView.Dock = DockStyle.Fill;
      historyListView.FullRowSelect = true;
      historyListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      historyListView.HideSelection = false;
      historyListView.Location = new Point(0, 0);
      historyListView.MultiSelect = false;
      historyListView.Name = "historyListView";
      historyListView.Size = new Size(784, 148);
      historyListView.TabIndex = 0;
      historyListView.UseCompatibleStateImageBehavior = false;
      historyListView.View = View.Details;
      historyListView.SelectedIndexChanged += HistoryListView_SelectedIndexChanged;
      historyListView.DoubleClick += HistoryListView_DoubleClick;
      // 
      // columnHeaderFile
      // 
      columnHeaderFile.Text = "File";
      columnHeaderFile.Width = 360;
      // 
      // columnHeaderTime
      // 
      columnHeaderTime.Text = "Timestamp";
      columnHeaderTime.Width = 220;
      // 
      // columnHeaderStatus
      // 
      columnHeaderStatus.Text = "Status";
      columnHeaderStatus.Width = 120;
      // 
      // statusStrip
      // 
      statusStrip.ImageScalingSize = new Size(20, 20);
      statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
      statusStrip.Location = new Point(0, 539);
      statusStrip.Name = "statusStrip";
      statusStrip.Size = new Size(784, 22);
      statusStrip.TabIndex = 1;
      statusStrip.Text = "statusStrip";
      // 
      // statusLabel
      // 
      statusLabel.Name = "statusLabel";
      statusLabel.Size = new Size(124, 17);
      statusLabel.Text = "Waiting for signal...";
      // 
      // SstvPanel
      // 
      AutoScaleDimensions = new SizeF(8F, 20F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(784, 561);
      Controls.Add(splitContainer);
      Controls.Add(statusStrip);
      Controls.Add(toolStrip);
      Name = "SstvPanel";
      TabText = "SSTV Decoder";
      Text = "SSTV Decoder";
      FormClosing += SstvPanel_FormClosing;
      toolStrip.ResumeLayout(false);
      toolStrip.PerformLayout();
      splitContainer.Panel1.ResumeLayout(false);
      splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
      splitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
      statusStrip.ResumeLayout(false);
      statusStrip.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
  }
}
