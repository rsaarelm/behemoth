using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Alg;
using Behemoth.TaoUtil;

namespace Rpg
{
  public class Rpg : App
  {
    public enum Sprite {
      Ground = 0x00,
      Water = 0x01,
      Grass = 0x02,
      Rocks = 0x03,
      TreeTop = 0x04,
      WallEdge = 0x05,
      WallTop = 0x06,
      CaveEdge = 0x07,
      CaveTop = 0x08,
      Shrub = 0x09,
      Stalagmite = 0x0a,
      Glyph = 0x0b,

      TreeBottom = 0x14,

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

      Media.AddPhysFsPath("Rpg.zip");
      Media.AddPhysFsPath("build", "Rpg.zip");

      Tile.AsciiTableIter(
        SetCharTerrain, Alg.A(
          "##|..........==...|#",
          "||.A..&.......==...#",
          "............*.==..##",
          "........*.....=..###",
          "#####.....#|||=|||||",
          "####|#....#...======",
          "||||.|||.||.......==",
          ".................%.=",
          "....................",
          "...............T....",
          "..qaaq......%..I...T",
          "..q..a............TT",
          "..q............,..IT",
          "..aaaa.............I",
          "..............%...,."));

      terrain[3, 2] = (byte)Sprite.Fighter;

      terrain[4, 3] = (byte)Sprite.Gib;

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
          }
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
      for (int y = 0; y < pixelHeight / spriteHeight; y++)
      {
        for (int x = 0; x < pixelWidth / spriteWidth; x++)
        {
          DrawSprite(x * spriteWidth, y * spriteHeight, world.Space[x, y, 0].Type);
        }
      }
    }


    public void DrawSprite(float x, float y, int frame)
    {
      Gfx.DrawSprite(
        x, y, frame,
        spriteWidth, spriteHeight, Textures[spriteTexture],
        16, 16);
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
        spr = Sprite.CaveTop;
        break;
      case '|':
        spr = Sprite.CaveEdge;
        break;
      case 'q':
        spr = Sprite.WallTop;
        break;
      case 'a':
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
      default:
        break;
      }

      world.Space[x, y, z] = new Terrain((byte)spr);
    }


    private Field2<byte> terrain = new Field2<byte>();

    private World world = new World();


    public const int pixelWidth = 480;
    public const int pixelHeight = 360;

    public const int spriteWidth = 16;
    public const int spriteHeight = 16;

    public const string spriteTexture = "tiles.png";
  }
}
