using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace SkyRoof.Sstv;

public sealed class SstvDecoderService : IDisposable
{
  private readonly object _syncRoot = new();
  private readonly IntPtr _handle;
  private readonly SynchronizationContext? _syncContext;
  private bool _disposed;

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
      int dstStride = bmpData.Stride;

      unsafe
      {
        fixed (byte* srcPtr = data)
        {
          byte* src = srcPtr;
          byte* dest = (byte*)bmpData.Scan0;
          for (int y = 0; y < height; y++)
          {
            Buffer.MemoryCopy(src, dest, dstStride, srcStride);
            src += srcStride;
            dest += dstStride;
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

