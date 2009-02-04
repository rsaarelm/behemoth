using System;
using System.Collections.Generic;

namespace Behemoth.Util
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
    /// Create an array from the parameters that can be of any type. Useful
    /// for writing list literals.
    /// </summary>
    public static Object[] OA(params Object[] args)
    {
      return (Object[])args.Clone();
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


    public static T Minimum<T>(ICollection<T> items, Func<T, double> measure)
    {
      if (items.Count == 0)
      {
        // Item might not be nullable, no default value to return.
        throw new ArgumentException("Empty collection", "items");
      }

      Console.WriteLine("Processing collection of size {0}.", items.Count);

      T result;
      double currentVal = double.MaxValue;

      foreach (var node in items)
      {
        Console.WriteLine("Minimum of elements... "+node);
      }


      foreach (var item in items)
      {
        Console.WriteLine("Considering item: "+item);
        double newVal = measure(item);
        Console.WriteLine("Measure taken: "+newVal);
        if (newVal < currentVal) {
          currentVal = newVal;
          result = item;
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
    public static Func<T, string> Both<T>(Func<T, string> lhs, Func<T, string> rhs)
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


    /// <summary>
    /// Joins two constraint-style predicates into a new constraint which
    /// fails if both of the subconstraints fails.
    /// </summary>
    /// <remarks>
    /// The resulting predicate short-circuits if predicate lhs succeeds.
    /// </remarks>
    public static Func<T, string> Either<T>(Func<T, string> lhs, Func<T, string> rhs)
    {
      return val => {
        string ret1 = lhs(val);
        if (ret1 == null) { return null; }
        string ret2 = rhs(val);
        if (ret2 == null) { return null; }
        // XXX: Arbitrarily omitting ret2 from result if both ret1 and ret2
        // return an error message.
        return ret1 != null ? ret1 : ret2;
      };

    }


    /// <summary>
    /// Make a predicate that checks a whether a given array is of the same
    /// size as the parameter predicate list and whether each element matches
    /// the corresponding parameter predicate.
    /// </summary>
    public static Func<T[], string> ArrayP<T>(
      params Func<T, string>[] predicates)
    {
      return val => {
        if (val == null)
        {
          return "Null array.";
        }
        if (val.Length != predicates.Length)
        {
          return String.Format(
            "Expected array length {0}, got {1}.",
            predicates.Length,
            val.Length);
        }
        for (int i = 0; i < predicates.Length; i++)
        {
          string ret = predicates[i](val[i]);
          if (ret != null)
          {
            return String.Format("Error on element {0}: {1}", i, ret);
          }
        }
        return null;
      };
    }


    public static void Iter2(int x0, int x1, int y0, int y1, Action<int, int> fn)
    {
      for (int y = y0; y < y1; y++)
      {
        for (int x = x0; x < x1; x++)
        {
          fn(x, y);
        }
      }
    }
  }
}