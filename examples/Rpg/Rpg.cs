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
        Alg.OA("ground", "Ground", '.', Color.Black, Color.Gray),
        Alg.OA("water", "Water", 247, Color.DeepSkyBlue, Color.MediumBlue),
        Alg.OA("grass", "Ground", '.', Color.Black, Color.Green),
        Alg.OA("wall", "Wall", '#', Color.DarkGray, Color.Black),
        Alg.OA("rock", "Wall", '#', Color.Chocolate, Color.Black),
        Alg.OA("stalagmite", "Pillar", 'i', Color.Red, Color.Black),
        // Door opening not yet supported, so now the work like walls you
        // can't see through but can walk through.
        Alg.OA("door", "IllusionWall", '+', Color.Gray, Color.Black),
        Alg.OA("window", "TransparentWall", '#', Color.White, Color.Black),
        Alg.OA("dirt", "Ground", '.', Color.Black, Color.Peru),
        Alg.OA("pillar", "Pillar", 'I', Color.White, Color.Black),
        };

      foreach (var row in terrainTable)
      {
        world.AddTerrain(TerrainData.FromDataRow(row));
      }

      world.Add(new EntityTemplate(
                  CoreTemplate.Default("ooze", 'j', Color.GreenYellow),
                  BrainTemplate.Default(2, 1))
                .AddProps("powerlevel", 1.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("bat", 'b', Color.SlateBlue),
                  BrainTemplate.Default(1, 3))
                .AddProps("powerLevel", 1.0, "rarity", 80.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("carnivorous worm", 's', Color.Orchid),
                  BrainTemplate.Default(4, 2))
                .AddProps("powerLevel", 3.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("plasma wisp", 'q', Color.Aquamarine),
                  BrainTemplate.Default(2, 8))
                .AddProps("powerLevel", 3.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("garm", 'c', Color.DarkRed),
                  BrainTemplate.Default(6, 7))
                .AddProps("powerLevel", 4.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("feral octopus", 'o', Color.LightSeaGreen),
                  BrainTemplate.Default(8, 10))
                .AddProps("powerLevel", 5.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("pirate", 'V', Color.OrangeRed),
                  BrainTemplate.Default(11, 14))
                .AddProps("powerLevel", 6.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("hermit", 'H', Color.Goldenrod),
                  BrainTemplate.Default(9, 16))
                .AddProps("powerLevel", 6.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("shard AI", 'd', Color.DarkSlateGray),
                  BrainTemplate.Default(11, 20))
                .AddProps("powerLevel", 7.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("praetor", 'V', Color.MediumVioletRed),
                  BrainTemplate.Default(16, 18))
                .AddProps("powerLevel", 7.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("pirate reaver", 'V', Color.Red),
                  BrainTemplate.Default(20, 15))
                .AddProps("powerLevel", 9.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("marine", 'W', Color.Teal),
                  BrainTemplate.Default(17, 20))
                .AddProps("powerLevel", 9.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("huww", 'G', Color.OliveDrab),
                  BrainTemplate.Default(28, 11))
                .AddProps("powerLevel", 10.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("roamer AI", 'd', Color.SteelBlue),
                  BrainTemplate.Default(16, 30))
                .AddProps("powerLevel", 10.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("special ops marine", 'W', Color.Aquamarine),
                  BrainTemplate.Default(25, 22))
                .AddProps("powerLevel", 12.0, "rarity", 100.0));
      world.Add(new EntityTemplate(
                  CoreTemplate.Default("simulacrum AI", 'K', Color.Chartreuse),
                  BrainTemplate.Default(30, 35))
                .AddProps("powerLevel", 13.0, "rarity", 100.0));

      world.Add(new EntityTemplate(
                  CoreTemplate.Default("avatar", '@', Color.GhostWhite),
                  BrainTemplate.Default(13, 16)));
      world.Add(new EntityTemplate(
                  CoreTemplate.FloorStatic("gib", '*', Color.DarkRed)));
      
      GenerateExampleMap();

      Entity pc = world.Spawn("avatar", new Vec3(1, 44, 0));
                
      Action.MakePlayer(pc);

      world.Spawn("ooze", new Vec3(5, 43, 0));

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
      
      for (int y = 0; y < WorldHeight; y++)
      {
        for (int x = 0; x < WorldWidth; x++)
        {
          double scale = 0.03;
          double noise = Num.PerlinNoise(0.5, 6, (double)x * scale, (double)y * scale, 0.0);
          noise += 0.5;
          
          var terr = "";
          if (noise < 0.15)
          {
            terr = "water";
          }
          else if (noise < 0.25)
          {
            terr = "dirt";
          }
          else if (noise < 0.6)
          {
            terr = "grass";
          }
          else if (noise < 0.8)
          {
            terr = "ground";
          }
          else if (noise >= 0.8)
          {
            terr = "rock";
          }
          world.Space[x, y, z] = new TerrainTile(world.GetTerrain(terr));
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

    public const int WorldHeight = 80;
    public const int WorldWidth = 1024;
  }
}
