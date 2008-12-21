using System;

using Behemoth.Alg;

namespace DummyApp
{
  public class DummyApp
  {
    public static void Main()
    {
      Console.WriteLine("An example program.");
      var list = Alg.L(2, 3, 5, 7, 11, 13, 17);
      foreach (var i in list)
      {
        Console.WriteLine(i);
      }
    }
  }
}