using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VE3NEA
{
  public static class Trig
  {
    public const float Pi = (float)Math.PI;
    public const float HalfPi = Pi / 2;
    public const float TwoPi = 2 * Pi;
    public const float DinR = 180 / Pi;
    public const float RinD = Pi / 180;

    public static double NormalizeTwoPi(double angle)
    {
      double result = angle % TwoPi;
      if (result < 0) result += TwoPi;
      return result;
    }
  }
}
