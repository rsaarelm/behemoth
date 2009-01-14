using System;

using Behemoth.Alg;

namespace Rpg
{
  public interface IUIService : IAppService
  {
    void Msg(string fmt, params Object[] args);
  }
}