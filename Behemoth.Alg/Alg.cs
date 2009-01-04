using System;
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


    /// <summary>
    /// Create an array from the parameters. Useful for writing list literals.
    /// </summary>
    public static T[] A<T>(params T[] args)
    {
      return (T[])args.Clone();
    }


    /// <summary>
    /// Create a dictionary from the given argument list. Each pair of
    /// consequent elements is interpreted as one key-value pair. Useful for
    /// writing literals.
    /// </summary>
    public static IDictionary<K, V> Dict<K, V>(params Object[] args)
    {
      if (args.Length % 2 != 0)
      {
        throw new ArgumentException("Incomplete key-value pair in arguments.", "args");
      }

      IDictionary<K, V> result = new Dictionary<K, V>();
      for (int i = 0; i < args.Length / 2; i++)
      {
        result[(K)args[i * 2]] = (V)args[i * 2 + 1];
      }

      return result;
    }


    /// <summary>
    /// Return the index of the element for which the measure function returns
    /// the smallest value.
    /// </summary>
    /// <returns>
    /// The index of the value with the smallest measure, or -1 if the list of
    /// items is empty.
    /// </returns>
    public static int MinIndex<T, U>(IList<U> items, Func<U, T> measure)
      where T : IComparable<T>
    {
      if (items.Count == 0)
      {
        return -1;
      }

      int result = 0;
      T currentVal = measure(items[0]);

      for (int i = 1; i < items.Count; i++)
      {
        T newVal = measure(items[i]);
        if (newVal.CompareTo(currentVal) < 0) {
          currentVal = newVal;
          result = i;
        }
      }

      return result;
    }


    /// <summary>
    /// Clamp a value between two bounds.
    /// </summary>
    public static T Clamp<T>(T min, T val, T max)
      where T : IComparable<T>
    {
      if (min.CompareTo(val) > 0)
      {
        return min;
      }
      else if (val.CompareTo(max) > 0)
      {
        return max;
      }
      else
      {
        return val;
      }
    }


    /// <summary>
    /// Creates a type-checking predicate.
    /// </summary>
    public static Func<T, string> TypeP<T>(Type type)
    {
      return val => {
        Type valType = val.GetType();
        return valType == type ? null :
        "Expected type "+type+", got "+valType+".";
      };
    }


    /// <summary>
    /// Joins two constraint-style predicates into a new constraint which
    /// fails if either of the subconstraints fails.
    /// </summary>
    /// <remarks>
    /// The resulting predicate short-circuits if predicate lhs fails.
    /// </remarks>
    public static Func<T, string> Join<T>(Func<T, string> lhs, Func<T, string> rhs)
    {
      return val => {
        string ret;
        ret = lhs(val);
        if (ret != null) { return ret; }
        ret = rhs(val);
        if (ret != null) { return ret; }
        return null;
      };
    }
  }
}