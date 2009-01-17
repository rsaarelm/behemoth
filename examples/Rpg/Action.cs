using System;
using System.Collections.Generic;
using System.Diagnostics;

using Behemoth.Apps;
using Behemoth.Util;

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
          UI.Msg("Splodey!");
          var gib = world.Spawn("gib");
          gib.Get<CCore>().Pos = entity.Get<CCore>().Pos;
          world.Add(gib);
        }
      }

      if (entity == Rpg.Service.Player)
      {
        Rpg.Service.GameOver(String.Format("Killed by {0}.", slayer.Get<CCore>().Name));
      }
    }


    public static void Attack(Entity entity, Entity target)
    {
      CBrain brain1, brain2;
      if (entity.TryGet(out brain1) && target.TryGet(out brain2))
      {
        UI.Msg("Whup!");
        brain2.Damage(entity, brain1.Might);
        // TODO: Attack message
      }
    }


    public static void AttackMove(Entity entity, int dir8)
    {
      var moveVec = Geom.Dir8ToVec(dir8);
      var targetPos = entity.Get<CCore>().Pos + moveVec;
      foreach (var e in Query.WorldOf(entity).EntitiesIn(targetPos))
      {
        if (Query.HostileTo(entity, e))
        {
          Action.Attack(entity, e);

          return;
        }
      }
      Action.MoveRel(entity, moveVec);

    }
  }
}