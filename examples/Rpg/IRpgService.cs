using System;

using Behemoth.App;
using Behemoth.Util;

namespace Rpg
{
  public interface IRpgService : IAppService
  {
    void Msg(string fmt, params Object[] args);

    void GameOver(string msg);

    Entity Player { get; }

    Rng Rng { get; }
  }
}