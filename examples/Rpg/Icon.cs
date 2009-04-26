using System;

using Behemoth.Util;

namespace Rpg
{
  [Serializable]
  public struct Icon
  {
    public short Symbol;
    public Color Color;

    public Icon(char symbol, Color color)
    {
      Symbol = (short)symbol;
      Color = color;
    }
  }
}