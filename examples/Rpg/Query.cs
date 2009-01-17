using System;
using System.Collections.Generic;

using Behemoth.Util;

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


    public static bool HostileTo(Entity e, Entity target)
    {
      CBrain brain1;
      CBrain brain2;
      if (e.TryGet(out brain1))
      {
        if (target.TryGet(out brain2))
        {
          return brain1.Alignment != brain2.Alignment;
        }
      }
      return false;
    }


    public static IEnumerable<Entity> NearbyEnemies(Entity e)
    {
      var world = WorldOf(e);
      foreach (var target in world.EntitiesNear(e.Get<CCore>().Pos))
      {
        if (HostileTo(e, target))
        {
          yield return target;
        }
      }
    }


    /// <summary>
    /// Return whether an entity exists in the world.
    /// </summary>
    public static bool IsAlive(Entity e)
    {
      return e != null && WorldOf(e) != null;
    }


    public static double Distance(Entity e1, Entity e2)
    {
      var pos1 = e1.Get<CCore>().Pos;
      var pos2 = e2.Get<CCore>().Pos;

      if (pos1.Z != pos2.Z)
      {
        return Double.MaxValue;
      }
      else
      {
        return ((pos2 - pos1).Abs());
      }
    }


    /// <summary>
    /// Return whether an entity can become aware of a target entity.
    /// </summary>
    public static bool Notices(Entity observer, Entity target)
    {
      const double cutoff = 100.0;
      const double sightDecay = 0.3;
      // TODO: Check line of sight.
      // TODO: Observer state modifiers.
      // TODO: Targer stealth modifiers.

      var dist = Distance(observer, target);
      if (dist > cutoff)
      {
        return false;
      }

      var prob = 1 - Num.Sigmoid2(dist * sightDecay);

      var result = Rpg.Service.Rng.RandDouble() < prob;

      /*
      UI.Msg("{0} is looking for a target, {1} chance to spot... {2}",
             observer.Get<CCore>().Name,
             prob,
             result ? "Saw it!" : "Didn't see it.");
      */

      return result;
    }
  }
}