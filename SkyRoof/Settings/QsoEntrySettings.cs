using System.ComponentModel;
using System.Drawing.Design;

namespace SkyRoof
{
  [Flags]
  [Editor(typeof(STUP.ComponentModel.Design.FlagsEditor), typeof(UITypeEditor))]
  public enum QsoFields {
    UTC = 1, 
    BAND = 2,
    MODE = 4,
    SAT = 8,
    CALL = 16,
    GRID = 32,
    STATE = 64,
    SENT = 128,
    RECV = 256,
    NAME = 512, 
    NOTES = 1024,
  }

  public enum NewFileEvery { Day, Month, Year }

  public class QsoEntrySettings
  {
    private const QsoFields AllFields = 
      QsoFields.UTC | QsoFields.BAND | QsoFields.MODE | QsoFields.SAT | QsoFields.CALL | 
      QsoFields.GRID | QsoFields.SENT | QsoFields.RECV | QsoFields.NAME | QsoFields.NOTES;

    [DisplayName("Visible Fields")]
    [DefaultValue(AllFields)]
    public QsoFields Fields { get; set; } = AllFields;

    [DisplayName("New file every")]
    [Description("Create a new ADIF file every day, month, or year.")]
    [DefaultValue(NewFileEvery.Year)]
    public NewFileEvery NewFileEvery { get; set; } = NewFileEvery.Year;


    public override string ToString() { return string.Empty; }
  }
}
