using System;

namespace Behemoth.Alg
{
  /// <summary>
  /// 3-dimensional floating point vectors.
  /// </summary>
  [Serializable]
  public struct Vec3
  {
    public double X;
    public double Y;
    public double Z;


    public Vec3(double x, double y, double z)
    {
      X = x;
      Y = y;
      Z = z;
    }


    public Vec3(Vec3I vec)
    {
      X = vec.X;
      Y = vec.Y;
      Z = vec.Z;
    }


    public override bool Equals(Object obj)
    {
      return obj is Vec3 && this == (Vec3)obj;
    }


    public override int GetHashCode()
    {
      return Num.HashPoint(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
    }


    public static bool operator==(Vec3 lhs, Vec3 rhs)
    {
      return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
    }


    public static bool operator!=(Vec3 lhs, Vec3 rhs)
    {
      return !(lhs == rhs);
    }


    public static Vec3 operator+(Vec3 lhs, Vec3 rhs)
    {
      return new Vec3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
    }


    public static Vec3 operator-(Vec3 lhs, Vec3 rhs)
    {
      return new Vec3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
    }


    public override String ToString()
    {
      return String.Format("<{0}, {1}, {2}>", X, Y, Z);
    }
  }
}