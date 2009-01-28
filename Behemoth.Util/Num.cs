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


    /// <summary>
    /// Noise between 1.0 and -1.0 from an integer seed.
    /// </summary>
    /// <remarks>
    /// From Hugo Elias, http://freespace.virgin.net/hugo.elias/models/m_perlin.htm
    /// </remarks>
    public static double Noise(int x)
    {
      x = (x << 13) ^ x;
      return (1.0 - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
    }


    public static double Noise(int x, int y)
    {
      return Noise(HashPoint(x, y));
    }


    public static double Noise(int x, int y, int z)
    {
      return Noise(HashPoint(x, y, z));
    }


    /// <summary>
    /// A modulo function that doesn't return negative numbers. Mod(k, n) is
    /// equivalent to ((k % n) + n) % n.
    /// </summary>
    public static int Mod(int k, int n)
    {
      return ((k % n) + n) % n;
    }

    public const double Epsilon = 0.000001;
  }
}