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
    public static bool MoveRel(Entity e, Vec3I delta)
    {
      var newPos = Query.Pos(e) + delta;
      var core = e.Get<CoreComponent>();
      var dir = Geom.VecToDir8(new Vec3(delta));

      core.Facing = (byte)dir;

      // TODO: Check here if the entity is blocked by something.

      Action.Place(e, newPos);
      return true;
    }


    /// <summary>
    /// The basic method for changing the position of an entity. All entity
    /// movement should use a Place action at the bottom.
    /// </summary>
    public static void Place(Entity e, Vec3I pos)
    {
      var core = e.Get<CoreComponent>();

      // XXX: Might want to do some spatial index update stuff here.

      core.Pos = pos;
    }
  }
}