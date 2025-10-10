using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SkyRoof
{
  public class PathOptimizer
  {
    internal readonly RectangleF FeasibleRect;
    internal readonly float MaxErrorDeg;
    internal Trellis? Trellis;
    internal DpTable? DpTable;
    internal List<List<Bearing>> BestPaths;
    internal Bearing? CurrentDirection;

    public PathOptimizer(RectangleF feasibleRect, float maxErrorDeg)
    {
      FeasibleRect = feasibleRect;
      MaxErrorDeg = maxErrorDeg;
    }

    public List<Bearing> OptimizePath(List<Bearing> rawPath, Bearing? currentDirection = null)
    {
      CurrentDirection = currentDirection;

      if (rawPath == null || rawPath.Count == 0) return new List<Bearing>();

      Trellis = new Trellis(rawPath, FeasibleRect, MaxErrorDeg);
      DpTable = new(Trellis);
      BestPaths = DpTable.FindBestPaths();

      // initial bearing is unknown, just return the best path
      if (currentDirection == null) return BestPaths[0];

      // find all best paths with similar cost, choose the one with the least initial rotation time      
      return BestPaths.OrderBy(path => currentDirection.RotationTime(path[0])).First();
    }  
  }
}