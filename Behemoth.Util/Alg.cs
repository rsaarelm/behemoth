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
      IDictionary<K, V> result = new Dictionary<K, V>();

      ApplyPairs<K, V>((k, v) => { result[k] = v; }, args);

      return result;
    }


    /// <summary>
    /// Give pairs of typed values from the vararg list to the action.
    /// </summary>
    public static void ApplyPairs<K, V>(Action<K, V> func, params Object[] args)
    {
      if (args.Length % 2 != 0)
      {
        throw new ArgumentException("Incomplete key-value pair in arguments.", "args");
      }

      for (int i = 0; i < args.Length / 2; i++)
      {
        func((K)args[i * 2], (V)args[i * 2 + 1]);
      }
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

      T result = default(T);
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


    /// <summary>
    /// Return whehter the IComparable lhs is smaller than the IComparable rhs.
    /// </summary>
    public static bool LessThan<T>(T lhs, T rhs) where T : IComparable<T>
    {
      return lhs.CompareTo(rhs) < 0;
    }


    public static bool ComparableEqual<T>(T lhs, T rhs) where T : IComparable<T>
    {
      return lhs.CompareTo(rhs) == 0;
    }


    /// <summary>
    /// Do a set operation, union, intersection or difference, on the values
    /// of two sorted sequences. The yieldMask value specifies the operation
    /// to perform. For each item encountered, a value mask, initially 0, is
    /// created. If the item is present in sequence 1, bit 0 of the value mask
    /// is set to 1. If the item is present in sequence 2, bit 1 of the value
    /// mask is set to 1. The resulting value mask can range from 0 to 3. The
    /// result is yielded if value (1 << mask) & yieldMask != 0.
    ///
    /// Example yieldMasks:
    /// union: 14
    /// intersection: 8
    /// difference: 2
    /// </summary>
    public static IEnumerable<T> GenericSortedSetOperation<T>(
      IEnumerable<T> sortedSeq1, IEnumerable<T> sortedSeq2, int yieldMask)
      where T : IComparable<T>
    {
      const int only1Mask = 1 << 1;
      const int only2Mask = 1 << 2;
      const int bothMask = 1 << 3;

      // Enumerators for the sequences.
      var en1 = sortedSeq1.GetEnumerator();
      var en2 = sortedSeq2.GetEnumerator();
      // Whether either sequence has items left.
      bool itemsLeft1 = en1.MoveNext();
      bool itemsLeft2 = en2.MoveNext();

      while (true)
      {
        if (itemsLeft1 && itemsLeft2)
        {
          var item1 = en1.Current;
          var item2 = en2.Current;
          if (ComparableEqual(item1, item2))
          {
            // The same item in both sequences.
            if ((yieldMask & bothMask) != 0)
            {
              yield return item1;
            }
            itemsLeft1 = en1.MoveNext();
            itemsLeft2 = en2.MoveNext();
          }
          else if (LessThan(item1, item2))
          {
            // The item only in the first sequence.
            if ((yieldMask & only1Mask) != 0)
            {
              yield return item1;
            }
            itemsLeft1 = en1.MoveNext();
          }
          else
          {
            // The item only in the second sequence.
            if ((yieldMask & only2Mask) != 0)
            {
              yield return item2;
            }
            itemsLeft2 = en2.MoveNext();
          }
        }
        else if (itemsLeft1)
        {
          // Only the first sequence has items.
          if ((yieldMask & only1Mask) != 0)
          {
            // If the mask cares about the first sequence without the second,
            // return values from it.
            yield return en1.Current;
            itemsLeft1 = en1.MoveNext();
          }
          else
          {
            // Otherwise there isn't anything more to do, exit the loop.
            break;
          }
        }
        else if (itemsLeft2)
        {
          // Only the second sequence has items.
          if ((yieldMask & only2Mask) != 0)
          {
            yield return en2.Current;
            itemsLeft2 = en2.MoveNext();
          }
          else
          {
            break;
          }
        }
        else
        {
          // Everything's gone.
          break;
        }
      }
    }


    /// <summary>
    /// Union of two sequences of sorted values.
    /// </summary>
    public static IEnumerable<T> SortedUnion<T>(
      IEnumerable<T> sortedSeq1,
      IEnumerable<T> sortedSeq2)
      where T : IComparable<T>
    {
      return GenericSortedSetOperation(sortedSeq1, sortedSeq2, 14);
    }


    /// <summary>
    /// Intersection of two sequences of sorted values.
    /// </summary>
    public static IEnumerable<T> SortedIntersection<T>(
      IEnumerable<T> sortedSeq1,
      IEnumerable<T> sortedSeq2)
      where T : IComparable<T>
    {
      return GenericSortedSetOperation(sortedSeq1, sortedSeq2, 8);
    }


    /// <summary>
    /// Yield the values in the first but not in the second sequence of sorted
    /// values.
    /// </summary>
    public static IEnumerable<T> SortedDifference<T>(
      IEnumerable<T> sortedSeq1,
      IEnumerable<T> sortedSeq2)
      where T : IComparable<T>
    {
      return GenericSortedSetOperation(sortedSeq1, sortedSeq2, 2);
    }
  }
}