using System.Drawing.Printing;
using SGPdotNET.Observation;
using SGPdotNET.TLE;
using SGPdotNET.Util;
using VE3NEA;
using static SkyRoof.SatellitePass;

namespace SkyRoof
{
  public class SatellitePass
  {
    public class TrackPoint
    {
      public DateTime Utc;
      public TopocentricObservation? Observation;
    }

    private const double TwoPi = 2 * Math.PI;
    private readonly TimeSpan TrackStep = TimeSpan.FromSeconds(10);

    private GroundStation GroundStation;
    public SatnogsDbSatellite Satellite;

    public DateTime StartTime, CulminationTime, EndTime;
    public double MaxElevation;
    public int OrbitNumber;
    public bool Geostationary { get; private set; }
    private SatelliteVisibilityPeriod SatelliteVisibilityPeriod;

    public PointF[]? MiniPath;
    private PointF[] hump;
    private List<TrackPoint> track;
    public List<TrackPoint> Track { get => track ?? MakeTrack(); }



    public SatellitePass(GroundStation groundStation, SatnogsDbSatellite satellite, SatelliteVisibilityPeriod visibilityPeriod)
    {
      GroundStation = groundStation;
      Satellite = satellite;
      SatelliteVisibilityPeriod = visibilityPeriod;
      StartTime = visibilityPeriod.Start;
      CulminationTime = visibilityPeriod.MaxElevationTime;
      EndTime = visibilityPeriod.End;
      MaxElevation = visibilityPeriod.MaxElevation.Degrees;
      OrbitNumber = ComputeOrbitNumber();
      Geostationary = satellite.Tracker.IsGeoStationary();
    }






    //----------------------------------------------------------------------------------------------
    //                                         get
    //----------------------------------------------------------------------------------------------

    internal TopocentricObservation? GetObservationAt(DateTime utc)
    {
      return Satellite.Tracker.Observe(GroundStation, utc);
    }

    internal bool IsActive()
    {
      return StartTime < DateTime.UtcNow && EndTime > DateTime.UtcNow;
    }

    internal string[] GetTooltipText(bool showSeconds = true)
    {
      if (Geostationary) return [$"Geostationary   orbit #{OrbitNumber}", "", "", "", "", ""];

      var now = DateTime.UtcNow;
      var obs1 = GetObservationAt(now);
      var obs2 = GetObservationAt(now.AddMinutes(1));
      bool down = obs1 == null || obs2 == null || obs1.Elevation > obs2.Elevation;
      string upDpwn = down ? "↓" : "↑";
  
      var aos = GetObservationAt(StartTime);
      var los = GetObservationAt(EndTime);

      string[] tooltip = new string[6];

      if (EndTime < DateTime.UtcNow) tooltip[0] = "Ended.";
      else if (StartTime < DateTime.UtcNow) tooltip[0] = $"LOS ↓  in {Utils.TimespanToString(EndTime - DateTime.UtcNow, showSeconds)}";
      else tooltip[0] = $"AOS ↑  in {Utils.TimespanToString(StartTime - DateTime.UtcNow, showSeconds)}";

      tooltip[1] = $"AOS azimuth {aos?.Azimuth.Degrees:F0}º at {StartTime:yyyy-MM-dd HH:mm:ss}";
      tooltip[2] = $"LOS azimuth {los?.Azimuth.Degrees:F0}º at {EndTime:yyyy-MM-dd HH:mm:ss}";
      tooltip[3] = $"Duration: {Utils.TimespanToString(EndTime - StartTime, false)}";
      tooltip[4] = $"Elevation: {obs1?.Elevation.Degrees:F0}º {upDpwn}  (Max {MaxElevation:F0}º at {CulminationTime:HH:mm})";
      tooltip[5] = $"Orbit: #{OrbitNumber}";

      return tooltip;
    }




    //----------------------------------------------------------------------------------------------
    //                                        compute
    //----------------------------------------------------------------------------------------------
    private int ComputeOrbitNumber()
    {
      if (!Satellite.Tracker.Enabled) return 0;
      Tle tle = Satellite.Tracker.Tle!;

      uint revNum = tle.OrbitNumber;
      var timeSinceOrbit = (SatelliteVisibilityPeriod.MaxElevationTime - tle.Epoch).TotalDays;
      var revPerDay = tle.MeanMotionRevPerDay;
      var drag = tle.BStarDragTerm;
      var meanAnomaly = tle.MeanAnomaly.Radians;
      var argumentPerigee = tle.ArgumentPerigee.Radians;

      return (int)(
        revNum +
        Math.Floor((meanAnomaly + argumentPerigee) / TwoPi)
        + (revPerDay + drag * timeSinceOrbit) * timeSinceOrbit);

      // (int)((revPerDay + drag * timeSinceOrbit) * timeSinceOrbit + (meanAnomaly + argumentPerigee) / TwoPi)
      // - (int)((meanAnomaly + argumentPerigee) / TwoPi)
      // + (int)revNum;
    }

