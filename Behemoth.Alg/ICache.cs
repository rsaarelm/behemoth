using System;

namespace Behemoth.Alg
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