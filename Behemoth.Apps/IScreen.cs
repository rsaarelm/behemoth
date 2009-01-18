using System;

namespace Behemoth.Apps
{
  /// <summary>
  /// A special service that implements the toplevel game screen.
  /// </summary>
  public interface IScreen : IAppService
  {
    void Draw(double timeElapsed);

    void Update(double timeElapsed);

    /// <summary>
    /// Inform the screen that a key has been pressed. Keycode and keyMod
    /// (whether shift, ctrl etc are pressed) values depend on the
    /// implementation. Currently assuming that SDL constants are used.
    /// </summary>
    void KeyPressed(int keycode, int keyMod, char ch);

    void KeyReleased(int keycode);
  }
}