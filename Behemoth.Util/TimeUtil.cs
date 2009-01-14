using System;

namespace Behemoth.Util
{
  public static class TimeUtil
  {
    public static double CurrentSeconds
    { get { return (double)DateTime.Now.Ticks / 1e7; } }
  }
}