using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Serilog;
using VE3NEA;
using SkyRoof.Sstv;
using WeifenLuo.WinFormsUI.Docking;

namespace SkyRoof;

public partial class SstvPanel : DockContent
{
  private readonly Context ctx;
  private Bitmap? currentImage;
  private readonly string storageFolder;
  private readonly List<StoredImageRecord> history = new();
  private readonly List<SstvModeOption> _modeOptions = new()
  {
    new("Auto", null),
    new("Martin 1", SstvVisCodes.Martin1),
    new("Martin 2", SstvVisCodes.Martin2),
    new("Scottie 1", SstvVisCodes.Scottie1),
    new("Scottie 2", SstvVisCodes.Scottie2),
    new("Scottie DX", SstvVisCodes.ScottieDx),
    new("SC2 60", SstvVisCodes.Sc2_60),
    new("SC2 120", SstvVisCodes.Sc2_120),
    new("SC2 180", SstvVisCodes.Sc2_180),
    new("Robot 24", SstvVisCodes.Robot24),
    new("Robot 36", SstvVisCodes.Robot36),
    new("Robot 72", SstvVisCodes.Robot72),
    new("P3", SstvVisCodes.P3),
    new("P5", SstvVisCodes.P5),
    new("P7", SstvVisCodes.P7),
    new("B/W 8", SstvVisCodes.Bw8),
    new("B/W 12", SstvVisCodes.Bw12),
    new("PD50", SstvVisCodes.Pd50),
    new("PD90", SstvVisCodes.Pd90),
    new("PD120", SstvVisCodes.Pd120),
    new("PD160", SstvVisCodes.Pd160),
    new("PD180", SstvVisCodes.Pd180),
    new("PD240", SstvVisCodes.Pd240),
    new("PD290", SstvVisCodes.Pd290),
    new("MP73", SstvVisCodes.Mp73),
    new("MP115", SstvVisCodes.Mp115),
    new("MP140", SstvVisCodes.Mp140),
    new("MP175", SstvVisCodes.Mp175),
    new("MR73", SstvVisCodes.Mr73),
    new("MR90", SstvVisCodes.Mr90),
    new("MR115", SstvVisCodes.Mr115),
    new("MR140", SstvVisCodes.Mr140),
    new("MR175", SstvVisCodes.Mr175),
    new("ML180", SstvVisCodes.Ml180),
    new("ML240", SstvVisCodes.Ml240),
    new("ML280", SstvVisCodes.Ml280),
    new("ML320", SstvVisCodes.Ml320),
    new("FAX480", SstvVisCodes.Fax480),
    new("AVT24", SstvVisCodes.Avt24),
    new("AVT90", SstvVisCodes.Avt90),
    new("AVT94", SstvVisCodes.Avt94),
    new("MP73 Narrow", SstvVisCodes.Mp73N),
    new("MP110 Narrow", SstvVisCodes.Mp110N),
    new("MP140 Narrow", SstvVisCodes.Mp140N),
    new("MC110 Narrow", SstvVisCodes.Mc110N),
    new("MC140 Narrow", SstvVisCodes.Mc140N),
    new("MC180 Narrow", SstvVisCodes.Mc180N)
  };

  private bool _initializingMode;

  public SstvPanel(Context ctx)
  {
    InitializeComponent();
    this.ctx = ctx;
    ctx.SstvPanel = this;
    ctx.MainForm.SstvDecoderMNU.Checked = true;
    Log.Information("Creating SstvPanel");

    storageFolder = Path.Combine(Utils.GetUserDataFolder(), "SstvImages");
    Directory.CreateDirectory(storageFolder);
    LoadHistory();

    modeCombo.ComboBox.DisplayMember = nameof(SstvModeOption.DisplayName);
    modeCombo.Items.AddRange(_modeOptions.Cast<object>().ToArray());

    _initializingMode = true;
    var selected = ctx.SstvAutoMode
      ? _modeOptions[0]
      : _modeOptions.FirstOrDefault(m => m.VisCode == ctx.SstvManualVisCode) ?? _modeOptions[0];
    modeCombo.SelectedItem = selected;
    UpdateModeContext(selected);
    modeCombo.SelectedIndexChanged += ModeCombo_SelectedIndexChanged;
    _initializingMode = false;

    UpdateStatus("Waiting for signal...");
    UpdateSaveButtonState();
    ApplyCurrentMode();
  }

