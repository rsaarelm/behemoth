using System.Collections.Generic;

namespace Behemoth.Alg
{
  /// <summary>
  /// Generic programming utilities.
  /// </summary>
  public static class Alg
  {
    /// <summary>
    /// Add the items of a collection into a list.
    /// </summary>
    public static void AddRange<T>(ICollection<T> range, IList<T> target)
    {
      foreach (T item in range)
      {
        target.Add(item);
      }
    }


    /// <summary>
    /// Concatenate a sequence of lists into a single existing list.
    /// </summary>
    public static IList<T> Concat<T>(ICollection<IList<T>> lists, IList<T> target)
    {
      foreach (ICollection<T> sub in lists)
      {
        AddRange(sub, target);
      }
      return target;
    }


    /// <summary>
    /// Concatenate a sequence of lists into a new list object.
    /// </summary>
    public static IList<T> Concat<T>(ICollection<IList<T>> lists)
    {
      return Concat(lists, new List<T>());
    }


    /// <summary>
    /// Create a list from the parameters. Useful for writing list literals.
    /// </summary>
    public static IList<T> L<T>(params T[] args)
    {
      List<T> result = new List<T>();
      result.AddRange(args);
      return result;
    }
  }
}