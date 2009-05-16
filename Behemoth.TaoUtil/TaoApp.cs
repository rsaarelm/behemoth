using System;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Util;
using Behemoth.Apps;

namespace Behemoth.TaoUtil
{
  public class TaoApp : App
  {
    public TaoApp(int pixelWidth, int pixelHeight, string title)
    {
      RegisterService(typeof(ITaoService), new TaoWindow(pixelWidth, pixelHeight, title));
    }


    protected override void Draw(double timeElapsed)
    {
      base.Draw(timeElapsed);
      GetService<ITaoService>().FlipScreen();
    }


    protected override void InitApp()
    {
    }


    protected override void UninitApp()
    {
    }


    void ReadInput()
    {
      Sdl.SDL_Event evt;

      IScreen screen = null;

      TryGetService(out screen);

      while (Sdl.SDL_PollEvent(out evt) != 0)
      {
        switch (evt.type)
        {
        case Sdl.SDL_QUIT:
          Exit();
          break;

        case Sdl.SDL_VIDEORESIZE:
          GetService<ITaoService>().Resize(evt.resize.w, evt.resize.h);
          break;

        case Sdl.SDL_KEYDOWN:
          if (screen != null)
          {
            // XXX: Can we just cast unicode ints to chars?
            screen.KeyPressed(
              evt.key.keysym.sym,
              evt.key.keysym.mod,
              (char)evt.key.keysym.unicode);
          }
          break;

        case Sdl.SDL_KEYUP:
          if (screen != null)
          {
            screen.KeyReleased(evt.key.keysym.sym);
          }
          break;

        case Sdl.SDL_JOYAXISMOTION:
          // TODO: Handle joystick event.
          break;

        case Sdl.SDL_JOYBUTTONDOWN:
          // TODO: Handle joystick event.
          break;

        case Sdl.SDL_JOYBUTTONUP:
          // TODO: Handle joystick event.
          break;
        }
      }
    }


    protected override void Update(double timeElapsed)
    {
      ReadInput();
      base.Update(timeElapsed);
    }
  }
}