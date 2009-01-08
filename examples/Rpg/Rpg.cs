using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Alg;
using Behemoth.TaoUtil;

namespace Rpg
{
  public class Rpg : App
  {
    public enum Sprite {
      Empty = 0x00,
      Ground = 0x01,
      Water = 0x02,
      Grass = 0x03,
      Rocks = 0x04,
      TreeTop = 0x05,
      Wall = 0x06,
      WallEdge = 0x07,
      Rock = 0x08,
      RockEdge = 0x09,
      Shrub = 0x0a,
      Stalagmite = 0x0b,
      Glyph = 0x0c,
      ClosedDoor = 0x0d,
      OpenDoor = 0x0e,
      Waste = 0x0f,

      Window = 0x11,
      Window2 = 0x12,
      TreeBottom = 0x15,

      Pillar = 0x19,

      Gib = 0x40,
      Flash = 0x41,
      Beastman = 0x50,
      Fighter = 0x52,
      Ooze = 0x54,
      Zombie = 0x56,
      DeathKnight = 0x58,
    }


    public static void Main(string[] args)
    {
      new Rpg().MainLoop();
    }


    public Rpg() : base (pixelWidth, pixelHeight, "Rpg demo")
    {
    }


    protected override void Init()
    {
      base.Init();

      var joystick = InputUtil.InitJoystick();

      if (joystick.HasValue)
      {
        Console.WriteLine("Joystick detected.");
        if (joystick.Value.MatchesPS2Pad())
        {
          Console.WriteLine("Joystick looks like a PS2 pad.");
        }
      }

      Sdl.SDL_EnableKeyRepeat(
        Sdl.SDL_DEFAULT_REPEAT_DELAY,
        Sdl.SDL_DEFAULT_REPEAT_INTERVAL);

      Media.AddPhysFsPath("Rpg.zip");
      Media.AddPhysFsPath("build", "Rpg.zip");

      LoadMap("example_map.tmx", 0, 0, 0);

      Entity pc = world.MakeEntity("avatar");
      CoreComponent core = new CoreComponent();
      pc.Set(core);
      core.Icon = (int)Sprite.Fighter;
      core.SetPos(3, 78, 0);

      world.Add(pc);

      world.Globals["player"] = pc;
    }


    protected override void ReadInput()
    {
      Sdl.SDL_Event evt;

      while (Sdl.SDL_PollEvent(out evt) != 0)
      {
        switch (evt.type)
        {
        case Sdl.SDL_QUIT:
          Quit();
          break;

        case Sdl.SDL_KEYDOWN:
          switch (evt.key.keysym.sym)
          {
          case Sdl.SDLK_ESCAPE:
            Quit();
            break;
          case Sdl.SDLK_UP:
            MoveCmd(0);
            break;
          case Sdl.SDLK_RIGHT:
            MoveCmd(2);
            break;
          case Sdl.SDLK_DOWN:
            MoveCmd(4);
            break;
          case Sdl.SDLK_LEFT:
            MoveCmd(6);
            break;

          }
          break;

        case Sdl.SDL_JOYAXISMOTION:
          //Console.WriteLine("Joy {0}: {1} [{2}]", evt.jaxis.which, evt.jaxis.axis, evt.jaxis.val);
          break;

        case Sdl.SDL_JOYBUTTONDOWN:
          switch (evt.jbutton.button)
          {
            // XXX: Hardcoded for PS2 pad
            // XXX: No key repeat for joystick, no fun tapping the keys.
            // XXX: Diagonal movement with the pad? Movement using the analog stick?
          case 12:
            MoveCmd(0);
            break;
          case 13:
            MoveCmd(2);
            break;
          case 14:
            MoveCmd(4);
            break;
          case 15:
            MoveCmd(6);
            break;
          }

          Console.WriteLine("Joy {0}: {1} [{2}]", evt.jbutton.which, evt.jbutton.button, evt.jbutton.state);
          break;


        case Sdl.SDL_VIDEORESIZE:
          Resize(evt.resize.w, evt.resize.h);
          break;
        }
      }
    }


    protected override void Update()
    {
    }


    protected override void Display()
    {
      DrawWorld(PlayerPos);
      DrawString("Fonter online.", 0, pixelHeight - 8, Color.Aliceblue);
    }


    public Vec3 PlayerPos { get { return Player.Get<CoreComponent>().Pos; } }


    public Entity Player { get { return (Entity)world.Globals["player"]; } }


