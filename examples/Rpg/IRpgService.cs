using System;
using System.Collections.Generic;

using Behemoth.Apps;
using Behemoth.Util;

namespace Rpg
{
  public interface IRpgService : IAppService
  {
    void Msg(string fmt, params Object[] args);

    IEnumerable<string> MsgLines { get; }

    void ClearMsg();

    void GameOver(string msg);

    Entity Player { get; }

    Rng Rng { get; }


    void MoveCmd(int dir8);
    void NewTurn();

    bool IsGameOver { get; }

    bool IsMapped(int x, int y, int z);

    World World { get; }

    Vec3 PlayerPos { get; }
  }
}