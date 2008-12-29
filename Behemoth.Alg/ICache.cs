using System;

namespace Behemoth.Alg
{
  public interface ICache<T> : IDisposable
  {
    T this[string name]
    {
      get;
      set;
    }
    void Clear();
    long TotalSize { get; }
  }
}