using System;
using System.Collections.Generic;

using Behemoth.Alg;
using Behemoth.TaoUtil;

namespace Rpg
{
  public static class UI
  {
    public static void Msg(string fmt, params Object[] args)
    {
      ((Rpg)App.Instance).Msg(fmt, args);
    }
  }
}
