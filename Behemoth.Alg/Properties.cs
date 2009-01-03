using System;
using System.Collections.Generic;

namespace Behemoth.Alg
{
  public class Properties<TKey, TValue>
  {
    /// <summary>
    /// Create a properties object without a parent.
    /// </summary>
    public Properties()
    {
    }


    /// <summary>
    /// Create a properties object with a parent.
    /// </summary>
    public Properties(Properties<TKey, TValue> parent)
    {
      this.parent = parent;
    }


    /// <summary>
    /// Query property presence. Properties not present in or hidden by the
    /// current object are recursively sought from the parent objects.
    /// </summary>
    public bool ContainsKey(TKey key)
    {
      if (hiddenKeys.Contains(key))
      {
        return false;
      }
      else if (data.ContainsKey(key))
      {
        return true;
      }
      else if (parent != null)
      {
        return parent.ContainsKey(key);
      }
      else
      {
        return false;
      }
    }


    /// <summary>
    /// Access property values. When reading, properties not present in or
    /// hidden by the current object are recursively sought from the parent
    /// objects.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// If key is null.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// If key isn't present.
    /// </exception>
    public TValue this[TKey key]
    {
      get
      {
        if (key == null)
        {
          throw new ArgumentNullException("key");
        }
        if (hiddenKeys.Contains(key))
        {
          throw new KeyNotFoundException(key.ToString());
        }
        else if (data.ContainsKey(key))
        {
          return data[key];
        }
        else if (parent != null)
        {
          return parent[key];
        }
        else
        {
          throw new KeyNotFoundException(key.ToString());
        }
      }
      set
      {
        if (hiddenKeys.Contains(key))
        {
          hiddenKeys.Remove(key);
        }
        data[key] = value;
      }
    }


    /// <summary>
    /// Set a key to be omitted from the current properties regardless of its
    /// presence in any of the parent objects.
    /// </summary>
    public void Hide(TKey key)
    {
      data.Remove(key);
      hiddenKeys.Add(key);
    }


    /// <summary>
    /// Remove the key from the current properties object, default to looking
    /// up the key from the parent.
    /// </summary>
    public void Unset(TKey key)
    {
      data.Remove(key);
      hiddenKeys.Remove(key);
    }


    public Properties<TKey, TValue> Parent
    {
      get { return parent; }
      set { parent = value; }
    }


    private Properties<TKey, TValue> parent = null;
    private IDictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();
    private HashSet<TKey> hiddenKeys = new HashSet<TKey>();
  }
}