  internal void UpdateImage(Bitmap bitmap, bool isComplete)
  {
    if (IsDisposed) return;

    var previous = currentImage;
    currentImage = bitmap;
    pictureBox.Image = currentImage;
    previous?.Dispose();

    UpdateStatus(isComplete ? "Image complete" : "Receiving...");
    UpdateSaveButtonState();

    AutoStoreImage(bitmap, isComplete);
  }

  private void ClearImage()
  {
    if (IsDisposed) return;
    pictureBox.Image = null;
    currentImage?.Dispose();
    currentImage = null;
    UpdateStatus("Waiting for signal...");
    UpdateSaveButtonState();
  }

  private void ClearButton_Click(object sender, EventArgs e)
  {
    ctx.SstvDecoder?.Reset();
    ApplyCurrentMode();
    ClearImage();
  }

  private void SaveButton_Click(object sender, EventArgs e)
  {
    if (historyListView.SelectedItems.Count > 0)
    {
      var record = historyListView.SelectedItems[0].Tag as StoredImageRecord;
      if (record == null || !File.Exists(record.Path)) return;

      using var dialog = new SaveFileDialog
      {
        Filter = "PNG Image|*.png|Bitmap Image|*.bmp|JPEG Image|*.jpg",
        FileName = Path.GetFileNameWithoutExtension(record.FileName) + ".png",
        AddExtension = true,
        RestoreDirectory = true
      };

      if (dialog.ShowDialog(this) == DialogResult.OK)
      {
        try
        {
          File.Copy(record.Path, dialog.FileName, true);
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Failed to copy SSTV image");
          MessageBox.Show(this, "Unable to save the selected image.", "SSTV", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
      return;
    }

    if (currentImage == null) return;

    using var dialogCurrent = new SaveFileDialog
    {
      Filter = "PNG Image|*.png|Bitmap Image|*.bmp|JPEG Image|*.jpg",
      FileName = $"sstv_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png",
      AddExtension = true,
      RestoreDirectory = true
    };

    if (dialogCurrent.ShowDialog(this) == DialogResult.OK)
      currentImage.Save(dialogCurrent.FileName);
  }

  private void ClearHistoryButton_Click(object sender, EventArgs e)
  {
    if (history.Count == 0) return;

    var result = MessageBox.Show(this, "Remove all stored SSTV images?", "SSTV", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
    if (result != DialogResult.Yes) return;

    foreach (var record in history)
    {
      try
      {
        if (File.Exists(record.Path)) File.Delete(record.Path);
      }
      catch (Exception ex)
      {
        Log.Warning(ex, "Unable to delete stored SSTV image {File}", record.Path);
      }
    }

    history.Clear();
    RefreshHistoryList();
    UpdateSaveButtonState();
  }

  private void HistoryListView_SelectedIndexChanged(object? sender, EventArgs e)
  {
    UpdateSaveButtonState();
  }

  private void HistoryListView_DoubleClick(object? sender, EventArgs e)
  {
    if (historyListView.SelectedItems.Count == 0) return;
    if (historyListView.SelectedItems[0].Tag is not StoredImageRecord record) return;
    if (!File.Exists(record.Path)) return;

    try
    {
      Process.Start(new ProcessStartInfo(record.Path) { UseShellExecute = true });
    }
    catch (Exception ex)
    {
      Log.Error(ex, "Failed to open SSTV image {File}", record.Path);
    }
  }

  private void UpdateStatus(string text)
  {
    statusLabel.Text = text;
  }

  private void ApplyCurrentMode()
  {
    if (ctx.SstvDecoder == null) return;
    var option = modeCombo.SelectedItem as SstvModeOption ?? _modeOptions[0];
    ctx.SstvDecoder.SetMode(option.VisCode);
  }

  private void UpdateModeContext(SstvModeOption option)
  {
    if (option.VisCode.HasValue)
    {
      ctx.SstvAutoMode = false;
      ctx.SstvManualVisCode = option.VisCode;
    }
    else
    {
      ctx.SstvAutoMode = true;
      ctx.SstvManualVisCode = null;
    }
  }

  private void ModeCombo_SelectedIndexChanged(object? sender, EventArgs e)
  {
    if (_initializingMode) return;
    if (modeCombo.SelectedItem is not SstvModeOption option) return;

    UpdateModeContext(option);
    ctx.SstvDecoder?.SetMode(option.VisCode);
    ClearImage();
  }

  private void SstvPanel_FormClosing(object sender, FormClosingEventArgs e)
  {
    Log.Information("Closing SstvPanel");
    ctx.SstvPanel = null;
    ctx.MainForm.SstvDecoderMNU.Checked = false;
    modeCombo.SelectedIndexChanged -= ModeCombo_SelectedIndexChanged;
    historyListView.SelectedIndexChanged -= HistoryListView_SelectedIndexChanged;
    historyListView.DoubleClick -= HistoryListView_DoubleClick;
    ClearImage();
  }

  private void AutoStoreImage(Bitmap bitmap, bool isComplete)
  {
    try
    {
      var timestamp = DateTime.UtcNow;
      var statusLabel = isComplete ? "complete" : "partial";
      var fileName = $"sstv_{timestamp:yyyyMMdd_HHmmssfff}_{statusLabel}.png";
      var path = Path.Combine(storageFolder, fileName);

      using var clone = (Bitmap)bitmap.Clone();
      clone.Save(path, ImageFormat.Png);

      history.Insert(0, new StoredImageRecord(fileName, path, timestamp, isComplete));
      RefreshHistoryList();
      UpdateSaveButtonState();
    }
    catch (Exception ex)
    {
      Log.Error(ex, "Failed to store SSTV image");
    }
  }

  private void LoadHistory()
  {
    history.Clear();
    if (!Directory.Exists(storageFolder)) return;

    foreach (var file in Directory.EnumerateFiles(storageFolder, "*.png"))
    {
      try
      {
        var info = new FileInfo(file);
        var status = file.IndexOf("partial", StringComparison.OrdinalIgnoreCase) < 0;
        history.Add(new StoredImageRecord(info.Name, info.FullName, info.LastWriteTimeUtc, status));
      }
      catch (Exception ex)
      {
        Log.Warning(ex, "Failed to enumerate stored SSTV image {File}", file);
      }
    }

    history.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
    RefreshHistoryList();
    UpdateSaveButtonState();
  }

  private void RefreshHistoryList()
  {
    historyListView.BeginUpdate();
    historyListView.Items.Clear();

    foreach (var record in history)
    {
      var item = new ListViewItem(record.FileName)
      {
        Tag = record
      };
      item.SubItems.Add(record.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
      item.SubItems.Add(record.IsComplete ? "Complete" : "Partial");
      historyListView.Items.Add(item);
    }

    historyListView.EndUpdate();
  }

  private void UpdateSaveButtonState()
  {
    var hasSelection = historyListView.SelectedItems.Count > 0;
    saveButton.Enabled = hasSelection || currentImage != null;
  }

  private sealed record SstvModeOption(string DisplayName, int? VisCode);

  private sealed record StoredImageRecord(string FileName, string Path, DateTime Timestamp, bool IsComplete);

  private static class SstvVisCodes
  {
    public const int Martin1 = 0xAC;
    public const int Martin2 = 0x28;
    public const int Scottie1 = 0x3C;
    public const int Scottie2 = 0xB8;
    public const int ScottieDx = 0xCC;
    public const int Sc2_60 = 0xBB;
    public const int Sc2_120 = 0x3F;
    public const int Sc2_180 = 0xB7;
    public const int Robot24 = 0x84;
    public const int Robot36 = 0x88;
    public const int Robot72 = 0x0C;
    public const int P3 = 0x71;
    public const int P5 = 0x72;
    public const int P7 = 0xF3;
    public const int Bw8 = 0x82;
    public const int Bw12 = 0x86;
    public const int Pd50 = 0xDD;
    public const int Pd90 = 0x63;
    public const int Pd120 = 0x5F;
    public const int Pd160 = 0xE2;
    public const int Pd180 = 0x60;
    public const int Pd240 = 0xE1;
    public const int Pd290 = 0xDE;
    public const int Mp73 = 0x2523;
    public const int Mp115 = 0x2923;
    public const int Mp140 = 0x2A23;
    public const int Mp175 = 0x2C23;
    public const int Mr73 = 0x4523;
    public const int Mr90 = 0x4623;
    public const int Mr115 = 0x4923;
    public const int Mr140 = 0x4A23;
    public const int Mr175 = 0x4A23;
    public const int Ml180 = 0x8523;
    public const int Ml240 = 0x8623;
    public const int Ml280 = 0x8923;
    public const int Ml320 = 0x8A23;
    public const int Fax480 = 0x00;
    public const int Avt24 = 0xC0;
    public const int Avt90 = 0x44;
    public const int Avt94 = 0x48;
    public const int Mp73N = 0x5C256D;
    public const int Mp110N = 0x44456D;
    public const int Mp140N = 0x40556D;
    public const int Mc110N = 0x05456D;
    public const int Mc140N = 0x01556D;
    public const int Mc180N = 0x0D656D;
  }
}
