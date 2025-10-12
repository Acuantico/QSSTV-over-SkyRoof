using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using Serilog;

namespace SkyRoof.Sstv;

public sealed class SstvDecoderService : IDisposable
{
  private readonly object _syncRoot = new();
  private readonly IntPtr _handle;
  private readonly SynchronizationContext? _syncContext;
  private bool _disposed;
  private bool _loggedSample;

  public event EventHandler<SstvImageEventArgs>? ImageDecoded;

  public SstvDecoderService(double sampleRateHz)
  {
    _syncContext = SynchronizationContext.Current;
    _handle = SstvNative.skyroof_sstv_create(sampleRateHz);
    if (_handle == IntPtr.Zero)
      throw new InvalidOperationException("Failed to initialise SSTV decoder native context.");
  }

  public void Reset()
  {
    lock (_syncRoot)
    {
      ThrowIfDisposed();
      SstvNative.skyroof_sstv_reset(_handle);
    }
  }

  public void SetMode(int? visCode)
  {
    lock (_syncRoot)
    {
      ThrowIfDisposed();
      int code = visCode.GetValueOrDefault();
      bool auto = !visCode.HasValue;
      SstvNative.skyroof_sstv_set_mode(_handle, code, auto);
    }
  }

  public void Feed(float[] buffer, int count)
  {
    if (buffer == null) throw new ArgumentNullException(nameof(buffer));
    if (count <= 0) return;

    lock (_syncRoot)
    {
      ThrowIfDisposed();
      SstvNative.skyroof_sstv_feed(_handle, buffer, (nuint)count);
      TryEmitImage();
    }
  }

  private void TryEmitImage()
  {
    if (!SstvNative.skyroof_sstv_try_acquire_image(_handle, out var nativeImage))
      return;

    try
    {
      if (nativeImage.Data == IntPtr.Zero || nativeImage.Length == 0)
        return;

      int width = nativeImage.Width;
      int height = nativeImage.Height;
      if (width <= 0 || height <= 0)
        return;

      var managed = new byte[(int)nativeImage.Length];
      Marshal.Copy(nativeImage.Data, managed, 0, managed.Length);

      if (!_loggedSample && managed.Length >= 9)
      {
        var samplePoints = new List<(int X, int Y, string Label)>
        {
          (0, 0, "P0"),
          (Math.Min(20, width - 1), Math.Min(20, height - 1), "P1"),
          (Math.Min(width / 2, width - 1), Math.Min(height / 2, height - 1), "P2")
        };

        var sb = new System.Text.StringBuilder();
        sb.Append($"len={managed.Length} complete={nativeImage.Complete} ");
        foreach (var (x, y, label) in samplePoints)
        {
          int idx = (y * width + x) * 3;
          if (idx + 2 < managed.Length)
          {
            sb.Append($"{label}[{x},{y}]={managed[idx]}/{managed[idx + 1]}/{managed[idx + 2]} ");
          }
        }
        Log.Information("SSTV sample buffer {Info}", sb.ToString());
        _loggedSample = true;
      }

      var bitmap = CreateBitmap(width, height, managed);
      DispatchImage(bitmap, nativeImage.Complete);
    }
    finally
    {
      SstvNative.skyroof_sstv_release_image(_handle);
    }
  }

  private void DispatchImage(Bitmap bitmap, bool complete)
  {
    void Raise(object? _)
    {
      ImageDecoded?.Invoke(this, new SstvImageEventArgs(bitmap, complete));
    }

    if (_syncContext != null)
    {
      _syncContext.Post(Raise, null);
    }
    else
    {
      Raise(null);
    }
  }

  private static Bitmap CreateBitmap(int width, int height, byte[] data)
  {
    var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
    var rect = new Rectangle(0, 0, width, height);
    var bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

    try
    {
      int srcStride = width * 3;
      int dstStride = Math.Abs(bmpData.Stride);
      bool flip = bmpData.Stride < 0;

      unsafe
      {
        fixed (byte* srcPtr = data)
        {
          byte* srcBase = srcPtr;
          byte* scan0 = (byte*)bmpData.Scan0;

          for (int y = 0; y < height; y++)
          {
            byte* srcRow = srcBase + (y * srcStride);
            int destRowIndex = flip ? (height - 1 - y) : y;
            byte* destRow = scan0 + (destRowIndex * dstStride);

            for (int x = 0; x < width; x++)
            {
              int srcIndex = x * 3;
              int destIndex = x * 3;

              byte r = srcRow[srcIndex];
              byte g = srcRow[srcIndex + 1];
              byte b = srcRow[srcIndex + 2];

              destRow[destIndex] = b;
              destRow[destIndex + 1] = g;
              destRow[destIndex + 2] = r;
            }
          }
        }
      }
    }
    finally
    {
      bitmap.UnlockBits(bmpData);
    }

    return bitmap;
  }

  private void ThrowIfDisposed()
  {
    if (_disposed)
      throw new ObjectDisposedException(nameof(SstvDecoderService));
  }

  public void Dispose()
  {
    lock (_syncRoot)
    {
      if (_disposed) return;
      SstvNative.skyroof_sstv_destroy(_handle);
      _disposed = true;
    }
    GC.SuppressFinalize(this);
  }
}

public sealed class SstvImageEventArgs : EventArgs
{
  public SstvImageEventArgs(Bitmap image, bool complete)
  {
    Image = image ?? throw new ArgumentNullException(nameof(image));
    IsComplete = complete;
  }

  public Bitmap Image { get; }

  public bool IsComplete { get; }
}

