using System;
using System.Collections.Generic;

using Behemoth.Alg;

namespace Rpg
{
  public static class Query
  {
    public static Vec3 Pos(Entity e)
    {
      var core = e.Get<CCore>();
      return core.Pos;
    }


    public static bool IsInRect(Entity e, int x, int y, int z, int w, int h)
    {
      CCore core;
      if (e.TryGet(out core))
      {
        return core.Pos.Z == z &&
          Geom.IsInRectangle(
            core.Pos.X, core.Pos.Y,
            x, y, w, h);
      }
      else
      {
        return false;
      }
    }


    public static World WorldOf(Entity e)
    {
      return e.Get<CCore>().World;
    }


    public static bool CanEnter(Entity e, Vec3 pos)
    {
      var world = WorldOf(e);

      var tile = world.Space[pos];

      // XXX: Assuming all entities are walking around and not able to phase
      // through walls.
      return !TerrainUtil.BlocksMoving(tile);
    }


    public static bool CanSeeThrough(Entity e, Vec3 pos)
    {
      var world = WorldOf(e);

      var tile = world.Space[pos];

      // X-Ray vision and other stuff here.
      return !TerrainUtil.BlocksSight(tile);
    }
  }
}