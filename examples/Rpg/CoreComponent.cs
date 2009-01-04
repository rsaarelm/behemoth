using System;

using Behemoth.Alg;

namespace Rpg
{
  [Serializable]
  public class CoreComponent : Component
  {
    public Vec3I Pos;

    public void SetPos(int x, int y, int z)
    {
      Pos.X = x;
      Pos.Y = y;
      Pos.Z = z;
    }

    public int Icon;

    public String Name;

    public byte Facing;

    public World World;

    public bool ActionPose;

    new public static String GetFamily()
    {
      return "core";
    }
  }
}