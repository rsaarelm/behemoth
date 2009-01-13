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
  public class Behetris : Behemoth.TaoUtil.App
  {
    public Behetris() : base(160, 144, "Behetris")
    {
      blocks = new Block[][] {
        Block.MakeSet(
          1,
          "    ",
          " ## ",
          " ## ",
          "    "),
        Block.MakeSet(
          2,
          "   ",
          "###",
          " # "),
        Block.MakeSet(
          3,
          "   ",
          "###",
          "  #"),
        Block.MakeSet(
          4,
          "   ",
          "###",
          "#  "),
        Block.MakeSet(
          5,
          "   ",
          "## ",
          " ##"),
        Block.MakeSet(
          6,
          "   ",
          " ##",
          "## "),
        Block.MakeSet(
          7,
          "    ",
          "####",
          "    ",
          "    "),
      };

      // Set field edges to impassable.
      for (int y = -10; y < fieldH + 1; y++)
      {
        field[-1, y] = -1;
        field[fieldW, y] = -1;
      }
      for (int x = 0; x < fieldW; x++)
      {
        field[x, fieldH] = -1;
      }

      SpawnBlock();
    }


    protected override void Update()
    {
      base.Update();

      if (speedDrop)
      {
        StepBlock();
      }
      else if (dropCounter-- < 0)
      {
        StepBlock();
        dropCounter = dropSpeed;
      }

      if (steerCounter-- < 0)
      {
        steerCounter = steerSpeed;
        MoveBlock(steerDir);
      }

      CheckClears();
    }


    protected override void Display()
    {
      // XXX: Magic numbers. Not caring much now, since this stuff is limited
      // to display logic.
      Gfx.DrawRect(0, 0, PixelWidth, PixelHeight, 230, 214, 156);
      Gfx.DrawRect(0, 0, 8, 144, 57, 56, 41);
      Gfx.DrawRect(88, 0, 8, 144, 57, 56, 41);

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

      DrawBlock(CurrentBlock, blockX, blockY);
      DrawFuseBlock();

    }


    private void DrawFuseBlock()
    {
      if (BlinkOn)
      {
        int fuseX, fuseY;
        BlockFusePos(out fuseX, out fuseY);
        DrawBlock(CurrentBlock, fuseX, fuseY);
      }
    }


    private bool BlinkOn { get { return (Tick / 4) % 2 == 0; } }


    private void DrawCell(int type, int x, int y)
    {
      Gfx.DrawRect(x * 8 + 8, PixelHeight - 8 - y * 8, 8, 8, 123, 113, 98);
    }


    // Draw blocks that haven't dropped yet.
    private void DrawBlock(Block block, int x, int y)
    {
      for (int blockY = 0; blockY < block.Height; blockY++)
      {
        for (int blockX = 0; blockX < block.Width; blockX++)
        {
          if (block[blockX, blockY] != 0)
          {
            DrawCell(block[blockX, blockY], x + blockX, y + blockY);
          }
        }
      }

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
          case Sdl.SDLK_LEFT:
            Steer(-1);
            break;
          case Sdl.SDLK_RIGHT:
            Steer(1);
            break;
          case Sdl.SDLK_UP:
            RotateBlock(1);
            break;
          case Sdl.SDLK_DOWN:
            speedDrop = true;
            break;

          }
          break;

        case Sdl.SDL_KEYUP:
          switch (evt.key.keysym.sym)
          {
          case Sdl.SDLK_DOWN:
            speedDrop = false;
            break;
          case Sdl.SDLK_LEFT:
            Unsteer(-1);
            break;
          case Sdl.SDLK_RIGHT:
            Unsteer(1);
            break;
          }
          break;

        case Sdl.SDL_VIDEORESIZE:
          Resize(evt.resize.w, evt.resize.h);
          break;
        }
      }
    }


    private void Steer(int dir)
    {
      Debug.Assert(-1 <= dir && dir <= 1);
      steerDir = Alg.Clamp(-1, steerDir + dir, 1);
      // Looks like we started moving. Make the next move instantaneous.
      if (dir != 0 && steerDir != 0)
      {
        steerCounter = 0;
      }
    }


    private void Unsteer(int dir)
    {
      if (steerDir == dir)
      {
        steerDir = 0;
      }
      else
      {
        Steer(-dir);
      }
    }


    private void SpawnBlock()
    {
      currentBlockIdx = rng.Next(blocks.Length);
      currentBlockRot = 0;
      blockX = 3;
      blockY = 0;
      dropCounter = dropSpeed;

      if (!BlockFits(CurrentBlock, blockX, blockY))
      {
        GameOver();
      }
    }


    private bool MoveBlock(int dir)
    {
      if (speedDrop)
      {
        return false;
      }
      if (dir < 0 && BlockFits(CurrentBlock, blockX - 1, blockY))
      {
        blockX -= 1;
        return true;
      }
      else if (dir > 0 && BlockFits(CurrentBlock, blockX + 1, blockY))
      {
        blockX += 1;
        return true;
      }
      else
      {
        return false;
      }
    }


    private void BlockFusePos(out int fuseX, out int fuseY)
    {
      fuseX = blockX;
      for (int y = blockY; y < fieldH; y++)
      {
        if (!BlockFits(CurrentBlock, blockX, y + 1))
        {
          fuseY = y;
          return;
        }
      }
      Debug.Assert(false, "Shouldn't end up here.");
      fuseY = blockY;
    }


    private bool RotateBlock(int dir)
    {
      if (speedDrop)
      {
        return false;
      }

      int newBlockRot;

      if (dir < 0)
      {
        newBlockRot = (currentBlockRot + 3) % 4;
      }
      else if (dir > 0)
      {
        newBlockRot = (currentBlockRot + 1) % 4;
      }
      else
      {
        return false;
      }

      if (BlockFits(blocks[currentBlockIdx][newBlockRot], blockX, blockY))
      {
        currentBlockRot = newBlockRot;
        return true;
      }
      else
      {
        return false;
      }
    }


    /// <summary>
    /// Move the current block down one step.
    /// </summary>
    /// <returns>
    /// True if the block was dropped, false if it became part of the field.
    /// </returns>
    private bool StepBlock()
    {
      if (BlockFits(CurrentBlock, blockX, blockY + 1))
      {
        blockY += 1;
        return true;
      }
      else
      {
        FuseBlock();
        return false;
      }
    }


    private void FuseBlock()
    {
      PasteBlock(CurrentBlock, blockX, blockY);
      speedDrop = false;
      if (Overflow)
      {
        GameOver();
      }
      else
      {
        SpawnBlock();
      }
    }


    public bool Overflow
    {
      get
      {
        for (int x = 0; x < fieldW; x++)
        {
          if (field[x, -1] != 0)
          {
            return true;
          }
        }
        return false;
      }
    }


    private void GameOver()
    {
      // TODO: Don't quit instantly, countdown to exit.
      Console.WriteLine("Ha-ha.");
      IsRunning = false;
    }


    private void ClearLine(int lineY)
    {
      for (int y = lineY - 1; y >= -1; y--)
      {
        for (int x = 0; x < fieldW; x++)
        {
          field[x, y + 1] = field[x, y];
        }
      }
    }


    private int LineCount(int lineY)
    {
      int result = 0;
      for (int x = 0; x < fieldW; x++)
      {
        if (field[x, lineY] != 0)
        {
          result++;
        }
      }
      return result;
    }


    private bool IsFullLine(int lineY)
    {
      return LineCount(lineY) == fieldW;
    }


    private void CheckClears()
    {
      for (int y = fieldH - 1; y >= 0; y--)
      {
        if (IsFullLine(y))
        {
          ClearLine(y);
        }
      }
    }


    Block CurrentBlock
    { get { return blocks[currentBlockIdx][currentBlockRot]; } }


    public static void Main(string[] args)
    {
      new Behetris().MainLoop();
    }


    // Field and dimensions of the Behetris well.
    private Field2<int> field = new Field2<int>();

    private Block[][] blocks;

    private int currentBlockIdx;
    private int currentBlockRot;
    private int blockX;
    private int blockY;

    private int dropSpeed = 30;
    private int dropCounter = 0;

    private int steerSpeed = 5;
    private int steerCounter = 0;
    private int steerDir = 0;

    private Random rng = new Random();

    private bool speedDrop = false;

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

    private Field2<int> shape = new Field2<int>();
  }
}
