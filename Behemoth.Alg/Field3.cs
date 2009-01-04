using System;
using System.Collections.Generic;

namespace Behemoth.Alg
{
  [Serializable]
  public class Field3<T> : IField3<T>
  {
    // Implemented in terms of Field2.
    public T this[int x, int y, int z]
    {
      get
      {
        if (layers.ContainsKey(z))
        {
          return layers[z][x, y];
        }
        else
        {
          return default(T);
        }
      }

      set
      {
        if (!layers.ContainsKey(z))
        {
          layers[z] = new Field2<T>();
        }
        layers[z][x, y] = value;
      }
    }


    public void Clear()
    {
      layers.Clear();
    }


    public void Clear(int x, int y, int z)
    {
      if (layers.ContainsKey(z))
      {
        var layer = layers[z];
        layer.Clear(x, y);

        if (layer.IsEmpty)
        {
          // Remove emptied layers.
          layers.Remove(z);
        }
      }
    }


    /// <summary>
    /// The value that will be returned for cells that haven't been set.
    /// </summary>
    public T DefaultValue = default(T);


    private Dictionary<int, Field2<T>> layers = new Dictionary<int, Field2<T>>();
  }
}
