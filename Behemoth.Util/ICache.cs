using System;

namespace Behemoth.Util
{
  public interface ICache<K, V> : IDisposable
  {
    V this[K name]
    {
      get;
      set;
    }
    void Clear();
    long TotalSize { get; }
  }
}