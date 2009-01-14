using System;

using Behemoth.App;

namespace Rpg
{
  public interface IUIService : IAppService
  {
    void Msg(string fmt, params Object[] args);
  }
}