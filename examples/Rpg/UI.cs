using System;
using System.Collections.Generic;

using Behemoth.Alg;

namespace Rpg
{
  public static class UI
  {
    public static void Msg(string fmt, params Object[] args)
    {
      App.Instance.GetService<IUIService>().Msg(fmt, args);
    }
  }
}
