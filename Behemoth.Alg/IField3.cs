namespace Behemoth.Alg
{
  /// <summary>
  /// A Field3 is an unbounded three-dimensional grid on which homogenous
  /// values can be placed.
  /// </summary>
  /// <typeparm name="T">
  /// The type of the field's cells.
  /// </typeparm>
  public interface IField3<T>
  {
    /// <summary>
    /// Access cells in the field.
    /// </summary>
    T this[int x, int y, int z]
    {
      get;
      set;
    }


    /// <summary>
    /// Access using a Vec3. The components of the vec are cast to integers.
    /// </summary>
    T this[Vec3 pos]
    {
      get;
      set;
    }


    /// <summary>
    /// Clear all values in the field.
    /// </summary>
    void Clear();

    /// <summary>
    /// Clear a specific cell in the field.
    /// </summary>
    void Clear(int x, int y, int z);
  }
}