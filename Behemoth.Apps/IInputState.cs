using System;

namespace Behemoth.Apps
{
  public interface IInputState
  {
    int MouseX { get; }
    int MouseY { get; }
    bool MouseButton(int num);

    int HotItem { get; set; }
    int ActiveItem { get; set; }

    bool KeyPressed(int keycode);
  }
}