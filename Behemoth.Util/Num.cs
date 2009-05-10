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


    public static double CosSpread(double x, double y)
    {
      double dist = Math.Min(1.0, Math.Sqrt(x * x + y * y));
      return Math.Cos(dist * Math.PI / 2);
    }


    /// <summary>
    /// Return whether two numbers are the same within epsilon.
    /// </summary>
    public static bool AlmostEqual(double a, double b)
    {
      return Math.Abs(a - b) <= Epsilon;
    }


    /// <summary>
    /// Function that returns -1, 0 or 1 based on the sign of the value.
    /// Signum(0) == 0.
    /// </summary>
    public static int Signum(double num)
    {
      return num < 0 ? -1 : (num > 0 ? 1 : 0);
    }


    /// <summary>
    /// Signum function for which SignumUnit(0) == 1.
    /// </summary>
    public static int SignumUnit(double num)
    {
      return num < 0 ? -1 : 1;
    }


    /// <summary>
    /// Smooth noise function for 3d space for Perlin noise etc functions.
    /// </summary>
    /// <remarks>
    /// From Ken Perlin's page, http://mrl.nyu.edu/~perlin/noise/
    /// </remarks>
    public static double SmoothNoise(double x, double y, double z)
    {
      // Get unit cube position in byte-3 space.
      int cubeX = (int)Math.Floor(x) & 0xff;
      int cubeY = (int)Math.Floor(y) & 0xff;
      int cubeZ = (int)Math.Floor(z) & 0xff;
      
      InUnitCube(ref x, ref y, ref z);
      double u = FadeCurve(x);
      double v = FadeCurve(y);
      double w = FadeCurve(z);

      // Corner hash coordinates.
      int a = noiseSeed[cubeX] + cubeY, aa = noiseSeed[a] + cubeZ,
        ab = noiseSeed[a + 1] + cubeZ, b = noiseSeed[cubeX + 1] + cubeY,
        ba = noiseSeed[b] + cubeZ, bb = noiseSeed[b + 1] + cubeZ;

      // Interpolate cube corners
      var edge1 = Lerp(
        PerlinGradient(noiseSeed[aa], x, y, z),
        PerlinGradient(noiseSeed[ba], x - 1, y, z),
        u);
      var edge2 = Lerp(
        PerlinGradient(noiseSeed[ab], x, y - 1, z),
        PerlinGradient(noiseSeed[bb], x - 1, y - 1, z),
        u);
      var edge3 = Lerp(
        PerlinGradient(noiseSeed[aa], x, y, z - 1),
        PerlinGradient(noiseSeed[ba], x - 1, y, z - 1),
        u);
      var edge4 = Lerp(
        PerlinGradient(noiseSeed[ab], x, y - 1, z - 1),
        PerlinGradient(noiseSeed[bb], x - 1, y - 1, z - 1),
        u);
      
      return Lerp(Lerp(edge1, edge2, v), Lerp(edge3, edge4, v), w);
    }


    /// <summary>
    /// From Perlin's noise function.
    /// </summary>
    private static double FadeCurve(double t)
    {
      return t * t * t * (t * (t * 6 - 15) + 10);
    }
    
    
    /// <summary>
    /// Return the gradient of a vector relative to an edge of the unit cube
    /// psedorandomly chosen based on the hash. Used by Perlin's smooth noise
    /// function.
    /// </summary>
    public static double PerlinGradient(int hash, double x, double y, double z)
    {
      int h = hash & 0xf;                      // CONVERT LO 4 BITS OF HASH CODE
      double u = h < 8 ? x : y,                 // INTO 12 GRADIENT DIRECTIONS.
             v = h < 4 ? y : h == 12 || h == 14 ? x : z;
      return ((h & 0x1) == 0 ? u : -u) + ((h & 0x2) == 0 ? v : -v);
    }
    

    /// <summary>
    /// Move x, y, z within an unit cube
    /// </summary>
    public static void InUnitCube(ref double x, ref double y, ref double z)
    {
      x -= Math.Floor(x);
      y -= Math.Floor(y);
      z -= Math.Floor(z);
    }
    

    public static double PerlinNoise(
      double persistence, int octaves, double x, double y, double z)
    {
      double result = 0.0;

      for (int i = 0; i < octaves; i++)
      {
        var freq = Math.Pow(2, i);
        var amp = Math.Pow(persistence, i);

        result += SmoothNoise(x * freq, y * freq, z * freq) * amp;
      }

      return result;
    }


    /// <summary>
    /// Probability density function for a normal distribution.
    /// </summary>
    public static double Gaussian(double x, double mean, double deviation)
    {
      return 1.0 / (deviation * Math.Sqrt(2 * Math.PI)) *
        Math.Exp(-Math.Pow(x - mean, 2) / (2 * deviation * deviation));
    }

    
    public static double CumulativeGaussian(double x, double mean, double deviation)
    {
      return 0.5 * (1.0 + Erf((x - mean) / (deviation * Sqrt2)));
    }


    /// <summary>
    /// Error function for cumulative normal distribution function. Use the
    /// approximation formula 7.1.26 from Abramowitz and Stegun: Handbook of
    /// Mathematical Functions
    /// </summary>
    public static double Erf(double x)
    {
      const double p = 0.3275911;
      const double a1 = 0.254829592;
      const double a2 = -0.284496736;
      const double a3 = 1.421413741;
      const double a4 = -1.453152027;
      const double a5 = 1.061405429;

      var sign = SignumUnit(x);
      x = Math.Abs(x);

      double t = 1 / (1 + p * x);
      double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t *
        Math.Exp(-x * x);
      return sign * y;
    }

    
    private static int[] noiseSeed = new int[512];
      
    
    static Num () 
    {
      // Init seed table for Perlin's noise function. Values from the source
      // code at Ken Perlin's page.
      int[] permutation = {
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,
        129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,
        49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
      };

      for (int i=0; i < 256; i++)
      {
        noiseSeed[256+i] = noiseSeed[i] = permutation[i];
      }
    }
    
    /// <summary>
    /// The official Very Small Number.
    /// </summary>
    public const double Epsilon = 0.000001;


    /// <summary>
    /// The square root of two.
    /// </summary>
    public const double Sqrt2 = 1.4142135623730951;
  }
}
