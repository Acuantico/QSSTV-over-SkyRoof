using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.Win32;

namespace SkyRoof
{
  internal class DpLayer : Dictionary<Bearing, List<(double cost, Bearing prev)>>;

  internal class DpTable : List<DpLayer>
  {
    private const double COST_TOLERANCE = 1e-2;
    private List<List<Bearing>> Results;

    public DpLayer Last => this[Count - 1];

    public DpTable(Trellis trellis)
    {
      int count = trellis.Count;
      for (int i = 0; i < count; i++) Add(new DpLayer());

      // initialize first layer
      foreach (var node in trellis[0]) this[0][node] = new() { (0, null) };

      // populate subsequent layers
      for (int i = 1; i < count; i++)
        foreach (var node in trellis[i])
        {
          var backLinks = new List<(double cost, Bearing prev)>();

          foreach (var prev in trellis[i - 1])
          {
            foreach (var (prevCost, _) in this[i - 1][prev])
            {
              double cost = prevCost + prev.RotationTime(node);
              backLinks.Add((cost, prev));
            }
          }

          int pathsToKeep = Math.Min(3, backLinks.Count);
          this[i][node] = backLinks.OrderBy(p => p.cost).Take(pathsToKeep).ToList();
        }
    }

    internal List<List<Bearing>> FindBestPaths()
    {
      Results = new List<List<Bearing>>();

      // Find minimum cost across all candidates in the last layer
      double bestCost = Last.Values.SelectMany(list => list.Select(x => x.cost)).Min();

      // Dictionary to store paths by layer
      var pathsByLayer = new Dictionary<int, List<(Bearing node, double cost, List<Bearing> path)>>();

      // Initialize with end nodes from the last layer
      pathsByLayer[Count - 1] = new List<(Bearing node, double cost, List<Bearing> path)>();

      foreach (var kv in Last)
        foreach (var (cost, prev) in kv.Value)
          if (cost - bestCost <= COST_TOLERANCE)
            pathsByLayer[Count - 1].Add((kv.Key, cost, new List<Bearing> { kv.Key }));

      // Process each layer from end to start
      for (int layerIndex = Count - 1; layerIndex > 0; layerIndex--)
      {
        var uniquePaths = new Dictionary<(Bearing firstHashCode, Bearing lastHashCode), (Bearing node, double cost, List<Bearing> path)>();

        foreach (var (node, nodeCost, path) in pathsByLayer[layerIndex])
        {
          foreach (var (cost, prev) in this[layerIndex][node])
          {
            if (cost - nodeCost <= COST_TOLERANCE)
            {
              var newPath = new List<Bearing>(path);
              newPath.Insert(0, prev);
              double prevCost = cost - node.RotationTime(prev);

              var firstBearing = newPath[0];
              var lastBearing = newPath[^1];
              
              var key = (firstBearing, lastBearing);

              if (!uniquePaths.TryGetValue(key, out var existing) || prevCost < existing.cost)
                uniquePaths[key] = (prev, prevCost, newPath);
            }
          }
        }

        pathsByLayer[layerIndex - 1] = uniquePaths.Values.ToList();

        // Keep only paths within tolerance of the best cost at this layer
        if (pathsByLayer[layerIndex - 1].Count > 0)
        {
          double layerBestCost = pathsByLayer[layerIndex - 1].Min(p => p.cost);
          pathsByLayer[layerIndex - 1] = pathsByLayer[layerIndex - 1]
            .Where(p => p.cost - layerBestCost <= COST_TOLERANCE)
            .ToList();
        }
      }

      // Extract complete paths
      if (pathsByLayer.ContainsKey(0) && pathsByLayer[0].Count > 0)
        Results = pathsByLayer[0].Select(p => p.path).ToList();

      return Results;
    }

    void Backtrack(int layer, Bearing node, Bearing prev, List<Bearing> path)
    {
      if (layer == 0)
      {
        // reached start: reverse path and store
        path.Reverse();
        Results.Add(new List<Bearing>(path)); // Create a copy
        return;
      }

      // Move to previous layer
      layer--;

      // Find matching entries in the previous layer
      foreach (var (cost, prevNode) in this[layer][prev])
      {
        // Create a new path for this branch
        var newPath = new List<Bearing>(path) { prev };
        Backtrack(layer, prev, prevNode, newPath);
      }
    }
  }
}