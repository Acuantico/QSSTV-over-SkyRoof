using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyRoof
{
  [Flags] public enum SatelliteFlags
  {
    None = 0,
    
    Alive = 1,
    Future = 2,
    ReEntered = 4,

    Ham = 8,
    NonHam = 16,

    Vhf = 32,
    Uhf = 64,
    OtherBands = 128,

    Transmitter = 256,
    Transceiver = 512,
    Transponder = 1024,
  }
}
