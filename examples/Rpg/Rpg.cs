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
          "##|..........==...##",
          "||.A..&.......==...#",
          "............*.==..##",
          "........*.....=..###",
          "#####.....#|||=|||||",
          "####|#....#...======",
          "||||.|||.||.......==",
          ".................%.=",
          "....................",
          "...............T....",
          "..qaaq......%..I....",
          "..q..a............T.",
          "..q............,..I.",
          "..aaaa..............",
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
          DrawSprite(x * spriteWidth, y * spriteHeight, terrain[x, y]);
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
      y = 14 - y;
      switch (ch)
      {
      case '.':
        terrain[x, y] = (byte)Sprite.Ground;
        break;
      case ',':
        terrain[x, y] = (byte)Sprite.Grass;
        break;
      case '#':
        terrain[x, y] = (byte)Sprite.CaveTop;
        break;
      case '|':
        terrain[x, y] = (byte)Sprite.CaveEdge;
        break;
      case 'q':
        terrain[x, y] = (byte)Sprite.WallTop;
        break;
      case 'a':
        terrain[x, y] = (byte)Sprite.WallEdge;
        break;
      case 'T':
        terrain[x, y] = (byte)Sprite.TreeTop;
        break;
      case 'I':
        terrain[x, y] = (byte)Sprite.TreeBottom;
        break;
      case '%':
        terrain[x, y] = (byte)Sprite.Shrub;
        break;
      case '*':
        terrain[x, y] = (byte)Sprite.Rocks;
        break;
      case '&':
        terrain[x, y] = (byte)Sprite.Glyph;
        break;
      case '=':
        terrain[x, y] = (byte)Sprite.Water;
        break;
      case 'A':
        terrain[x, y] = (byte)Sprite.Stalagmite;
        break;
      default:
        break;
      }
    }


    private Field2<byte> terrain = new Field2<byte>();


    public const int pixelWidth = 320;
    public const int pixelHeight = 240;

    public const int spriteWidth = 16;
    public const int spriteHeight = 16;

    public const string spriteTexture = "tiles.png";
  }
}
