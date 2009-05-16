using System;
using System.Collections.Generic;

using Behemoth.Apps;
using Behemoth.Util;

namespace Rpg
{
  public interface IRpgService : IAppService
  {
    void NewGame();

    /// <summary>
    /// Print an onscreen message.
    /// </summary>
    void Msg(string fmt, params Object[] args);

    IEnumerable<string> MsgLines { get; }

    /// <summary>
    /// Clear the onscreen message buffer.
    /// </summary>
    void ClearMsg();

    void GameOver(string msg);

    Entity Player { get; }

    Rng Rng { get; }

    void MoveCmd(int dir8);

    void NewTurn();

    bool IsGameOver { get; }

    World World { get; }

    Vec3 PlayerPos { get; }

    /// <summary>
    /// Acquire a thread synchronization lock on game state.
    /// </summary>
    void AcquireWorldLock();

    /// <summary>
    /// Release the thread sychronization lock on game state.
    /// </summary>
    void ReleaseWorldLock();
  }
}