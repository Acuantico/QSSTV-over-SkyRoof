using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGPdotNET.Observation;
using VE3NEA;

namespace SkyRoof
{
  internal class OptimizedRotationPath
  {
    public readonly SatellitePass? Pass;
    internal readonly SatnogsDbSatellite? Satellite;
    internal readonly List<Bearing> Path;

    public PathOptimizer Optimizer { get; }

    private readonly List<Bearing> OptimizedPath;
    private DateTime StartTime, EndTime;
    private readonly float StepSize; // Step size in degrees

    public OptimizedRotationPath(SatellitePass? pass, RotatorSettings settings, Bearing? currentDirection)
    {
      Pass = pass;
      Satellite = pass?.Satellite;
      if (Satellite == null) return;

      StartTime = pass!.StartTime > DateTime.UtcNow ? pass.StartTime : DateTime.UtcNow;
      EndTime = pass!.EndTime;
      StepSize = settings.StepSize; // Save step size

      var bearings = pass.Geostationary ? GenerateGeostationaryBearings(pass) : GenerateBearings(pass);
      if (bearings == null) return;

      var rect = ComputeFeasibleRect(settings);
      Optimizer = new PathOptimizer(rect, StepSize);
      OptimizedPath = Optimizer.OptimizePath(bearings, currentDirection);
    }

    private List<Bearing>? GenerateGeostationaryBearings(SatellitePass pass)
    {
      var aos = pass.GetObservationAt(pass.StartTime);
      var los = pass.GetObservationAt(pass.EndTime);
      if (aos == null || los == null) return new();

      return
        [
        new Bearing(aos.Azimuth.Radians, aos.Elevation.Radians),
        new Bearing(los.Azimuth.Radians, los.Elevation.Radians)
        ];
    }


    private List<Bearing>? GenerateBearings(SatellitePass pass)
    {
      double totalSeconds = (EndTime - StartTime).TotalSeconds;
      int intervals = Math.Max(1, (int)Math.Round(totalSeconds / 5.0));

      TimeSpan step = TimeSpan.FromSeconds(totalSeconds / intervals);
      TimeSpan epsilon = TimeSpan.FromMilliseconds(1);

      var bearings = new List<Bearing>();

      for (DateTime t = StartTime; t < EndTime + epsilon; t = t.Add(step))
      {
        var obs = pass.GetObservationAt(t);
        if (obs != null) bearings.Add(new Bearing(obs.Azimuth.Radians, obs.Elevation.Radians));
      }
      return bearings.Count > 0 ? bearings : null;
    }

    private static RectangleF ComputeFeasibleRect(RotatorSettings settings)
    {
      float minAzRad = (float)(settings.MinAzimuth * Trig.RinD);
      float minElRad = (float)(settings.MinElevation * Trig.RinD);
      float maxAzRad = (float)(settings.MaxAzimuth * Trig.RinD);
      float maxElRad = (float)(settings.MaxElevation * Trig.RinD);

      return new RectangleF(
        minAzRad,
        minElRad,
        maxAzRad - minAzRad,
        maxElRad - minElRad
      );
    }


    // return point maxerror/2 ahead of current point
    internal Bearing? GetNextAntennaBearing()
    {
      if (OptimizedPath == null || OptimizedPath.Count < 2)
        return null;

      // Get current satellite bearing
      var currentBearing = GetSatelliteBearing();
      if (currentBearing == null)
        return null;

      // Find current position in the path
      DateTime now = DateTime.UtcNow;
      var (lowerIndex, upperIndex, fraction) = GetPathSegment(now);

      // Start from the current segment
      double maxAngleError = (StepSize / 2.0) * Trig.RinD; // Convert to radians

      // Check subsequent segments until angle exceeds maxerror/2
      for (int i = upperIndex; i < OptimizedPath.Count; i++)
      {
        double angle = currentBearing.AngleFrom(OptimizedPath[i]);
        if (angle >= maxAngleError)
        {
          // Found the segment where angle exceeds maxerror/2
          // Interpolate to get exactly maxerror/2 ahead
          if (i > 0)
          {
            var b0 = OptimizedPath[i - 1];
            var b1 = OptimizedPath[i];
            double anglePrev = currentBearing.AngleFrom(b0);
            double angleFraction = (maxAngleError - anglePrev) / (angle - anglePrev);

            // Clamp fraction between 0 and 1
            angleFraction = Math.Max(0, Math.Min(1, angleFraction));

            double az = b0.Az + (b1.Az - b0.Az) * angleFraction;
            double el = b0.El + (b1.El - b0.El) * angleFraction;
            return new Bearing(az, el);
          }
          return OptimizedPath[i];
        }
      }

      // If we get here, no bearing exceeds maxerror/2, return the last point
      return OptimizedPath[OptimizedPath.Count - 1];
    }

    private (int lowerIndex, int upperIndex, double fraction) GetPathSegment(DateTime time)
    {
      double totalSeconds = (EndTime - StartTime).TotalSeconds;
      double elapsedSeconds = (time - StartTime).TotalSeconds;
      double passProgress = elapsedSeconds / totalSeconds;
      double position = passProgress * (OptimizedPath.Count - 1);
      int lowerIndex = (int)Math.Floor(position);
      int upperIndex = (int)Math.Ceiling(position);

      // Clamp indices to valid range
      lowerIndex = Math.Min(Math.Max(0, lowerIndex), OptimizedPath.Count - 1);
      upperIndex = Math.Min(Math.Max(0, upperIndex), OptimizedPath.Count - 1);

      // Calculate interpolation fraction
      double fraction = position - lowerIndex;
      return (lowerIndex, upperIndex, fraction);
    }

    // if the pass has not started, return AOS point
    // if the pass is in progress, return point interpolated for the current time
    // if the pass is over, return null
    internal Bearing? GetSatelliteBearing()
    {
      // todo: use Satellite to comute current bearing

      if (OptimizedPath == null) return null;

      DateTime now = DateTime.UtcNow;
      if (now < StartTime) return OptimizedPath.FirstOrDefault();
      if (now > EndTime) return null;

      var (lowerIndex, upperIndex, fraction) = GetPathSegment(now);

      // Interpolate between the two nearest bearings
      var b0 = OptimizedPath[lowerIndex];
      var b1 = OptimizedPath[upperIndex];
      double az = b0.Az + (b1.Az - b0.Az) * fraction;
      double el = b0.El + (b1.El - b0.El) * fraction;

      var bearing = new Bearing(az, el);
      return bearing.Normalize();
    }

    internal Bearing? GetRealSatelliteBearing()
    {
      var observation = Pass?.GetObservationAt(DateTime.UtcNow);
      if (observation == null) return null;
      return new Bearing(observation.Azimuth.Radians, observation.Elevation.Radians);
    }
  }
}
