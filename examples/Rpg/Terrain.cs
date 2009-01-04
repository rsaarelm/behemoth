using System;
using System.Collections.Generic;

namespace Rpg
{
  [Serializable]
  public struct Terrain
  {
    public Terrain(byte type)
    {
      Type = type;
    }

    // More stuff here if needed.
    public byte Type;
  }
}