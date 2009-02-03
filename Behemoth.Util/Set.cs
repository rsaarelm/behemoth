using System;
using System.Collections;
using System.Collections.Generic;

namespace Behemoth.Util
{
  public class Set<T> : ICollection<T>
  {
    public Set() {}


    public Set(ICollection<T> seq)
    {
      Add(seq);
    }


    public void Add(T item)
    {
      members[item] = true;
    }


    public void Add(ICollection<T> seq)
    {
      foreach (T elt in seq)
      {
        Add(elt);
      }
    }


    public void Clear()
    {
      members.Clear();
    }


    public bool Contains(T item)
    {
      return members.ContainsKey(item);
    }


    public bool Remove(T item)
    {
      return members.Remove(item);
    }


    public void CopyTo(T[] array, int arrayIndex)
    {
      members.Keys.CopyTo(array, arrayIndex);
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator)members.Keys.GetEnumerator();
    }


    public IEnumerator<T> GetEnumerator()
    {
      return members.Keys.GetEnumerator();
    }


    public int Count { get { return members.Count; } }


    public bool IsReadOnly { get { return false; } }


    public static Set<T> Union(Set<T> lhs, Set<T> rhs)
    {
      var result = new Set<T>();

      foreach (var elt in lhs)
      {
        result.Add(elt);
      }

      foreach (var elt in rhs)
      {
        result.Add(elt);
      }

      return result;
    }


    public static Set<T> Intersection(Set<T> lhs, Set<T> rhs)
    {
      // XXX: Could be made to run in linear time if we restricted elements to
      // be orderable and used an ordered internal container.
      Set<T> smaller, larger;
      if (lhs.Count < rhs.Count)
      {
        smaller = lhs;
        larger = rhs;
      }
      else
      {
        smaller = rhs;
        larger = lhs;
      }

      var result = new Set<T>();

      foreach (var elt in smaller)
      {
        if (larger.Contains(elt))
        {
          result.Add(elt);
        }
      }

      return result;
    }


    public static Set<T> Difference(Set<T> lhs, Set<T> rhs)
    {
      var result = new Set<T>(lhs);

      foreach (var elt in rhs)
      {
        result.Remove(elt);
      }

      return result;
    }


    public static Set<T> operator+(Set<T> lhs, Set<T> rhs)
    {
      return Union(lhs, rhs);
    }


    public static Set<T> operator*(Set<T> lhs, Set<T> rhs)
    {
      return Intersection(lhs, rhs);
    }


    public static Set<T> operator-(Set<T> lhs, Set<T> rhs)
    {
      return Difference(lhs, rhs);
    }


    private IDictionary<T, bool> members = new Dictionary<T, bool>();
  }
}