using System;
using System.Collections.Generic;
using System.Diagnostics;

using Behemoth.Alg;

namespace Rpg
{
  public static class Action
  {
    /// <summary>
    /// Move an entity relative to its current position.
    /// </summary>
    /// <returns>
    /// Whether moving was successful.
    /// </returns>
    public static bool MoveRel(Entity e, Vec3 delta)
    {
      var newPos = Query.Pos(e) + delta;
      var core = e.Get<CCore>();
      var dir = Geom.VecToDir8(delta);

      // Hack: Since dirs are currently only used to flip the character icon
      // horizontally, do not impose a facing change when moving along the
      // vertical axis.

      // XXX: Remove this if facing gets any game-mechanical significance.
      if (!IsVerticalDir(dir))
      {
        core.Facing = (byte)dir;
      }

      // TODO: Check here if the entity is blocked by something.

      if (!Query.CanEnter(e, newPos))
      {
        return false;
      }

      Action.Place(e, newPos);
      return true;
    }


    /// <summary>
    /// The basic method for changing the position of an entity. All entity
    /// movement should use a Place action at the bottom.
    /// </summary>
    public static void Place(Entity e, Vec3 pos)
    {
      var core = e.Get<CCore>();

      // XXX: Might want to do some spatial index update stuff here.

      core.Pos = pos;
    }


    public static bool IsVerticalDir(int dir8)
    {
      return dir8 == 0 || dir8 == 4;
    }


    public static void Kill(Entity entity, Entity slayer)
    {
      var world = Query.WorldOf(entity);
      world.Remove(entity);
      CBrain brain;
      if (entity.TryGet(out brain))
      {
        if (brain.Gibs)
        {
          var gib = world.Spawn("gib");
          gib.Get<CCore>().Pos = entity.Get<CCore>().Pos;
          world.Add(gib);
        }
      }
    }


    public static void Attack(Entity entity, Entity target)
    {
      CBrain brain1, brain2;
      if (entity.TryGet(out brain1) && target.TryGet(out brain2))
      {
        brain2.Damage(entity, brain1.Might);
        // TODO: Attack message
      }
    }
  }
}