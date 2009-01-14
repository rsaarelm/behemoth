using System;
using System.Collections.Generic;

namespace Behemoth.Util
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


    // Implemented in terms of Field2.
    public T this[Vec3 pos]
    {
      get { return this[(int)pos.X, (int)pos.Y, (int)pos.Z]; }
      set { this[(int)pos.X, (int)pos.Y, (int)pos.Z] = value; }
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
    /// Iterate through all the points defined in the field.
    /// </summary>
    public IEnumerable<Tuple3<int, int, int>> Points
    {
      get
      {
        foreach (int z in layers.Keys)
        {
          foreach (var pt in layers[z].Points)
          {
            yield return new Tuple3<int, int, int>(pt.First, pt.Second, z);
          }
        }
      }
    }


    public bool IsEmpty { get { return layers.Count == 0; } }



    /// <summary>
    /// The value that will be returned for cells that haven't been set.
    /// </summary>
    public T DefaultValue = default(T);


    private Dictionary<int, Field2<T>> layers = new Dictionary<int, Field2<T>>();
  }
}
