using System;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Util;
using Behemoth.Apps;
using Behemoth.TaoUtil;

namespace Rpg
{
  public class TitleScreen : IScreen
  {
    public void KeyPressed(int keycode, int keyMod, char ch)
    {
      switch (keycode)
      {
      case Sdl.SDLK_ESCAPE:
        App.Instance.Exit();
        break;
      case Sdl.SDLK_q:
        App.Instance.Exit();
        break;
      case Sdl.SDLK_n:
        StartGame();
        break;
      }
    }


    public void KeyReleased(int keycode) {}


    public void Init() {}


    public void Uninit() {}


    public void Update(double timeElapsed)
    {
    }


    public void Print(string txt, int line)
    {
      var color = Color.Green;

      Gfx.DrawString(txt, 0, Rpg.pixelHeight - Rpg.fontH * (line + 1),
                     Rpg.fontW, Rpg.fontH, App.Service<ITaoService>().Textures[Rpg.fontTexture],
                     color);

    }

    public void Draw(double timeElapsed)
    {
      Gfx.ClearScreen();

      Print("Behemoth RPG tech demo", 0);

      Print("Insert really impressive title screen...", 1);

      Print("N)ew game", 4);

      Print("Q)uit", 5);
    }


    void StartGame()
    {
      Rpg.Service.NewGame();
      App.Service<IScreenManager>().SwapScreen(new PlayScreen());
    }

  }
}