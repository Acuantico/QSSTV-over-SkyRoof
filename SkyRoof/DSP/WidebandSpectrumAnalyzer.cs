using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using VE3NEA;

namespace SkyRoof
{
  internal class WidebandSpectrumAnalyzer : ThreadedProcessor<Complex32>
  {
    public float Median;
    public event EventHandler<DataEventArgs<float>>? SpectrumAvailable;

    internal Spectrum<Complex32>? Spectrum;


    internal WidebandSpectrumAnalyzer(int size, int step)
    {
      Spectrum = new(size, step, 4);
      Spectrum.SpectrumAvailable += Spectrum_SpectrumAvailable;
    }

    public override void Dispose()
    {
      base.Dispose();
      Spectrum?.Dispose();
      Spectrum = null;
    }

    protected override void Process(DataEventArgs<Complex32> args)    
    {
      Spectrum?.Process(args);
    }

    private void Spectrum_SpectrumAvailable(object? sender, DataEventArgs<float> e)
    {
      if (Spectrum == null) return;
      Median = FilterMedian(Spectrum.FastMedian);
      SpectrumAvailable?.Invoke(this, e);
    }

    float[] mdnBuf = new float[11];
    float[] mdnBuf2 = new float[11];
    int mdnIdx;
    float filt;

    private float FilterMedian(float mdn)
    {
      // eliminate short spikes
      if (++mdnIdx == mdnBuf.Length) mdnIdx = 0;
      mdnBuf[mdnIdx] = mdn;
      Array.Copy(mdnBuf, mdnBuf2, mdnBuf.Length);
      mdn = ArrayStatistics.PercentileInplace(mdnBuf2, 50);

      // smooth
      filt = 0.8f * filt + 0.2f * mdn;
      return filt;
    }
  }
}
