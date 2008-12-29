using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Behemoth.Alg
{
  /// <summary>
  /// A cache for some type of loadable resource.
  /// </summary>
  public abstract class Cache<T> : IDisposable
  {
    /// <summary>
    /// Request an item from the cache. Load the item if it isn't cached yet.
    /// </summary>
    public T Get(string name)
    {
      if (disposed)
      {
        throw new ApplicationException("Trying to use a cache that has been disposed.");
      }

      if (!items.ContainsKey(name))
      {
        T item = Load(name);
        items[name] = item;
        totalSize += Size(item);
      }
      return items[name];
    }


    /// <summary>
    /// Manually add a new named item in the cache, freeing any existing item
    /// cached to the same name.
    /// </summary>
    public void Add(string name, T item)
    {
      if (disposed)
      {
        throw new ApplicationException("Trying to use a cache that has been disposed.");
      }

      if (items.ContainsKey(name))
      {
        totalSize -= Size(items[name]);
        Free(items[name]);
      }

      items[name] = item;
      totalSize += Size(item);
    }


    /// <summary>
    /// Clear the cache and free all cached items.
    /// </summary>
    public void Clear()
    {
      FreeAll();
      items.Clear();
      totalSize = 0;
    }


    /// <summary>
    /// The total size of the cached items.
    /// </summary>
    public long TotalSize { get { return totalSize; } }


    public void Dispose()
    {
      Dispose(true);
      // Drop this from the finalization queue since things should already be
      // cleaned up.
      GC.SuppressFinalize(this);
    }


    ~Cache()
    {
      Dispose(false);
    }


    protected virtual void Dispose(bool disposing)
    {
      disposed = true;
      // XXX: Is the item dictionary going to still be valid when finalizing?
      FreeAll();
    }


    private void FreeAll()
    {
      foreach (T item in items.Values)
      {
        Free(item);
      }
    }


    /// <summary>
    /// Load an item based on a name.
    /// </summary>
    protected abstract T Load(string name);

    /// <summary>
    /// Free a loaded item.
    /// </summary>
    protected abstract void Free(T item);

    /// <summary>
    /// Get the size of an item.
    /// </summary>
    /// <remarks>
    /// The default implementation returns 0 for all items. Leave it like that
    /// if you don't care about tracking cache size.
    /// </remarks>
    protected virtual long Size(T item)
    {
      return 0;
    }

    private IDictionary<string, T> items = new Dictionary<string, T>();

    private bool disposed = false;

    private long totalSize = 0;
  }
}