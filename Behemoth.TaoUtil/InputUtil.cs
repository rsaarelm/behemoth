using System;
using System.Collections.Generic;

using Tao.Sdl;

namespace Behemoth.TaoUtil
{
  public static class InputUtil
  {
    public static JoystickInfo? InitJoystick()
    {
      var numJoysticks = Sdl.SDL_NumJoysticks();
      JoystickInfo? result = null;

      if (numJoysticks > 0)
      {
        var joy = Sdl.SDL_JoystickOpen(0);

        if (joy != IntPtr.Zero)
        {
          result = new JoystickInfo(0, joy);
          return result;
        }
      }

      return null;
    }
  }


  public struct JoystickInfo
  {
    public string Name;
    public int Axes;
    public int Buttons;
    public int Balls;
    public int Hats;


    internal JoystickInfo(int index, IntPtr sdlJoyPtr)
    {
      Name = Sdl.SDL_JoystickName(index);
      Axes = Sdl.SDL_JoystickNumAxes(sdlJoyPtr);
      Buttons = Sdl.SDL_JoystickNumButtons(sdlJoyPtr);
      Balls = Sdl.SDL_JoystickNumBalls(sdlJoyPtr);
      Hats = Sdl.SDL_JoystickNumHats(sdlJoyPtr);
    }


    public override string ToString()
    {
      return String.Format(
        "\"{0}\": {1} axes, {2} buttons, {3} balls, {4} hats.",
        Name, Axes, Buttons, Balls, Hats);
    }


    /// <summary>
    /// Check whether a joystick matches a configuration of components. Can be
    /// used to guess if a joystick is of a specific type and map controls
    /// accordingly.
    /// </summary>
    public bool MatchesConfig(int axes, int buttons, int balls, int hats)
    {
      return Axes == axes && Buttons == buttons && Balls == balls && Hats == hats;
    }


    public bool MatchesPS2Pad()
    {
      return Axes == 6 && Buttons == 16 && Balls == 0 && Hats == 0;

    }
  }
}