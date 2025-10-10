namespace VE3NEA
{
  public static class Geo
  {
    public const double DinR = 180 / Math.PI;
    public const double RinD = 1 / DinR;
    public const double EarthRadiusKm = 6378; // also km per radian
    public const double RinKm = 1 / EarthRadiusKm;
    public const double KmInR = 1 / RinKm;
    public const double HalfPi = Math.PI / 2;
    public const double TwoPi = 2 * Math.PI;
    public const double EquatorLengthKm = TwoPi * EarthRadiusKm;
  }

  public class GridSquare
  {
    public static GeoPoint ToGeoPoint(string square)
    {
      if (!IsValid(square))
        throw new Exception($"Invalid grid square: '{square}'");

      if (square.Length > 6) square = square.Substring(0, 6);
      square = square.ToUpper();
      GeoPoint point = new GeoPoint();

      point.Longitude = -180 + (square[0] - 'A') * 20;
      point.Latitude = -90 + (square[1] - 'A') * 10;

      if (square.Length > 3)
      {
        point.Longitude += 2 * (square[2] - '0');
        point.Latitude += square[3] - '0';
      }

      if (square.Length > 5)
      {
        point.Longitude += 2 / 24d * (square[4] - 'A');
        point.Latitude += 1 / 24d * (square[5] - 'A');
      }

      // corner to center
      double mid = 5;
      if (square.Length == 4) mid /= 10; else if (square.Length == 6) mid /= 240;
      point.Latitude += mid;
      point.Longitude += 2 * mid;

      return point;
    }

    public static string FromGeoPoint(GeoPoint point, int length)
    {
      double x = (point.Longitude + 180) / 20;
      double y = (point.Latitude + 90) / 10;
      string square = $"{(char)('A' + (int)x)}{(char)('A' + (int)y)}";

      if (length > 2)
      {
        x = 10 * (x - (int)x);
        y = 10 * (y - (int)y);
        square += $"{(char)('0' + (int)x)}{(char)('0' + (int)y)}";
      }
      if (length > 4)
      {
        x = 24 * (x - (int)x);
        y = 24 * (y - (int)y);
        square += $"{(char)('a' + (int)x)}{(char)('a' + (int)y)}";
      }

      return square;
    }

    public static bool IsValid(string square)
    {
      if (square == "RR73") return false;

      if (square.Length != 2 && square.Length != 4 && square.Length != 6) return false;
      square = square.ToUpper();

      bool ok = square[0] >= 'A' && square[0] <= 'R' && square[1] >= 'A' && square[1] <= 'R';
      if (square.Length == 2) return ok;

      ok = ok && square[2] >= '0' && square[2] <= '9' && square[3] >= '0' && square[3] <= '9';
      if (square.Length == 4) return ok;

      return ok && square[4] >= 'A' && square[4] <= 'X' && square[5] >= 'A' && square[5] <= 'X';
    }

    internal static string? ForceCase(string? square)
    {
      switch (square?.Length)
      {
        case 4: return square.ToUpper();
        case 6: return square.Substring(0, 4).ToUpper() + square.Substring(4, 2).ToLower();
        default: return square;
      }
    }
  }

  public struct GeoPath
  {
    public double AzimuthRad, DistanceRad;


    public GeoPath(double azimuth, double distance) { AzimuthRad = DistanceRad = 0; Azimuth = azimuth; Distance = distance; }
    
    public double Azimuth { get => AzimuthRad * Geo.DinR; set => AzimuthRad = value * Geo.RinD; }
    public double Distance { get => DistanceRad * Geo.KmInR; set => DistanceRad = value * Geo.RinKm; }

    public double LongPathAzimuth { get => 360 - Azimuth; }
    public double LongPathDistance { get => Geo.EquatorLengthKm - Distance; set => DistanceRad = value * Geo.RinKm; }

    public override string ToString()
    {
      return $"{Azimuth:F0}º  {Distance:F0} km  (LP {LongPathAzimuth:F0}º  {LongPathDistance:F0} km)";
    }
  }

  public struct GeoPoint
  {
    public static GeoPoint NorthPole = new(90, 0);
    public static GeoPoint SouthPole = new(-90, 0);

    internal double LatitudeRad, LongitudeRad;

    public double Latitude { get => LatitudeRad * Geo.DinR; set => LatitudeRad = value * Geo.RinD; }
    public double Longitude { get => LongitudeRad * Geo.DinR; set => LongitudeRad = value * Geo.RinD; }


    public GeoPoint(double latitude, double longitude)
    {
      LatitudeRad = latitude * Geo.RinD;
      LongitudeRad = longitude * Geo.RinD;
    }

    public string ToGridSquare(int length)
    {
      return GridSquare.FromGeoPoint(this, length);
    }

    public GeoPoint MoveBy(GeoPath path)
    {
      GeoPoint point = new GeoPoint();

      double a = Math.Sin(LatitudeRad) * Math.Cos(path.DistanceRad)
        + Math.Cos(LatitudeRad) * Math.Sin(path.DistanceRad) * Math.Cos(path.AzimuthRad);
      a = Math.Max(-1, Math.Min(1, a));
      point.LatitudeRad = Math.Asin(a);

      double num = Math.Sin(path.AzimuthRad) * Math.Sin(path.DistanceRad) * Math.Cos(LatitudeRad);
      double den = Math.Cos(path.DistanceRad) - Math.Sin(LatitudeRad) * a;
      point.LongitudeRad = LongitudeRad + Math.Atan2(num, den);

      if (point.LongitudeRad < -Math.PI) point.LongitudeRad += Geo.TwoPi;
      else if (point.LongitudeRad >= Math.PI) point.LongitudeRad -= Geo.TwoPi;

      return point;
    }

    private GeoPath PathTo(GeoPoint point)
    {
      GeoPath path = new GeoPath();

      double s1 = Math.Sin(LatitudeRad);
      double c1 = Math.Cos(LatitudeRad);
      double s2 = Math.Sin(point.LatitudeRad);
      double c2 = Math.Cos(point.LatitudeRad);
      double sL = Math.Sin(point.LongitudeRad - LongitudeRad);
      double cL = Math.Cos(point.LongitudeRad - LongitudeRad);

      path.DistanceRad = Math.Acos(s1 * s2 + c1 * c2 * cL);

      path.AzimuthRad = Math.Atan2(sL * c2, c1 * s2 - s1 * c2 * cL);

      if (path.AzimuthRad < 0) path.AzimuthRad += Geo.TwoPi;
      else if (path.AzimuthRad > Geo.TwoPi) path.AzimuthRad -= Geo.TwoPi;

      return path;
    }

    public override string ToString()
    {
      string ns = LatitudeRad > 0 ? "N" : "S";
      string ew = LongitudeRad > 0 ? "E" : "W";
      return $"{Math.Abs(Latitude):F2} {ns}  {Math.Abs(Longitude):F2} {ew}";
    }

    public static GeoPoint operator +(GeoPoint point, GeoPath path) => point.MoveBy(path);
    public static GeoPath operator -(GeoPoint p2, GeoPoint p1) => p1.PathTo(p2);

    // exact comparison
    public static bool operator ==(GeoPoint p2, GeoPoint p1) => p2.LatitudeRad == p1.LatitudeRad && p2.LongitudeRad == p1.LongitudeRad;
    public static bool operator !=(GeoPoint p2, GeoPoint p1) => p2.LatitudeRad != p1.LatitudeRad || p2.LongitudeRad != p1.LongitudeRad;
  }

  public class Bearing
  {
    public double Azimuth { get; set; }
    public double Elevation { get; set; }
    public Bearing(double azimuth, double elevation)
    {
      Azimuth = azimuth;
      Elevation = elevation;
    }
    public static bool operator ==(Bearing? b2, Bearing? b1) => b2?.Azimuth == b1?.Azimuth && b2?.Elevation == b1?.Elevation;
    public static bool operator !=(Bearing? b2, Bearing? b1) => b2?.Azimuth != b1?.Azimuth || b2?.Elevation != b1?.Elevation;
    public static double AngleBetween(Bearing b1, Bearing b2)
    {
      // haversine formula
      double se = Math.Sin((b2.Elevation - b1.Elevation) * Geo.RinD / 2);
      double ce = Math.Cos(b1.Elevation * Geo.RinD) * Math.Cos(b2.Elevation * Geo.RinD);
      double sa = Math.Sin((b2.Azimuth - b1.Azimuth) * Geo.RinD / 2);
      double a = se * se + ce * sa * sa;
      return Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)) * 2 * Geo.DinR;
    }

    public static double AzimuthDifference(Bearing b1, Bearing b2)
    {
      double diff = Math.Abs(b1.Azimuth - b2.Azimuth);
      if (diff > 180) diff = 360 - diff;
      return diff;
    }
  }
}