    public PointF[] ComputeHump(float x0, float y0, float scaleX, float scaleY)
    {
      for (int i = 0; i < Track.Count; i++)
        hump[i] = new(
          x0 + (float)(Track[i].Utc - StartTime).TotalMinutes * scaleX, 
          y0 - (float)Track[i].Observation.Elevation.Degrees * scaleY);

      return hump;
    }

    private List<TrackPoint> MakeTrack()
    {
      int stepCount = 63;
      if (Geostationary) stepCount = 3;
      TimeSpan step = (EndTime - StartTime) / stepCount;
      if (step < TimeSpan.FromSeconds(10)) step = TimeSpan.FromSeconds(10);
      stepCount = (int)Math.Round((EndTime - StartTime) / step);

      track = new();
      
      for (int i=0; i <= stepCount; i++)
        {
          var trackPoint = new TrackPoint();
          trackPoint.Utc = StartTime + i * step;
          trackPoint.Observation = GetObservationAt(trackPoint.Utc);
          track.Add(trackPoint);
        }

      hump = new PointF[track.Count];

      return track.Where(p => p != null).ToList();
    }


    private const int StepCount = 10;
    public void MakeMiniPath()
    {
      if (MiniPath != null) return;

      var duration = EndTime - StartTime;
      var step = duration / StepCount;
      MiniPath = new PointF[StepCount + 1];

      for (int i = 0; i <= StepCount; i++)
      {
        var utc = StartTime + step * i;
        var observation = Satellite.Tracker.Observe(GroundStation, utc);
        if (observation == null) continue;

        double ro = 1 - observation.Elevation.Radians / Utils.HalfPi;
        double phi = Utils.HalfPi - observation.Azimuth.Radians;

        MiniPath[i] = new PointF((float)(ro * Math.Cos(phi)), (float)(ro * Math.Sin(phi)));
      }
    }

    // currently not used
    public List<GeoPath> GetCoveragePolygon()
    {
      // only for LEO
      if (EndTime - StartTime > TimeSpan.FromHours(2)) return new();

      var leftPoints = new List<GeoPoint>();
      var rightPoints = new List<GeoPoint>();

      var predictions = Track.Select(t => Satellite.Tracker.Predict(t.Utc)).Where(p=>p != null).ToArray();
      if (predictions.Length < 3) return new List<GeoPath>();

      for (int i = 0; i < predictions.Length; i++)
      {
        var prevP = predictions[Math.Max(0, i-1)];
        var P = predictions[i];
        var nextP = predictions[Math.Min(predictions.Length-1, i+1)];

        var prevG = new GeoPoint(prevP.Latitude.Degrees, prevP.Longitude.Degrees);
        var G = new GeoPoint(P.Latitude.Degrees, P.Longitude.Degrees);
        var nextG = new GeoPoint(nextP.Latitude.Degrees, nextP.Longitude.Degrees);

        var azim = (nextG - prevG).Azimuth;
        var radius = P.GetFootprint();

        var pointL = G + new GeoPath(azim - 90, radius);
        var pointR = G + new GeoPath(azim + 90, radius);

        if (i == 0)
          for (double a = 1; a < 180; a+=1)
            leftPoints.Add(G + new GeoPath(azim + 90 + a, radius));

        leftPoints.Add(pointL);
        rightPoints.Add(pointR);

        if (i == predictions.Length-1)
          for (double a = 1; a < 180; a+=1)
            leftPoints.Add(G + new GeoPath(azim - 90 + a, radius));
      }

      rightPoints.Reverse();
      leftPoints.AddRange(rightPoints);
      
      // close the loop
      leftPoints.Add(leftPoints[0]);

      // lat/lon to az/dist
      var homeP = GroundStation.Location.ToGeodetic();
      var homeG = new GeoPoint(homeP.Latitude.Degrees, homeP.Longitude.Degrees);
      return leftPoints.Select(g => g - homeG).ToList();
    }
  }
}
