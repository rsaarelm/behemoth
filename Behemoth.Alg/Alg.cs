using System.Collections.Generic;

namespace Behemoth.Alg
{
  /// <summary>
  /// Generic programming utilities.
  /// </summary>

  public static class Alg
  {
    // A static AddRange since IList doesn't have it as a method.
    public static void AddRange<T>(ICollection<T> range, IList<T> target)
    {
      foreach (T item in range)
      {
        target.Add(item);
      }
    }

    public static IList<T> Concat<T>(ICollection<IList<T>> lists, IList<T> target)
    {
      foreach (ICollection<T> sub in lists)
      {
        AddRange(sub, target);
      }
      return target;
    }

    public static IList<T> Concat<T>(ICollection<IList<T>> lists)
    {
      return Concat(lists, new List<T>());
    }

    public static IList<T> L<T>(params T[] args)
    {
      List<T> result = new List<T>();
      result.AddRange(args);
      return result;
    }
  }
}