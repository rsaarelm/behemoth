using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Behemoth.Util
{
  /// <summary>
  /// A class for a general, inheritable set of properties.
  /// </summary>
  /// <seealso href="http://steve-yegge.blogspot.com/2008/10/universal-design-pattern.html">
  /// Steve Yegge: The Universal Design Pattern
  /// </seealso>
  [Serializable]
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
      Parent = parent;
    }


    /// <summary>
    /// Add a set of key-value pairs as flat parameter data.
    /// </summary>
    /// <returns>
    /// Reference to this object, so that the method can be used with a fluent
    /// interface idiom.
    /// </returns>
    public Properties<TKey, TValue> Add(params Object[] contents)
    {
      if (contents.Length % 2 != 0)
      {
        throw new ArgumentException(
          "List length not even; key not matched by value.",
          "contents");
      }
      for (int i = 0; i < contents.Length / 2; i++)
      {
        this[(TKey)contents[i * 2]] = (TValue)contents[i * 2 + 1];
      }

      return this;
    }


    /// <summary>
    /// Query property presence. Properties not present in or hidden by the
    /// current object are recursively sought from the parent objects.
    /// </summary>
    public bool ContainsKey(TKey key)
    {
      if (hiddenKeys.ContainsKey(key))
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
        return Get(key);
      }
      set
      {
        Set(key, value);
      }
    }


    /// <summary>
    /// Return the value for a key if the key is set, otherwise do nothing.
    /// </summary>
    /// <returns>
    /// Whether the key was found and the value was provided.
    /// </returns>
    public bool TryGet(TKey key, out TValue val)
    {
      if (ContainsKey(key))
      {
        val = this[key];
        return true;
      }
      else
      {
        return false;
      }
    }


    /// <summary>
    /// Set a key to be omitted from the current properties regardless of its
    /// presence in any of the parent objects.
    /// </summary>
    public void Hide(TKey key)
    {
      data.Remove(key);
      hiddenKeys[key] = true;
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


    public virtual Properties<TKey, TValue> Parent
    {
      get { return parent; }
      set { parent = value; }
    }


    public virtual TValue Get(TKey key)
    {
      if (key == null)
      {
        throw new ArgumentNullException("key");
      }
      if (hiddenKeys.ContainsKey(key))
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


    public virtual Properties<TKey, TValue> Set(TKey key, TValue val)
    {
      if (hiddenKeys.ContainsKey(key))
      {
        hiddenKeys.Remove(key);
      }
      data[key] = val;
      return this;
    }


    protected Properties<TKey, TValue> parent = null;

    private IDictionary<TKey, TValue> data = new Dictionary<TKey, TValue>();
    private IDictionary<TKey, bool> hiddenKeys = new Dictionary<TKey, bool>();
  }
}