namespace SkyRoof
{
    partial class PathOptimizerForm
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
      dataGridView = new DataGridView();
      CurrentDirection = new DataGridViewTextBoxColumn();
      RotationTime = new DataGridViewTextBoxColumn();
      FirstPoint = new DataGridViewTextBoxColumn();
      LastPoint = new DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
      SuspendLayout();
      // 
      // dataGridView
      // 
      dataGridView.AllowUserToAddRows = false;
      dataGridView.AllowUserToDeleteRows = false;
      dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridView.Columns.AddRange(new DataGridViewColumn[] { CurrentDirection, RotationTime, FirstPoint, LastPoint });
      dataGridView.Dock = DockStyle.Fill;
      dataGridView.Location = new Point(0, 0);
      dataGridView.Margin = new Padding(3, 2, 3, 2);
      dataGridView.Name = "dataGridView";
      dataGridView.ReadOnly = true;
      dataGridView.RowHeadersVisible = false;
      dataGridView.RowHeadersWidth = 51;
      dataGridView.RowTemplate.Height = 29;
      dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
      dataGridView.Size = new Size(405, 145);
      dataGridView.TabIndex = 0;
      // 
      // CurrentDirection
      // 
      CurrentDirection.HeaderText = "Current Point";
      CurrentDirection.MinimumWidth = 6;
      CurrentDirection.Name = "CurrentDirection";
      CurrentDirection.ReadOnly = true;
      CurrentDirection.SortMode = DataGridViewColumnSortMode.NotSortable;
      // 
      // RotationTime
      // 
      RotationTime.HeaderText = "Initial Rotation";
      RotationTime.MinimumWidth = 6;
      RotationTime.Name = "RotationTime";
      RotationTime.ReadOnly = true;
      RotationTime.SortMode = DataGridViewColumnSortMode.NotSortable;
      // 
      // FirstPoint
      // 
      FirstPoint.HeaderText = "First Point";
      FirstPoint.MinimumWidth = 6;
      FirstPoint.Name = "FirstPoint";
      FirstPoint.ReadOnly = true;
      FirstPoint.SortMode = DataGridViewColumnSortMode.NotSortable;
      // 
      // LastPoint
      // 
      LastPoint.HeaderText = "Last Point";
      LastPoint.MinimumWidth = 6;
      LastPoint.Name = "LastPoint";
      LastPoint.ReadOnly = true;
      LastPoint.SortMode = DataGridViewColumnSortMode.NotSortable;
      // 
      // PathOptimizerForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(405, 145);
      Controls.Add(dataGridView);
      Margin = new Padding(3, 2, 3, 2);
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "PathOptimizerForm";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "Path Optimizer Details";
      TopMost = true;
      ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView;
    private DataGridViewTextBoxColumn CurrentDirection;
    private DataGridViewTextBoxColumn RotationTime;
    private DataGridViewTextBoxColumn FirstPoint;
    private DataGridViewTextBoxColumn LastPoint;
  }
}