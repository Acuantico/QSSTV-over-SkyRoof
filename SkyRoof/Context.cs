using MathNet.Numerics;
using SkyRoof.Sstv;
using VE3NEA;

namespace SkyRoof
{
  public class Context
  {
    // resources
    internal readonly Font AwesomeFont8 = FontAwesomeFactory.Create(8);
    internal readonly Font AwesomeFont14 = FontAwesomeFactory.Create(14);
    internal readonly PaletteManager PaletteManager = new PaletteManager();

    // satellite data
    public Settings Settings = new();
    public SatnogsDb SatnogsDb;
    public GroupSatellitePasses GroupPasses;
    public HamSatellitePasses HamPasses;
    public SdrSatellitePasses SdrPasses;
    public AmsatStatusLoader AmsatStatusLoader = new();

    // main form
    public MainForm MainForm;
    public SatelliteSelectorWidget SatelliteSelector;
    public FrequencyWidget FrequencyControl;
    public RotatorWidget RotatorControl;

    // panels
    public GroupViewPanel? GroupViewPanel;
    public SatelliteDetailsPanel? SatelliteDetailsPanel;
    public PassesPanel? PassesPanel;
    public TimelinePanel? TimelinePanel;
    public SkyViewPanel? SkyViewPanel;
    public EarthViewPanel? EarthViewPanel;
    public TransmittersPanel? TransmittersPanel;
    public WaterfallPanel? WaterfallPanel;
    public QsoEntryPanel? QsoEntryPanel;
    public SstvPanel? SstvPanel;

    // devices
    public SoapySdrDevice? Sdr;
    public Slicer? Slicer;
  public CatControl CatControl = new();
  public SstvDecoderService? SstvDecoder;
  public bool SstvAutoMode = true;
  public int? SstvManualVisCode;

    // soundcards
    public readonly Soundcard<float> SpeakerSoundcard = new();
    public readonly Soundcard<float> AudioVacSoundcard = new();
    public readonly Soundcard<Complex32> IqVacSoundcard = new();
    public readonly Announcer Announcer = new();
    public UdpStreamSender UdpStreamSender = new();

    public void ClosePanels()
    {
      GroupViewPanel?.Close();
      SatelliteDetailsPanel?.Close();
      PassesPanel?.Close();
      TimelinePanel?.Close();
      SkyViewPanel?.Close();
      EarthViewPanel?.Close();
      TransmittersPanel?.Close();
      WaterfallPanel?.Close();
      QsoEntryPanel?.Close();
      SstvPanel?.Close();
    }
  }
}
