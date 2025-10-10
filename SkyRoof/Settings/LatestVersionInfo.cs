using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyRoof
{
  public class LatestVersionInfo
  {
    public bool Known { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public int Major { get; set; }
    public int Minor { get; set; }
  }
}
