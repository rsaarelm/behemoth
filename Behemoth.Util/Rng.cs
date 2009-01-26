using System;
using System.Text;

namespace Behemoth.Util
{
  /// <summary>
  /// Random number generator class.
  /// </summary>
  [Serializable]
  public abstract class Rng
  {
    public Rng()
    {
      Init(DateTime.Now.Ticks);
    }


    public int RandInt(int max)
    {
      return Next() % max;
    }


    public bool CoinFlip()
    {
      return Next() % 2 == 0;
    }


    public double RandDouble()
    {
      return (double)Next() / (double)int.MaxValue;
    }


    public double RandDouble(double min, double max)
    {
      return min + RandDouble() * (max - min);
    }


    /// <summary>
    /// Make a random unit vector.
    /// </summary>
    /// <remarks>
    /// Samples points in [-1, -1, -1] - [1, 1, 1] until one that's neither at
    /// the origin or outside the unit sphere is found. Returns this vector
    /// normalized to unit length.
    /// </remarks>
    public Vec3 UnitVec()
    {
      double len;
      Vec3 result;
      do
      {
        result = new Vec3(
          RandDouble(-1.0, 1.0),
          RandDouble(-1.0, 1.0),
          RandDouble(-1.0, 1.0));
        len = result.Abs();
      } while (len < Num.Epsilon || len > 1.0);

      return result.Unit();
    }


    public bool OneChanceIn(int num)
    {
      return RandInt(num) == 0;
    }


    public void Init(long seed)
    {
      Init(BitConverter.GetBytes(seed));
    }


    public void Init(string seed)
    {
      Init(Encoding.UTF8.GetBytes(seed));
    }


    /// <summary>
    /// Initialize the rng with the given byte array as seed.
    /// </summary>
    public abstract void Init(byte[] seed);

    /// <summary>
    /// Return a pseudorandom integer between 0 and int.MaxValue.
    /// </summary>
    protected abstract int Next();
  }
}