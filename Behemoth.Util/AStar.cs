using System;
using System.Collections.Generic;

namespace Behemoth.Util
{
  public static class AStar
  {
    public static IList<T> Search<T>(
      T startPos,
      T targetPos,
      Func<T, T, double> distance,
      Func<T, T, double> heuristic,
      Func<T, IEnumerable<T>> neighbors,
      int maxIterations)
    {
      var explored = new Set<T>();
      var edge = new Set<Cons<Tuple2<T, double>>>();

      edge.Add(Node(distance, null, startPos));

      while (maxIterations-- > 0)
      {
        if (edge.Count == 0)
        {
          // All paths are exhausted, search failed.
          return null;
        }

        // Find the most promising edge node.

        // Tried to use Alg.Minimum for this, but had mysterious data corruption bugs.

        // XXX: Edge container could be sorted according to node quality, then
        // you could just pick the first one.
        Cons<Tuple2<T, double>> nextNode = null;
        double bestCost = Double.MaxValue;
        foreach (var node in edge)
        {
          double cost = PathCost(node) + heuristic(HeadPos(node), targetPos);
          if (nextNode == null || cost < bestCost)
          {
            nextNode = node;
            bestCost = cost;
          }
        }

        // If this is the target node, return it, operation succesful.
        if (HeadPos(nextNode).Equals(targetPos))
        {
          return BuildPath(nextNode);
        }

        // Remove from edge, add to explored nodes.
        edge.Remove(nextNode);
        explored.Add(HeadPos(nextNode));

        // Add unexplored neighbors to the edge.
        foreach (var node in neighbors(HeadPos(nextNode)))
        {
          if (!explored.Contains(node))
          {
            edge.Add(Node(distance, nextNode, node));
          }
        }
      }

      // Ran out of iterations, search failed.
      return null;
    }


    private static T HeadPos<T>(Cons<Tuple2<T, double>> node)
    {
      return node.Head.First;
    }


    private static double PathCost<T>(Cons<Tuple2<T, double>> node)
    {
      return node.Head.Second;
    }


    private static IList<T> BuildPath<T>(Cons<Tuple2<T, double>> node)
    {
      var result = new List<T>();
      while (node != null)
      {
        result.Insert(0, HeadPos(node));
        node = node.Tail;
      }

      return result;
    }


    private static Cons<Tuple2<T, double>> Node<T>(
      Func<T, T, double> distance,
      Cons<Tuple2<T, double>> parent,
      T pos)
    {
      double dist = 0.0;

      if (parent != null)
      {
        dist = parent.Head.Second + distance(pos, parent.Head.First);
      }

      var result = new Cons<Tuple2<T, double>>(
        new Tuple2<T, double>(pos, dist), parent);

      return result;
    }
  }
}