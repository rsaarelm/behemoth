using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Alg;
using Behemoth.TaoUtil;

namespace Behetris
{
  /// <summary>
  /// A falling blocks game.
  /// </summary>
  public class Behetris : App
  {
    public Behetris() : base(160, 144, "Behetris")
    {
      blocks = new Block[][] {
        Block.MakeSet(
          1,
          "##",
          "##"),
        Block.MakeSet(
          2,
          " # ",
          " ##",
          " # "),
        Block.MakeSet(
          3,
          " # ",
          " # ",
          " ##"),
        Block.MakeSet(
          4,
          " # ",
          " # ",
          "## "),
        Block.MakeSet(
          5,
          " # ",
          "## ",
          "#  "),
        Block.MakeSet(
          6,
          " # ",
          " ##",
          "  #"),
        Block.MakeSet(
          7,
          "  # ",
          "  # ",
          "  # ",
          "  # "),
      };
    }


    public override void Update()
    {
      tick++;
    }


    public override void Display()
    {
      // XXX: Magic numbers. Not caring much now, since this stuff is limited
      // to display logic.
      DrawRect(0, 0, PixelWidth, PixelHeight, 230, 214, 156);
      DrawRect(0, 0, 8, 144, 57, 56, 41);
      DrawRect(88, 0, 8, 144, 57, 56, 41);

      for (int y = 0; y < fieldH; y++)
      {
        for (int x = 0; x < fieldW; x++)
        {
          if (field[x, y] != 0)
          {
            DrawCell(field[x, y], x, y);
          }
        }
      }

    }


    private void DrawCell(int type, int x, int y)
    {
      DrawRect(x * 8 + 8, PixelHeight - 8 - y * 8, 8, 8, 123, 113, 98);
    }


    private void PasteBlock(Block block, int x, int y)
    {
      for (int blockY = 0; blockY < block.Height; blockY++)
      {
        for (int blockX = 0; blockX < block.Width; blockX++)
        {
          if (block[blockX, blockY] != 0)
          {
            field[x + blockX, y + blockY] = block[blockX, blockY];
          }
        }
      }
    }


    private bool BlockFits(Block block, int x, int y)
    {
      for (int blockY = 0; blockY < block.Height; blockY++)
      {
        for (int blockX = 0; blockX < block.Width; blockX++)
        {
          if (block[blockX, blockY] != 0 && field[x + blockX, y + blockY] != 0)
          {
            return false;
          }
        }
      }
      return true;
    }


    public override void ReadInput()
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


    public static void Main(string[] args)
    {
      new Behetris().MainLoop();
    }


    private int tick = 0;

    // Field and dimensions of the Behetris well.
    private Field2<int> field = new Field2<int>();

    Block[][] blocks;

    const int fieldW = 10;
    const int fieldH = 18;

  }



  /// <summary>
  /// A single Behetris block.
  /// </summary>
  class Block
  {
    private Block()
    {
    }

    public Block(int type, params string[] shape)
    {
      Tile.AsciiTableDims(shape, out width, out height);
      Tile.AsciiTableIter(
        (ch, x, y) => { if (ch != ' ') this.shape[x, y] = type; },
        shape);
    }


    public int Width { get { return width; } }


    public int Height { get { return height; } }


    public int this[int x, int y]
    { get { return shape[x, y]; } }


    // Make a copy of the block rotated 90 degrees clockwise.
    public Block RotatedCopy()
    {
      Block result = new Block();
      result.width = height;
      result.height = width;

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          result.shape[result.width - 1 - y, x] = shape[x, y];
        }
      }

      return result;
    }


    public static Block[] MakeSet(int type, params string[] shape)
    {
      Block[] result = new Block[4];
      result[0] = new Block(type, shape);

      for (int i = 1; i < 4; i++)
      {
        result[i] = result[i - 1].RotatedCopy();
      }

      return result;
    }


    private int width;
    private int height;

    private Random rng = new Random();

    private Field2<int> shape = new Field2<int>();
  }
}
