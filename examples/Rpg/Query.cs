using System;
using System.Collections.Generic;

using Behemoth.Util;

namespace Rpg
{
  /// <summary>
  /// The Query module is for all kinds of miscellaneous functions which
  /// determine things about the game world but do not alter it.
  /// </summary>
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

      foreach (var entity in WorldOf(e).EntitiesIn(pos))
      {
        if (entity.Get<CCore>().IsObstacle)
        {
          return false;
        }
      }

      // XXX: Assuming all entities are walking around and not able to phase
      // through walls.
      return !TerrainUtil.BlocksMoving(tile);
    }


    public static bool CanSpawnIn(EntityTemplate template, Vec3 pos)
    {
      var world = Rpg.Service.World;

      var tile = world.Space[pos];

      foreach (var entity in world.EntitiesIn(pos))
      {
        if (entity.Get<CCore>().IsObstacle)
        {
          return false;
        }
      }

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
      // TODO: Target stealth modifiers.

      var dist = Distance(observer, target);
      if (dist > cutoff)
      {
        return false;
      }

      var prob = 1 - Num.Sigmoid2(dist * sightDecay);

      var result = Rpg.Service.Rng.RandDouble() < prob;

      return result;
    }


    public static double SpawnProb(EntityTemplate template, double threatLevel)
    {
      double powerLevel = (double)template.Prop["powerLevel"];
      double rarity = (double)template.Prop["rarity"];

      double p = rarity;
      if (powerLevel <= threatLevel)
      {
        return p;
      }
      else
      {
        // Decay the probability for power levels beyond the threat range.
        // The formula is ad hoc and can probably use some tweaking.
        return p * (1 - Math.Atan(powerLevel - threatLevel) / (Math.PI / 2));
      }
    }


    /// <summary>
    /// Given a random number x and a threat level, return a suitable spawn
    /// from a set of templates.
    /// </summary>
    public static EntityTemplate ChooseSpawn(
      double x, double threatLevel, IEnumerable<EntityTemplate> templates)
    {
      // Only sample from templates with the required parameters.
      templates = Alg.Filter(
        t => t.Prop.ContainsKey("powerLevel") && t.Prop.ContainsKey("rarity"),
        templates);

      return Alg.WeightedChoice(
        x,
        Alg.Zip(Alg.Map<EntityTemplate, double>(
                  t => SpawnProb(t, threatLevel), templates), templates));
    }


    /// <summary>
    /// Convenience function that takes the random number.
    /// </summary>
    public static EntityTemplate ChooseSpawn(
      double threatLevel, IEnumerable<EntityTemplate> templates)
    {
      return ChooseSpawn(Rpg.Service.Rng.RandDouble(), threatLevel, templates);
    }


    /// <summary>
    /// Return whether an attempt with a certain skill succeeds agains a
    /// certain difficulty. The result is positive if the attempt succeeds,
    /// and its magnitude can be used to tell the magnitude of the success or
    /// the failure. The result is always between -1 and 1.
    ///
    /// The scale is exponential, +1 to skill or difficulty will chance the
    /// probability by the same amount for any pair of equal skill and
    /// difficulty.
    /// </summary>
    public static double Success(double skill, double difficulty)
    {
      double attack = Rpg.Service.Rng.RandDouble() * skill;
      double defense = Rpg.Service.Rng.RandDouble() * difficulty;
      var result = 4 * Math.Log(
        Math.Pow(2, attack / 4.0) - Math.Pow(2, defense / 4.0), 2);

      // Normalize to -1, 1.
      if (result < 0)
      {
        result /= difficulty;
      }
      else
      {
        result /= skill;
      }

      return result;
    }


    public static bool IsMapped(int x, int y, int z)
    {
      return Rpg.Service.Player.Get<CLos>().IsMapped(new Vec3(x, y, z));
    }
  }
}