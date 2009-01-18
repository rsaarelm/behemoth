using System;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Util;
using Behemoth.Apps;
using Behemoth.TaoUtil;

namespace Rpg
{
  public class PlayScreen : IScreen
  {
    public void Init() {}


    public void Uninit() {}


    public void KeyPressed(int keycode, int keyMod, char ch)
    {
      // Clear the msg buffer whenever the player presses a key.
      Rpg.Service.ClearMsg();

      switch (keycode)
      {
      case Sdl.SDLK_ESCAPE:
        App.Instance.Exit();
        break;
      case Sdl.SDLK_q:
        Rpg.Service.GameOver("Quit.");
        break;
      case Sdl.SDLK_UP:
        Rpg.Service.MoveCmd(0);
        break;
      case Sdl.SDLK_RIGHT:
        Rpg.Service.MoveCmd(2);
        break;
      case Sdl.SDLK_DOWN:
        Rpg.Service.MoveCmd(4);
        break;
      case Sdl.SDLK_LEFT:
        Rpg.Service.MoveCmd(6);
        break;
      case Sdl.SDLK_SPACE:
        Rpg.Service.NewTurn();
        break;
      }
    }


    public void KeyReleased(int keycode) {}


    public void Update(double timeElapsed)
    {
      if (Rpg.Service.IsGameOver)
      {
        WaitKey();
        // Return to title screen.
        App.Service<IScreenManager>().SwapScreen(new TitleScreen());
      }
    }


    public void Draw(double timeElapsed)
    {
      Gfx.ClearScreen();
      DrawWorld(Rpg.Service.PlayerPos);
      DrawMessages();
    }


    void DrawWorld(Vec3 center)
    {
      int cols = Rpg.pixelWidth / Rpg.spriteWidth;
      int rows = Rpg.pixelHeight / Rpg.spriteHeight;

      int xOff = (int)center.X - cols / 2;
      int yOff = (int)center.Y - rows / 2;

      for (int y = 0; y <= rows; y++)
      {
        for (int x = 0; x <= cols; x++)
        {
          int mapX = xOff + x;
          int mapY = yOff + y;
          int mapZ = (int)center.Z;

          if (Rpg.Service.IsMapped(mapX, mapY, mapZ))
          {
            DrawSprite(
              x * Rpg.spriteWidth, y * Rpg.spriteHeight,
              TerrainIcon(mapX, mapY, mapZ));
          }
        }
      }

      // XXX: Iterating through every entity. Good optimization would be for
      // example to provide a Z-coordinate based entity index since pretty
      // much all of the current logic operates on a single Z layer.
      List<Entity> entitiesToDraw = new List<Entity>(
        Rpg.Service.World.EntitiesInRect(xOff, yOff, (int)center.Z, cols, rows));

      // Sort the entities in draw order.
      entitiesToDraw.Sort(
        (lhs, rhs) =>
        lhs.Get<CCore>().DrawPriority.CompareTo(rhs.Get<CCore>().DrawPriority));

      foreach (var entity in entitiesToDraw)
      {
        var core = entity.Get<CCore>();
        // Draw static entities anywhere on the mapped area, dynamic ones only
        // if they're instantly visible.

        // XXX: If static entities move around without the player's direct
        // action, this will show their moving around outside the pc's field
        // of view. A robust solution would require a separate map memory
        // structure which retains the old view even after the static entity
        // has covertly moved around.
        if ((core.IsStatic && Rpg.Service.Player.Get<CLos>().IsMapped(core.Pos)) ||
            (Rpg.Service.Player.Get<CLos>().IsVisible(core.Pos)))
        {
          DrawEntity(entity, xOff * Rpg.spriteWidth, yOff * Rpg.spriteHeight);
        }
      }
    }


    int TerrainIcon(int x, int y, int z)
    {
      var tile = Rpg.Service.World.Space[x, y, z];
      var nextTile = Rpg.Service.World.Space[x, y - 1, z];

      var useBackIcon = TerrainUtil.IsWall(tile) && TerrainUtil.IsWall(nextTile);
      return useBackIcon ? tile.Type.BackIcon : tile.Type.Icon;
    }


    void DrawSprite(double x, double y, int frame)
    {
      Gfx.DrawSprite(
        x, y, frame,
        Rpg.spriteWidth, Rpg.spriteHeight, App.Service<ITaoService>().Textures[Rpg.spriteTexture],
        16, 16);
    }


    void DrawMirroredSprite(double x, double y, int frame)
    {
      Gfx.DrawMirroredSprite(
        x, y, frame,
        Rpg.spriteWidth, Rpg.spriteHeight, App.Service<ITaoService>().Textures[Rpg.spriteTexture],
        16, 16);
    }


    public static bool IsFacingLeft(int facing)
    {
      return facing >= 4;
    }


    public void DrawEntity(Entity e, double xOff, double yOff)
    {
      CCore core;
      if (e.TryGet(out core))
      {
        int frame = core.Icon + (core.ActionPose ? 1 : 0);
        double x = -xOff + Rpg.spriteWidth * core.Pos.X;
        double y = -yOff + Rpg.spriteHeight * core.Pos.Y;

        if (IsFacingLeft(core.Facing))
        {
          DrawMirroredSprite(x, y, frame);
        }
        else
        {
          DrawSprite(x, y, frame);
        }
      }
    }


    void DrawMessages()
    {
      double y = Rpg.pixelHeight;
      foreach (string line in Rpg.Service.MsgLines)
      {
        y -= Rpg.fontH;
        DrawString(line, 0, y, Color.Aliceblue);
      }
    }


    void DrawString(String str, double x, double y, Color color)
    {
      var outlineColor = Color.Black;
      for (int yOff = -1; yOff <= 1; yOff++)
      {
        for (int xOff = -1; xOff <= 1; xOff++)
        {
          if (xOff != 0 || yOff != 0)
          {
            Gfx.DrawString(
              str, x + xOff * Rpg.fontPixelScale, y + yOff * Rpg.fontPixelScale,
              Rpg.fontW, Rpg.fontH, App.Service<ITaoService>().Textures[Rpg.fontTexture], outlineColor);
          }
        }
      }
      Gfx.DrawString(str, x, y, Rpg.fontW, Rpg.fontH, App.Service<ITaoService>().Textures[Rpg.fontTexture], color);
    }


    // XXX: Move to generic UI module.
    void WaitKey()
    {
      Sdl.SDL_Event evt;

      while (Sdl.SDL_WaitEvent(out evt) != 0)
      {
        if (evt.type == Sdl.SDL_KEYDOWN)
        {
          break;
        }
      }
    }

  }
}