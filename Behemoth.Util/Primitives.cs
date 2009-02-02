using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Behemoth.Util
{
  public struct Plane
  {
    public Plane(Vec3 normal, double distance)
    {
      Debug.Assert(Num.AlmostEqual(normal.Abs(), 1.0));

      this.normal = normal;
      this.distance = distance;
    }


    public Vec3 Normal { get { return normal; } }


    public double Distance { get { return distance; } }


    private Vec3 normal;
    private double distance;
  }


  public struct Ray
  {
    public Ray(Vec3 origin, Vec3 dir)
    {
      Debug.Assert(Num.AlmostEqual(dir.Abs(), 1.0));

      this.origin = origin;
      this.dir = dir;
    }


    public Vec3 Origin { get { return origin; } }


    public Vec3 Dir { get { return dir; } }


    private Vec3 origin;
    private Vec3 dir;
  }
}