using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Tao.OpenGl;
using Tao.Sdl;
using Tao.Lua;

using Behemoth.Util;
using Behemoth.Apps;
using Behemoth.TaoUtil;
using Behemoth.LuaUtil;

namespace Rpg
{
  public class Rpg : IRpgService
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
      Dirt = 0x13,
      Bookshelf = 0x14,
      TreeBottom = 0x15,
      Table = 0x16,
      Chest = 0x17,
      Chest2 = 0x18,
      Pillar = 0x19,
      BrokenWallEdge = 0x1a,
      BrokenWall = 0x1b,
      WoodenFloor = 0x1c,
      WoodenFloor2 = 0x1d,

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
      var app = new TaoApp(pixelWidth, pixelHeight, "Rpg demo");
      app.RegisterService(typeof(IRpgService), new Rpg());

      new ScreenManager(new TitleScreen()).Register(app);

      app.Run();
    }


    private void PrintTable(IDictionary<Object, Object> table)
    {
      foreach (var kvp in table)
      {
        if (kvp.Value is IDictionary<Object, Object>) {
          Console.WriteLine("{0}: [", kvp.Key);
          PrintTable((IDictionary<Object, Object>)kvp.Value);
          Console.WriteLine("]");
        }
        else
        {
          Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
        }
      }
    }


    public void Init()
    {
      var joystick = InputUtil.InitJoystick();

      if (joystick.HasValue)
      {
        //Console.WriteLine("Joystick detected.");
        if (joystick.Value.MatchesPS2Pad())
        {
          //Console.WriteLine("Joystick looks like a PS2 pad.");
        }
      }

      Sdl.SDL_EnableKeyRepeat(
        Sdl.SDL_DEFAULT_REPEAT_DELAY,
        Sdl.SDL_DEFAULT_REPEAT_INTERVAL);

      Media.AddPhysFsPath("Rpg.zip");
      Media.AddPhysFsPath("build", "Rpg.zip");

      NewGame();

      // Test Lua dumping.
      //var lua = new LuaState();
      //lua.DoString("a = 1 b = 2 c = {5, 4, foo = 'bar'}");
      //
      //PrintTable(lua.DumpGlobals());
    }


    public void Uninit() {}


    public void NewGame()
    {
      world = new World();
      ClearMsg();
      gameOver = false;
      rng = new DefaultRng();

      var terrainTable = new Object[][] {
        Alg.OA("nothing", "NoTerrain", 'x', Color.HotPink, Color.Black),
        Alg.OA("ground", "Ground", '.', Color.Gray, Color.Black),
        Alg.OA("water", "Water", 247, Color.DeepSkyBlue, Color.MediumBlue),
        Alg.OA("grass", "Ground", ',', Color.Green, Color.Black),
        Alg.OA("wall", "Wall", '#', Color.DarkGray, Color.Black),
        Alg.OA("rock", "Wall", '#', Color.Brown, Color.Black),
        Alg.OA("stalagmite", "Pillar", 'i', Color.Red, Color.Black),
        // Door opening not yet supported, so now the work like walls you
        // can't see through but can walk through.
        Alg.OA("door", "IllusionWall", '+', Color.Gray, Color.Black),
        Alg.OA("window", "TransparentWall", '#', Color.White, Color.Black),
        Alg.OA("dirt", "Ground", '.', Color.Peru, Color.Black),
        Alg.OA("pillar", "Pillar", 'I', Color.White, Color.Black),
        };

      foreach (var row in terrainTable)
      {
        world.AddTerrain(TerrainData.FromDataRow(row));
      }

      world.Add("beastman", new EntityTemplate(
                  CoreTemplate.Default("beastman", 0x50),
                  new BrainTemplate()));
      world.Add("ooze", new EntityTemplate(
                  CoreTemplate.Default("ooze", 0x54),
                  new BrainTemplate()));
      world.Add("zombie", new EntityTemplate(
                  CoreTemplate.Default("zombie", 0x56),
                  new BrainTemplate()));
      world.Add("deathKnight", new EntityTemplate(
                  CoreTemplate.Default("death knight", 0x58),
                  new BrainTemplate()));
      world.Add("chest", new EntityTemplate(
                  CoreTemplate.FloorStatic("chest", 0x17)));
      world.Add("gib", new EntityTemplate(
                  CoreTemplate.FloorStatic("gib", 0x40)));

      GenerateExampleMap();

      Entity pc = world.MakeEntity("avatar");
      var core = new CCore();
      pc.Set(core);
      pc.Set(new CLos());

      var brain = new CBrain();
      pc.Set(brain);
      // No AI for player. Use human control instead.
      brain.AiActive = false;
      // Different from standard creatures, create hostility.
      brain.Alignment = 1;
      brain.Might = 20.0;
      brain.Resistance = 4.0;

      core.Icon = (int)Sprite.Fighter;
      core.SetPos(1, 44, 0);

      world.Add(pc);

      world.Globals["player"] = pc;

      DoLos();
    }


    public Vec3 PlayerPos { get { return Player.Get<CCore>().Pos; } }


    public Entity Player { get { return (Entity)world.Globals["player"]; } }


    public void Msg(string fmt, params Object[] args)
    {
      string msg = String.Format(fmt, args);
      foreach (var paragraph in TextUtil.SplitAtNewlines(msg))
      {
        foreach (var line in TextUtil.SplitLongLine(paragraph, ConsoleColumns))
        {
          messages.Add(line);
        }
      }
    }


    public IEnumerable<string> MsgLines { get { return messages; } }


    public void ClearMsg()
    {
      messages.Clear();
    }


    public void GenerateExampleMap()
    {
      int z = 0;
      for (int y = 0; y < 80; y++)
      {
        for (int x = 0; x < 320; x++)
        {
          world.Space[x, y, z] = new TerrainTile(world.GetTerrain("ground"));
          if (x == 30)
          {
            world.Space[x, y, z] = new TerrainTile(world.GetTerrain("water"));
          }
        }
      }
    }


    public void MoveCmd(int dir8)
    {
      Action.AttackMove(Player, dir8);
      DoLos();
      NewTurn();
    }


    void DoLos()
    {
      Player.Get<CLos>().DoLos();
    }


    public void NewTurn()
    {
      UpdateBrains();
    }


    void UpdateBrains()
    {
      foreach (var e in world.Entities)
      {
        CBrain brain;
        if (e.TryGet(out brain))
        {
          brain.Update();
        }
      }
    }


    public bool IsMapped(int x, int y, int z)
    {
      return Player.Get<CLos>().IsMapped(new Vec3(x, y, z));
    }


    public void GameOver(string msg)
    {
      Msg(msg);
      gameOver = true;
    }


    public Rng Rng { get { return rng; } }


    public bool IsGameOver { get { return gameOver; } }


    public World World { get { return world; } }


    /// <summary>
    /// Rpg.Service is a shortcut for
    /// App.Service<IRpgService>().
    /// </summary>
    public static IRpgService Service
    {
      get { return App.Service<IRpgService>(); }
    }


    private World world;

    private List<string> messages = new List<string>();

    private bool gameOver;

    private Rng rng;


    public const int pixelWidth = 640;
    public const int pixelHeight = 480;

    public const string spriteTexture = "tiles.png";

    public const string fontTexture = "font8x16.png";
    public const double fontW = 8.0;
    public const double fontH = 16.0;
    public const double fontPixelScale = 1.0;

    public const string iconFontTexture = "font8x8.png";
    public const double iconFontW = 16.0;
    public const double iconFontH = 16.0;

    public const int ConsoleColumns = 40;
  }
}
