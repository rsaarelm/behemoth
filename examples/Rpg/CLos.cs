using System;

using Behemoth.Util;

namespace Rpg
{
  [Serializable]
  public class CLos : Component
  {
    public void DoLos()
    {
      visible.Clear();
      int radius = 24;

      var core = Entity.Get<CCore>();

      int centerX = (int)core.Pos.X;
      int centerY = (int)core.Pos.Y;
      int z = (int)core.Pos.Z;

      Func<int, int, bool> isBlockedPredicate = (x, y) =>
      {
        return !Query.CanSeeThrough(Entity, new Vec3(centerX + x, centerY + y, z));
      };

      Action<int, int> markSeenAction = (x, y) =>
      {
        visible[centerX + x, centerY + y, z] = true;
        mapped[centerX + x, centerY + y, z] = true;
      };

      Func<int, int, bool> outsideRadiusPredicate = (x, y) =>
      {
        return x * x + y * y >= radius * radius;
      };

      Tile.LineOfSight(
        isBlockedPredicate, markSeenAction, outsideRadiusPredicate);
    }


    public bool IsVisible(Vec3 pos)
    {
      return visible[pos];
    }


    public bool IsMapped(Vec3 pos)
    {
      return mapped[pos];
    }


    private Field3<bool> mapped = new Field3<bool>();
    private Field3<bool> visible = new Field3<bool>();

    new public static Type GetFamily()
    {
      return typeof(CLos);
    }
  }
}