    void DrawWorld(Vec3 center)
    {
      int cols = pixelWidth / spriteWidth;
      int rows = pixelHeight / spriteHeight;

      int xOff = (int)center.X - cols / 2;
      int yOff = (int)center.Y - rows / 2;

      for (int y = 0; y <= rows; y++)
      {
        for (int x = 0; x <= cols; x++)
        {
          DrawSprite(x * spriteWidth, y * spriteHeight, world.Space[xOff + x, yOff + y, (int)center.Z].Type);
        }
      }

      // XXX: Iterating through every entity. Good optimization would be for
      // example to provide a Z-coordinate based entity index since pretty
      // much all of the current logic operates on a single Z layer.
      var entitiesToDraw = world.EntitiesInRect(xOff, yOff, (int)center.Z, cols, rows);

      // Here we could sort entitiesToDraw by any priority preferences.

      foreach (var entity in entitiesToDraw)
      {
        DrawEntity(entity, xOff * spriteWidth, yOff * spriteHeight);
      }
    }


    void DrawSprite(double x, double y, int frame)
    {
      Gfx.DrawSprite(
        x, y, frame,
        spriteWidth, spriteHeight, Textures[spriteTexture],
        16, 16);
    }


    void DrawMirroredSprite(double x, double y, int frame)
    {
      Gfx.DrawMirroredSprite(
        x, y, frame,
        spriteWidth, spriteHeight, Textures[spriteTexture],
        16, 16);
    }


    void DrawString(String str, double x, double y, Color color)
    {
      var outlineColor = Color.Black;
      Gfx.DrawString(str, x+1, y, 8, Textures[fontTexture], outlineColor);
      Gfx.DrawString(str, x+1, y-1, 8, Textures[fontTexture], outlineColor);
      Gfx.DrawString(str, x, y-1, 8, Textures[fontTexture], outlineColor);

      Gfx.DrawString(str, x, y, 8, Textures[fontTexture], color);
    }


    public static bool IsFacingLeft(int facing)
    {
      return facing >= 4;
    }


    public void DrawEntity(Entity e, double xOff, double yOff)
    {
      CoreComponent core;
      if (e.TryGet(out core))
      {
        int frame = core.Icon + (core.ActionPose ? 1 : 0);
        double x = -xOff + spriteWidth * core.Pos.X;
        double y = -yOff + spriteHeight * core.Pos.Y;

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


    void LoadMap(string name, int xOff, int yOff, int zOff)
    {
      int width;
      int height;
      int[] tiles;
      TiledImport.LoadMapData(
        Media.GetPfsFileData(name),
        out width, out height, out tiles);

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          world.Space[x + xOff, y + yOff, zOff] =
            new Terrain((byte)(tiles[x + (height - y - 1) * width] - 1));

        }
      }
    }


    void SetCharTerrain(char ch, int x, int y)
    {
      int z = 0;
      y = 14 - y;
      Sprite spr = Sprite.Ground;

      switch (ch)
      {
      case '.':
        spr = Sprite.Ground;
        break;
      case ',':
        spr = Sprite.Grass;
        break;
      case '#':
        spr = Sprite.Rock;
        break;
      case '|':
        spr = Sprite.RockEdge;
        break;
      case 'p':
        spr = Sprite.Pillar;
        break;
      case 'w':
        spr = Sprite.Wall;
        break;
      case 'e':
        spr = Sprite.WallEdge;
        break;
      case 'T':
        spr = Sprite.TreeTop;
        break;
      case 'I':
        spr = Sprite.TreeBottom;
        break;
      case '%':
        spr = Sprite.Shrub;
        break;
      case '*':
        spr = Sprite.Rocks;
        break;
      case '&':
        spr = Sprite.Glyph;
        break;
      case '=':
        spr = Sprite.Water;
        break;
      case 'A':
        spr = Sprite.Stalagmite;
        break;
      case '+':
        spr = Sprite.Window;
        break;
      case '-':
        spr = Sprite.Window2;
        break;
      case 'd':
        spr = Sprite.ClosedDoor;
        break;
      default:
        break;
      }

      world.Space[x, y, z] = new Terrain((byte)spr);
    }


    void MoveCmd(int dir8)
    {
      Vec3 moveVec = Geom.Dir8ToVec(dir8);
      Action.MoveRel(Player, moveVec);
    }


    private World world = new World();


    public const int pixelWidth = 480;
    public const int pixelHeight = 360;

    public const int spriteWidth = 16;
    public const int spriteHeight = 16;

    public const string spriteTexture = "tiles.png";

    public const string fontTexture = "font8x8.png";
  }
}
