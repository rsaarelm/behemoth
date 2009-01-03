using System;
using System.Runtime.Serialization;

namespace Behemoth.Alg
{
  /// <summary>
  /// Global Unique IDentifier dispenser.
  /// </summary>
  [Serializable]
  public class Guid
  {
    public string Get(string prefix)
    {
      return prefix + "#" + counter++;
    }


    public string Get()
    {
      return Get("");
    }


    private long counter = 0;
  }
}