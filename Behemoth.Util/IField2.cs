namespace Behemoth.Util
{
  /// <summary>
  /// A Field2 is an unbounded two-dimensional grid on which homogenous values
  /// can be placed.
  /// </summary>
  /// <typeparm name="T">
  /// The type of the field's cells.
  /// </typeparm>
  public interface IField2<T>
  {
    /// <summary>
    /// Access cells in the field.
    /// </summary>
    T this[int x, int y]
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
    void Clear(int x, int y);
  }
}