using System;
using System.Collections.Generic;

namespace Behemoth.Util
{
  public static class Num
  {
    /// <summary>
    /// Make a hash code for an integer point.
    /// </summary>
    public static int HashPoint(int x, int y)
    {
      const int arbitraryMagicPrime = 467;
      return x + y * arbitraryMagicPrime;
    }


    /// <summary>
    /// Make a hash code for an integer point.
    /// </summary>
    public static int HashPoint(int x, int y, int z)
    {
      const int arbitraryMagicPrime = 2293;
      return HashPoint(x, y) + z * arbitraryMagicPrime;
    }


    /// <summary>
    /// Make a hash code for an integer point.
    /// </summary>
    public static int HashPoint(int x, int y, int z, int w)
    {
      const int arbitraryMagicPrime = 6067;
      return HashPoint(x, y, z) + w * arbitraryMagicPrime;
    }


    /// <summary>
    /// Do linear interpolation between two values.
    /// </summary>
    public static double Lerp(double a, double b, double x)
    {
      return a + x * (b - a);
    }

  }
}