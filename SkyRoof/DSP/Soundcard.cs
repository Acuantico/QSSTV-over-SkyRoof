using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

namespace VE3NEA
{
  public class Soundcard<T> : IDisposable
  {
    public class Entry
    {
      public string Id, Name;
      public Entry(string id, string name) { Id = id; Name = name; }
    }

    private WaveSource<T> waveSource;
    private MMDevice? mmDevice;
    private WasapiOut? wasapiOut;
    private bool enabled;
    private float volume;
    public event EventHandler? StateChanged;


    public bool Enabled { get => enabled; set => SetEnabled(value); }
    public float Volume { get => volume; set => SetVolume(value); }


    public Soundcard(string? audioDeviceId = null, int? samplingRate = null)
    {
      waveSource = new WaveSource<T>(samplingRate);
      SetDeviceId(audioDeviceId);
    }

    public void SetDeviceId(string? deviceId)
    {
      //if (deviceId == mmDevice?.DeviceID) return;

      bool wasEnabled = enabled;
      Enabled = false;
      if (deviceId == null) return;

      using (var deviceEnumerator = new MMDeviceEnumerator())
        try { mmDevice = deviceEnumerator.GetDevice(deviceId); } catch { return; }

      Enabled = wasEnabled;
    }


    private void SetEnabled(bool value)
    {
      if (value == Enabled) return;
      enabled = value;

      if (value) Start(); else Stop();
    }

    public bool IsPlaying()
    {
      return wasapiOut?.PlaybackState == PlaybackState.Playing;
    }

    private void Start()
    {
      if (mmDevice == null) return;

      try
      {
        waveSource.ClearBuffer();

        wasapiOut = new WasapiOut(false, AudioClientShareMode.Shared, 100);
        wasapiOut.Device = mmDevice;
        wasapiOut.Initialize(waveSource);
        wasapiOut.Volume = volume;
        wasapiOut.Stopped += WasapiOut_Stopped;
        wasapiOut.Play();

        StateChanged?.Invoke(this, EventArgs.Empty);
      }
      catch
      {
        Stop();
      }
    }

    private void Stop()
    {
      wasapiOut?.Stop();
      wasapiOut?.Dispose();
      wasapiOut = null;

      StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetVolume(float value)
    {
      volume = value;
      if (wasapiOut != null) wasapiOut.Volume = value;
    }

    private void WasapiOut_Stopped(object? sender, PlaybackStoppedEventArgs e)
    {
      StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
      Enabled = false;
    }

    public void AddSamples(T[] samples)
    {
      if (Enabled) waveSource.AddSamples(samples);
    }


    public static Entry[] ListDevices(DataFlow direction)
    {
      using (var deviceEnumerator = new MMDeviceEnumerator())
      using (var deviceCollection = deviceEnumerator.EnumAudioEndpoints(direction, DeviceState.Active))
        return deviceCollection.Select(s => new Entry(s.DeviceID, s.FriendlyName)).ToArray();
    }

    public static string? GetDefaultSoundcardId()
    {
      using (var deviceEnumerator = new MMDeviceEnumerator())
        return deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)?.DeviceID;
    }

    public static string? GetFirstVacId()
    {
      using (var deviceEnumerator = new MMDeviceEnumerator())
      using (var deviceCollection = deviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
        return deviceCollection.FirstOrDefault(d => d.FriendlyName.Contains("Virtual"))?.DeviceID;
    }

    internal string GetDisplayName()
    {
      try
      {
        return mmDevice?.FriendlyName ?? "None Selected";
      }
      catch
      {
        // when the device is disconnected, we are not notified about this,
        // but, but some internal variable in MMDevice becomes null
        // then a call to FriendlyName throws an exception
        return "Device Failed";
      }
    }
  }
}