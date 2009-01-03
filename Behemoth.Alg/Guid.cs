using System;

namespace Behemoth.Alg
{
  /// <summary>
  /// Global Unique IDentifier dispenser.
  /// </summary>
  [Serializable]
  public class Guid
  {
    public string Next(string prefix)
    {
      return prefix + "#" + counter++;
    }


    public string Next()
    {
      return Next("");
    }


    private long counter = 0;
  }
}