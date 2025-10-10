using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace VE3NEA
{
  public class AdifEntry : Dictionary<string, string>;

  public class AdifFile
  {
    private Regex? FieldRegdx;

    public string FieldsFilter = string.Empty;
    public AdifEntry Header = new();
    public List<AdifEntry> Qsos = new();

    public void Clear()
    {
      Header.Clear();
      Qsos.Clear();
    }


    //----------------------------------------------------------------------------------------------
    //                                       write
    //----------------------------------------------------------------------------------------------
    public void SaveToFile(string path)
    {
      var sb = new StringBuilder();
      sb.Append(HeaderToString(Header));
      foreach (var qso in Qsos) sb.Append(QsoToString(qso));
      File.WriteAllText(path, sb.ToString());
    }

    public void AppendToFile(string path, AdifEntry qso)
    {
      if (!File.Exists(path)) File.WriteAllText(path, HeaderToString(Header));
      File.AppendAllText(path, QsoToString(qso));
    }

    public static string HeaderToString(AdifEntry header)
    {
      string result = $"File generated {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}Z\n";
      return result + string.Join("\n", header.Select(kv => $"<{kv.Key}:{kv.Value.Length}>{kv.Value}")) + "\n<EOH>\n";
    }

    public static string QsoToString(AdifEntry header)
    {
      return string.Join("", header.Select(kv => $"<{kv.Key}:{kv.Value.Length}>{kv.Value}")) + "<EOR>\n";
    }




    //----------------------------------------------------------------------------------------------
    //                                       read
    //----------------------------------------------------------------------------------------------
    public void LoadFromFile(string path)
    {
      string adifString = File.ReadAllText(path);
      LoadFromString(adifString);
    }

    private void LoadFromString(string adifString)
    {
      BuildRegex();
      MatchCollection matches = FieldRegdx!.Matches(adifString);

      Clear();
      AdifEntry entry = new();

      foreach (Match match in matches)
        if (match.Groups[3].Value.ToLower() == "<eoh>") { Header = entry; entry = new(); }
        else if (match.Groups[3].Value.ToLower() == "<eor>") {  Qsos.Add(entry); entry = new(); }
        else
        {
          string fieldName = match.Groups[1].Value.Trim().ToUpper();
          int fieldLength = int.Parse(match.Groups[2].Value);
          entry[fieldName] = adifString.Substring(match.Index + match.Length, fieldLength);
        }
    }

    private void BuildRegex()
    {
      string filter = FieldsFilter.Trim();
      if (string.IsNullOrEmpty(filter)) filter = "[^:>]+"; // any field name
      string regexPattern = $"<({filter}):(\\d+)[^>]*>|(<eoh>|<eor>)";
      FieldRegdx = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
    }
  }
}