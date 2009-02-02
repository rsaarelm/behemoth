using System;
using System.Diagnostics;

namespace Behemoth.Util
{
  /// <summary>
  /// Geometric utilities.
  /// </summary>
  public static class Geom
  {
    /// <description>
    /// Fit the largest centered (pixelWidth * scale, pixelHeight * scale)
    /// viewport with integer 'scale' into the given viewport. If the viewport
    /// is too small to for unit scale (pixelWidth, pixelHeight) viewport,
    /// scale the pixel viewport down into the largest non-integer fractional
    /// size that fits in the viewport.
    /// </description>
    public static void MakeScaledViewport(
      int pixelWidth, int pixelHeight,
      int viewportWidth, int viewportHeight,
      out int x, out int y, out int width, out int height)
    {
      Debug.Assert(
        pixelWidth > 0 && pixelHeight > 0 && viewportWidth > 0 && viewportHeight > 0,
        "Zero or negative viewport dimensions are not allowed.");
      double pixelAspect = (double)pixelWidth / (double)pixelHeight;
      double viewportAspect = (double)viewportWidth / (double)viewportHeight;
      int scale = Math.Min(viewportWidth / pixelWidth, viewportHeight / pixelHeight);
      if (scale == 0) {
        // Viewport is too small to do proper pixels, just fit the largest rect we can in it.
        if (viewportAspect > pixelAspect)
        {
          // Fit Y, adjust X
          width = (int)(pixelAspect * viewportHeight);
          x = viewportWidth / 2 - width / 2;
          height = viewportHeight;
          y = 0;
        }
        else
        {
          // Fit X, adjust Y
          height = (int)(viewportWidth / pixelAspect);
          y = viewportHeight / 2 - height / 2;
          width = viewportWidth;
          x = 0;
        }
      }
      else
      {
        width = scale * pixelWidth;
        height = scale * pixelHeight;

        x = viewportWidth / 2 - width / 2;
        y = viewportHeight / 2 - height / 2;
      }
    }


    public static bool RectanglesIntersect(
      double x1, double y1, double w1, double h1,
      double x2, double y2, double w2, double h2)
    {
      return !(x1 + w1 < x2 ||
               y1 + h1 < y2 ||
               x2 + w2 < x1 ||
               y2 + h2 < y1);
    }


    public static bool IsInRectangle(
      double x, double y,
      double rectX, double rectY, double rectW, double rectH)
    {
      return x >= rectX && y >= rectY &&
        x < rectX + rectW && y < rectY + rectH;
    }


    /// <summary>
    /// Determine the 1/16th sector of a circle a point in the XY plane points
    /// towards. Sector 0 is clockwise from line x = 0, and subsequent sectors
    /// are clockwise from there. The origin point is handled in the same way
    /// as in Math.Atan2.
    /// </summary>
    /// <remarks>
    /// Hexadecants can be used to get both quadrants and octants a vector is
    /// pointing to.
    /// </remarks>
    public static int Hexadecant(double x, double y)
    {
      const double hexadecantWidth = Math.PI / 8.0;

      double radian = Math.Atan2(x, y);

      radian = radian < 0 ? radian + 2.0 * Math.PI : radian;
      Debug.Assert(radian >= 0.0);

      return (int)Math.Floor(radian / hexadecantWidth);
    }


    public static bool IsDir8(int dir)
    {
      return dir >= 0 && dir < 8;
    }


    public static bool IsDir4(int dir)
    {
      return dir >= 0 && dir < 4;
    }


    /// <summary>
    /// Convert a 8-directional direction (clockwise, starting from (0, 1))
    /// into a vector.
    /// </summary>
    public static Vec3 Dir8ToVec(int dir8)
    {
      if (!IsDir8(dir8))
      {
        throw new ArgumentException("Not a dir8.", "dir8");
      }
      else
      {
        return dir8Vecs[dir8];
      }
    }


    /// <summary>
    /// Convert a 4-directional direction (clockwise, starting from (0, 1))
    /// into a vector.
    /// </summary>
    public static Vec3 Dir4ToVec(int dir4)
    {
      if (!IsDir4(dir4))
      {
        throw new ArgumentException("Not a dir4.", "dir4");
      }
      else
      {
        return dir4Vecs[dir4];
      }
    }


    private static readonly Vec3[] dir8Vecs = new Vec3[] {
      new Vec3( 0,  1,  0),
      new Vec3( 1,  1,  0),
      new Vec3( 1,  0,  0),
      new Vec3( 1, -1,  0),
      new Vec3( 0, -1,  0),
      new Vec3(-1, -1,  0),
      new Vec3(-1,  0,  0),
      new Vec3(-1,  1,  0),
    };


    private static readonly Vec3[] dir4Vecs = new Vec3[] {
      new Vec3( 0,  1,  0),
      new Vec3( 1,  0,  0),
      new Vec3( 0, -1,  0),
      new Vec3(-1,  0,  0),
    };


    /// <summary>
    /// Return the dir4 a vector's XY projection points to.
    /// </summary>
    public static int VecToDir4(Vec3 vec)
    {
      var result = ((Hexadecant(vec.X, vec.Y) + 2) % 16) / 4;

      return result;
    }


    /// <summary>
    /// Return the dir8 a vector's XY projection points to.
    /// </summary>
    public static int VecToDir8(Vec3 vec)
    {
      var result = ((Hexadecant(vec.X, vec.Y) + 1) % 16) / 2;

      return result;
    }


    /// <summary>
    /// If two vectors are on the same Z plane, return a dir8 pointing from
    /// the first to the second.
    /// </summary>
    public static bool PointTo(Vec3 origin, Vec3 target, out int dir8)
    {
      dir8 = -1;
      if (origin.Z == target.Z)
      {
        dir8 = VecToDir8(target - origin);
        return true;
      }
      else
      {
        return false;
      }
    }



    /// <summary>
    /// Generate an axis + angle rotation that aligns a vector with
    /// the given direction vector.
    /// </summary>
    public static void OrientTowards(Vec3 init, Vec3 dir, out Vec3 axis, out double angle)
    {
      axis = Vec3.Cross(init, dir).Unit();
      angle = Vec3.Angle(init, dir);
    }


    /// <summary>
    /// Convert radians to degrees.
    /// </summary>
    public static double Rad2Deg(double radian)
    {
      return radian * 180.0 / Math.PI;
    }


    /// <summary>
    /// Convert degrees to radians.
    /// </summary>
    public static double Deg2Rad(double degree)
    {
      return degree * Math.PI / 180.0;
    }


    /// <summary>
    /// Determine the point where a ray hits a plane if it does. Return
    /// whether the ray hits the plane or not.
    /// </summary>
    public static bool RayPlaneIntersection(
      Ray ray, Plane plane, out Vec3 intersection)
    {
      double rayPlaneDot = Vec3.Dot(ray.Dir, plane.Normal);

      if (Num.AlmostEqual(rayPlaneDot, 0.0))
      {
        // Ray is practically parallel to the plane.
        intersection = new Vec3(0, 0, 0);

        return false;
      }

      // How far along the ray is the intersection?
      double t = (plane.Distance - Vec3.Dot(plane.Normal, ray.Origin)) / rayPlaneDot;

      if (t >= 0.0) {
        intersection = ray.Origin + ray.Dir * t;
        return true;
      }
      else
      {
        intersection = new Vec3(0, 0, 0);

        return false;
      }
    }
  }
}