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