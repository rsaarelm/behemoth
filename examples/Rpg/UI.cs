using System;
using System.Collections.Generic;

using Behemoth.App;

namespace Rpg
{
  public static class UI
  {
    public static void Msg(string fmt, params Object[] args)
    {
      Rpg.Service.Msg(fmt, args);
    }
  }
}
