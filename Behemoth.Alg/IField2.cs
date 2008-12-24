namespace Behemoth.Alg
{
  /// <summary>
  /// A Field2 is an unbounded two-dimensional grid on which homogenous values
  /// can be placed.
  /// </summary>
  public interface IField2<T>
  {
    T this[int x, int y]
    {
      get;
      set;
    }

    void Clear();

    void Clear(int x, int y);
  }
}