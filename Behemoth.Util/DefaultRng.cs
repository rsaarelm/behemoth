using System;

namespace Behemoth.Util
{
  public class DefaultRng : Rng
  {
    public override void Init(byte[] seed)
    {
      seed = MemUtil.Pad(seed, 4);
      rng = new Random(BitConverter.ToInt32(seed, 0));
    }

    protected override int Next()
    {
      return rng.Next();
    }

    private Random rng = new Random();
  }
}