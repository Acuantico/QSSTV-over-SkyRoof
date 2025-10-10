using System;
using System.Runtime.InteropServices;

namespace SkyRoof.Sstv;

internal static partial class SstvNative
{
  private const string LibraryName = "skyroof_sstv.dll";

  [StructLayout(LayoutKind.Sequential)]
  internal struct SkyroofSstvImage
  {
    public IntPtr Data;
    public nuint Length;
    public int Width;
    public int Height;
    [MarshalAs(UnmanagedType.I1)]
    public bool Complete;
  }

  [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
  internal static extern IntPtr skyroof_sstv_create(double sampleRate);

  [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
  internal static extern void skyroof_sstv_destroy(IntPtr ctx);

  [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
  internal static extern void skyroof_sstv_reset(IntPtr ctx);

  [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
  internal static extern void skyroof_sstv_feed(IntPtr ctx, float[] samples, nuint count);

  [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.I1)]
  internal static extern bool skyroof_sstv_try_acquire_image(IntPtr ctx, out SkyroofSstvImage image);

  [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
  internal static extern void skyroof_sstv_release_image(IntPtr ctx);

  [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
  internal static extern void skyroof_sstv_set_mode(IntPtr ctx, int visCode, [MarshalAs(UnmanagedType.I1)] bool autoMode);
}
