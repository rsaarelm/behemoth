using System;

namespace Behemoth.Util
{
  /// <summary>
  /// Utilities for operating on tile grids.
  /// </summary>
  public static class Tile
  {
    /// <summary>
    /// Call a function for each character in an ascii table with the char and
    /// coordinates of the character.
    /// </summary>
    public static void AsciiTableIter(Action<char, int, int> func, string[] lines)
    {
      for (int y = 0; y < lines.Length; y++)
      {
        for (int x = 0; x < lines[y].Length; x++)
        {
          func(lines[y][x], x, y);
        }
      }
    }


    /// <summary>
    /// Calculate the maximum dimensions of an ascii table.
    /// </summary>
    public static void AsciiTableDims(
      string[] lines, out int width, out int height)
    {
      height = lines.Length;
      if (lines.Length == 0)
      {
        width = 0;
      }
      else
      {
        width = lines[Alg.MinIndex(lines, str => -str.Length)].Length;
      }
    }


    /// <summary>
    /// Run a recursive shadowcasting line-of-sight algorithm. It is based on
    /// three delegates. All delegates use a coordinate system centered on LOS
    /// center, meaning that position (0, 0) is always where the LOS viewpoint
    /// is located.
    /// </summary>
    /// <param name="isBlockedPredicate">
    /// Query whether a tile blocks LOS in a coordinate system centered on LOS center.
    /// </param>
    /// <param name="markSeenAction">
    /// Called to mark a location as being visible.
    /// </param>
    /// <param name="outsideRadiusPredicate">
    /// Query whether a point is outside LOS radius. Should work fine when it
    /// describes any convex shape that touches its axis-aligned bounding box
    /// at the axes (axis-aligned square, circle, axis-aligned diamond). With
    /// stranger shapes, the field of view will probably only form a part of
    /// the shape due to the way the algorithm works.
    /// </param>
    public static void LineOfSight(
      Func<int, int, bool> isBlockedPredicate,
      Action<int, int> markSeenAction,
      Func<int, int, bool> outsideRadiusPredicate)
    {
      markSeenAction(0, 0);
      for (int octant = 0; octant < 8; octant++)
      {
        ProcessOctant(
          isBlockedPredicate, markSeenAction, outsideRadiusPredicate,
          octant, 0.0, 1.0, 1);
      }
    }


    /// <summary>
    /// Recursive raycasting line-of-sight worker function. Uses three
    /// delegates to interface with the actual map. The parameters startSlope
    /// and endSlope specify the sector to iterate. The parameter u is the
    /// distance from origin along the major axis of the octant.
    /// </summary>
    static void ProcessOctant(
      Func<int, int, bool> isBlockedPredicate,
      Action<int, int> markSeenAction,
      Func<int, int, bool> outsideRadiusPredicate,
      int octant,
      double startSlope,
      double endSlope,
      int u) {

      if (outsideRadiusPredicate(u, 0)) {
        return;
      }

      if (endSlope <= startSlope) {
        return;
      }

      bool traversingObstacle = true;

      for (int v = (int)Math.Round(u * startSlope),
            ev = (int)Math.Ceiling(u * endSlope);
          v <= ev;
          ++v) {
        int mapX, mapY;
        switch (octant) {
          case 0:
            mapX = v;
            mapY = -u;
            break;
          case 1:
            mapX = u;
            mapY = -v;
            break;
          case 2:
            mapX = u;
            mapY = v;
            break;
          case 3:
            mapX = v;
            mapY = u;
            break;
          case 4:
            mapX = -v;
            mapY = u;
            break;
          case 5:
            mapX = -u;
            mapY = v;
            break;
          case 6:
            mapX = -u;
            mapY = -v;
            break;
          case 7:
            mapX = -v;
            mapY = -u;
            break;
          default:
            // Shouldn't happen
            mapX = mapY = 0;
            break;
        }

        if (isBlockedPredicate(mapX, mapY)) {
          if (!traversingObstacle) {
            // Hit an obstacle
            ProcessOctant(
                isBlockedPredicate,
                markSeenAction,
                outsideRadiusPredicate,
                octant,
                startSlope,
                ((double)v - 0.5) / ((double)u + 0.5),
                u + 1);
            traversingObstacle = true;
          }
        } else {
          if (traversingObstacle) {
            traversingObstacle = false;
            if (v > 0) {
              // Risen above an obstacle
              startSlope = Math.Max(
                startSlope, ((double)v - 0.5) / ((double)u - 0.5));
            }
          }
        }

        // Not using u and v because the radius predicate might treat x and y
        // axes differently.
        if (startSlope < endSlope &&
          !outsideRadiusPredicate(mapX, mapY)) {
          markSeenAction(mapX, mapY);
        }
      }

      if (!traversingObstacle) {
        ProcessOctant(
            isBlockedPredicate,
            markSeenAction,
            outsideRadiusPredicate,
            octant,
            startSlope,
            endSlope,
            u + 1);
      }
    }
  }
}