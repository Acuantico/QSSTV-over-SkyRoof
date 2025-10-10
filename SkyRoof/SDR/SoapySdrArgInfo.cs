using System.Runtime.InteropServices;

namespace VE3NEA
{
  public struct SoapySDRArgInfo
  {
    public string Key;
    public string Value;
    public string Name;
    public string Description;
    public string Unit;
    public NativeSoapySdr.SoapySDRArgInfoType Type;
    public NativeSoapySdr.SoapySDRRange Range;
    public Dictionary<string, string> Options;

    public static SoapySDRArgInfo FromNative(IntPtr ptr)
    {
      var result = new SoapySDRArgInfo();
      var nativeInfo = Marshal.PtrToStructure<NativeSoapySdr.SoapySDRArgInfo>(ptr);

      result.Key = Marshal.PtrToStringAnsi(nativeInfo.key);
      result.Value = Marshal.PtrToStringAnsi(nativeInfo.value);
      result.Name = Marshal.PtrToStringAnsi(nativeInfo.name);
      result.Description = Marshal.PtrToStringAnsi(nativeInfo.description);
      result.Unit = Marshal.PtrToStringAnsi(nativeInfo.units);
      result.Type = nativeInfo.type;
      result.Range = nativeInfo.range;
      result.Options = SoapySdrHelper.MarshalOptions(nativeInfo.optionNames, nativeInfo.options, nativeInfo.numOptions);

      NativeSoapySdr.SoapySDRArgInfo_clear(ptr);
      SoapySdr.CheckError();
      
      return result;
    }
  }
}
