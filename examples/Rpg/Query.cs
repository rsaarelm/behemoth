using System;
using System.Collections.Generic;

using Behemoth.Alg;

namespace Rpg
{
  public static class Query
  {
    public static Vec3 Pos(Entity e)
    {
      var core = e.Get<CoreComponent>();
      return core.Pos;
    }


    public static bool IsInRect(Entity e, int x, int y, int z, int w, int h)
    {
      CoreComponent core;
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
  }
}