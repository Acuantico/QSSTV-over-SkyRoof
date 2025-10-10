using System.Diagnostics;
using System.Security.Cryptography;
using CSCore.DSP;
using CSCore.Streams;
using MathNet.Filtering.IIR;
using MathNet.Numerics;
using SkyRoof;
using VE3NEA;

namespace SkyRoof
{
  public unsafe class SoftSquelch : IDisposable
  {
    private const int FILTER_DELAY = 766;

    private readonly OnlineIirFilter Hpf, Lpf;      
    private NativeLiquidDsp.firfilt_rrrf* Bpf;
    private float[] DelayLine = new float[FILTER_DELAY + 1024];
    //private float[] Buffer = new float[1024];

    public bool Enabled = true;

    public SoftSquelch()
    {
      // b, a = scipy.signal.iirfilter(5, 3500, fs=48000, btype="high", ftype="cheby2", rs=40)
      double[] coefficients = [
        0.39422113, -1.86812319, 3.63871459, -3.63871459, 1.86812319, -0.39422113,
        1, -2.97879203, 3.91647098, -2.74144198, 1.01000506, -0.15540778f];

      Hpf = new OnlineIirFilter(coefficients);

      // b, a = scipy.signal.iirfilter(3, Wn=10, fs=48000, btype="low", ftype="bessel")
      coefficients = [
        2.23580506e-09, 6.70741518e-09, 6.70741518e-09, 2.23580506e-09,
        1, -2.99363411, 2.9872851, -0.99365097f];

      Lpf = new OnlineIirFilter(coefficients);

      CreateBandpassFilter();
    }

    private const float BANDWIDTH = 2700;
    private const float CENTER_FREQ = 1650;
    private const int STOPBAND_REJECTION_DB = 80;
    private const int FILTER_LENGTH = 601;
    private void CreateBandpassFilter()
    {
      // create lowpass filter
      float cutoff = BANDWIDTH / 2 / SdrConst.AUDIO_SAMPLING_RATE;
      float centerFreq = CENTER_FREQ / SdrConst.AUDIO_SAMPLING_RATE;     
      var lowpassFilter = NativeLiquidDsp.firfilt_rrrf_create_kaiser(FILTER_LENGTH, cutoff, STOPBAND_REJECTION_DB, 0);

      // shift the frequency response to the center frequency
      var coeffs = NativeLiquidDsp.firfilt_rrrf_get_coefficients(lowpassFilter);
      Dsp.Mix(coeffs, FILTER_LENGTH, centerFreq);
      Bpf = NativeLiquidDsp.firfilt_rrrf_create(coeffs, FILTER_LENGTH);
      NativeLiquidDsp.firfilt_rrrf_destroy(lowpassFilter);
    }

    public unsafe void Process(float[] data)
    {
      if (!Enabled) return;

      // ensure buffer size
      int count = data.Length;
      //if (Buffer.Length < count) Array.Resize(ref Buffer, count);
      if (DelayLine.Length < FILTER_DELAY + count) Array.Resize(ref DelayLine, FILTER_DELAY + count);

      // push to delay line
      Array.Copy(data, 0, DelayLine, FILTER_DELAY, count);

      for (int i = 0; i < data.Length; i++)
      {
        // extract noise power above 3500 Hz
        double value = Hpf.ProcessSample(data[i]);
        value *= value;

        // compute smoothed amplitude
        value = Lpf.ProcessSample(value);
        value = Math.Sqrt(Math.Max(1e-6, value));

        // threshold
        if (value < 0.08) gain = 3; else if (value > 0.095) gain = 0.3f;

        // apply gain
        data[i] = DelayLine[i] * gain;
      }

      // low-pass filter the result
      fixed (float* pData = data) 
        NativeLiquidDsp.firfilt_rrrf_execute_block(Bpf, pData, (uint)count, pData);

      // dump old samples
      Array.Copy(DelayLine, count, DelayLine, 0, FILTER_DELAY);
    }
    float gain = 1;

    public void Dispose()
    {
      if(Bpf != null) NativeLiquidDsp.firfilt_rrrf_destroy(Bpf);
      Bpf = null;
    }
  }
}
