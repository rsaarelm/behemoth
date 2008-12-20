
using System;
namespace Behemoth.Dummy
{

  /// <summary>
  /// An example Behemoth library class.
  /// </summary>
  public static class Dummy
  {
    public static void HelloWorld()
    {
      System.Console.WriteLine("Hello, world!");
      System.Console.WriteLine(""+Factorial(10));
    }

    /// <summary>
    /// The factorial function.
    /// </summary>
    public static int Factorial(int n)
    {
      // Use this to test unit testing.
      int result = 1;
      for (int i = 1; i <= n; i++) {
        result *= i;
      }
      return result;
    }

  }


}
