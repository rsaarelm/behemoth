namespace Behemoth.Alg
{
  /// <summary>
  /// Utilities for operating on tile grids.
  /// </summary>
  public static class Tile
  {
    public delegate void CharHandler(char ch, int x, int y);

    /// <summary>
    /// Call a function for each character in an ascii table with the char and
    /// coordinates of the character.
    /// </summary>
    public static void AsciiTableIter(CharHandler func, string[] lines)
    {
      for (int y = 0; y < lines.Length; y++)
      {
        for (int x = 0; x < lines[y].Length; x++)
        {
          func(lines[y][x], x, y);
        }
      }
    }
  }
}