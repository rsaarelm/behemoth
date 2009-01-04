using System;

namespace Behemoth.Alg
{
  /// <summary>
  /// 3-dimensional integer vectors.
  /// </summary>
  [Serializable]
  public struct Vec3I
  {
    public int X;
    public int Y;
    public int Z;


    public Vec3I(int x, int y, int z)
    {
      X = x;
      Y = y;
      Z = z;
    }


    public override bool Equals(Object obj)
    {
      return obj is Vec3I && this == (Vec3I)obj;
    }


    public override int GetHashCode()
    {
      return Num.HashPoint(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
    }


    public static bool operator==(Vec3I lhs, Vec3I rhs)
    {
      return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
    }


    public static bool operator!=(Vec3I lhs, Vec3I rhs)
    {
      return !(lhs == rhs);
    }
  }
}