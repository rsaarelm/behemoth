using System;
using System.Collections.Generic;

namespace Behemoth.Alg
{
  [Serializable]
  public class Field2<T> : IField2<T>
  {
    // This is a very simple and probably ineffective dictionary-based field
    // implementation.

    public T this[int x, int y]
    {
      get
      {
        if (rows.ContainsKey(y))
        {
          var row = rows[y];
          if (row.ContainsKey(x))
          {
            return row[x];
          }
        }
        return default(T);
      }

      set
      {
        if (!rows.ContainsKey(y))
        {
          rows[y] = new Dictionary<int, T>();
        }
        rows[y][x] = value;
      }
    }


    public void Clear()
    {
      rows.Clear();
    }


    public void Clear(int x, int y)
    {
      if (rows.ContainsKey(y))
      {
        var row = rows[y];
        row.Remove(x);

        if (row.Count == 0)
        {
          // Remove emptied rows.
          rows.Remove(y);
        }
      }
    }


    /// <summary>
    /// The value that will be returned for cells that haven't been set.
    /// </summary>
    public T DefaultValue = default(T);

    private Dictionary<int, Dictionary<int, T>> rows = new Dictionary<int, Dictionary<int, T>>();
  }
}