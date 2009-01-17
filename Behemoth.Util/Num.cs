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


    /// <summary>
    /// Sigmoid function P(t) = 1 / (1 + e^-t). Maps (-inf, inf) into (0, 1).
    /// </summary>
    public static double Sigmoid(double x)
    {
      return 1 / (1 + Math.Exp(-x));
    }


    /// <summary>
    /// Scaled sigmoid function. Maps (-inf, inf) into (-1, 1).
    /// </summary>
    public static double Sigmoid2(double x)
    {
      return 2.0 * (Sigmoid(x) - 0.5);
    }

  }
}