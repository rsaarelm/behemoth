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
  public class Rpg : DrawableAppComponent, IRpgService
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
      app.Add(new Rpg());
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


    public override void Init()
    {
      tao = App.GetService<ITaoService>();

      App.RegisterService(typeof(IRpgService), this);

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


      // Test Lua dumping.
      //var lua = new LuaState();
      //lua.DoString("a = 1 b = 2 c = {5, 4, foo = 'bar'}");
      //
      //PrintTable(lua.DumpGlobals());


      var terrainTable = new Object[][] {
        Alg.OA("nothing", "NoTerrain", 0x00, 0x00),
        Alg.OA("ground", "Ground", 0x01),
        Alg.OA("water", "Water", 0x02),
        Alg.OA("grass", "Ground", 0x03),
        Alg.OA("stones", "Ground", 0x04),
        Alg.OA("tree", "Wall", 0x15, 0x05),
        Alg.OA("wall", "Wall", 0x07, 0x06),
        Alg.OA("rock", "Wall", 0x09, 0x08),
        Alg.OA("shrub", "Ground", 0x0A),
        Alg.OA("stalagmite", "Pillar", 0x0B),
        Alg.OA("glyph", "Ground", 0x0C),
        // Door opening not yet supported, so now the work like walls you
        // can't see through but can walk through.
        Alg.OA("door", "IllusionWall", 0x0D, 0x0D),
        Alg.OA("soot", "Ground", 0x0F),
        Alg.OA("rubble", "Ground", 0x10),
        Alg.OA("window", "TransparentWall", 0x11, 0x12),
        Alg.OA("dirt", "Ground", 0x13),
        Alg.OA("bookshelf", "Pillar", 0x14),
        Alg.OA("table", "Pillar", 0x16),
        Alg.OA("chest", "Ground", 0x17),
        Alg.OA("pillar", "Pillar", 0x19),
        Alg.OA("broken wall", "WallGap", 0x1A, 0x1B),
        Alg.OA("boards", "Ground", 0x1C),
        Alg.OA("boards", "Ground", 0x1D),
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


      LoadMap("example_map.tmx", 0, 0, 0);

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

      core.Icon = (int)Sprite.Fighter;
      core.SetPos(1, 44, 0);

      world.Add(pc);

      world.Globals["player"] = pc;

      DoLos();
    }


    void ReadInput()
    {
      Sdl.SDL_Event evt;

      while (Sdl.SDL_PollEvent(out evt) != 0)
      {
        switch (evt.type)
        {
        case Sdl.SDL_QUIT:
          App.Exit();
          break;

        case Sdl.SDL_KEYDOWN:
          // Clear the msg buffer whenever the player presses a key.
          ClearMsg();

          switch (evt.key.keysym.sym)
          {
          case Sdl.SDLK_ESCAPE:
            App.Exit();
            break;
          case Sdl.SDLK_q:
            GameOver("Quit.");
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
          case Sdl.SDLK_SPACE:
            NewTurn();
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


          // XXX: Really doesn't belong at this abstraction level...
        case Sdl.SDL_VIDEORESIZE:
          tao.Resize(evt.resize.w, evt.resize.h);
          break;
        }
      }
    }


    public override void Update(double timeElapsed)
    {
      if (gameOver)
      {
        WaitKey();
        App.Exit();
      }
      ReadInput();
    }


    public override void Draw(double timeElapsed)
    {
      Gfx.ClearScreen();
      DrawWorld(PlayerPos);
      DrawMessages();
    }


    public Vec3 PlayerPos { get { return Player.Get<CCore>().Pos; } }


    public Entity Player { get { return (Entity)world.Globals["player"]; } }


    void DrawMessages()
    {
      double y = pixelHeight;
      foreach (string line in messages)
      {
        y -= fontSize;
        DrawString(line, 0, y, Color.Aliceblue);
      }
    }


    public void Msg(string fmt, params Object[] args)
    {
      string msg = String.Format(fmt, args);
      // TODO: Handle newlines
      messages.Add(msg);
    }


    public void ClearMsg()
    {
      messages.Clear();
    }

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
          int mapX = xOff + x;
          int mapY = yOff + y;
          int mapZ = (int)center.Z;

          if (IsMapped(mapX, mapY, mapZ))
          {
            DrawSprite(
              x * spriteWidth, y * spriteHeight,
              TerrainIcon(mapX, mapY, mapZ));
          }
        }
      }

      // XXX: Iterating through every entity. Good optimization would be for
      // example to provide a Z-coordinate based entity index since pretty
      // much all of the current logic operates on a single Z layer.
      List<Entity> entitiesToDraw = new List<Entity>(
        world.EntitiesInRect(xOff, yOff, (int)center.Z, cols, rows));

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
        if ((core.IsStatic && Player.Get<CLos>().IsMapped(core.Pos)) ||
            (Player.Get<CLos>().IsVisible(core.Pos)))
        {
          DrawEntity(entity, xOff * spriteWidth, yOff * spriteHeight);
        }
      }
    }


    int TerrainIcon(int x, int y, int z)
    {
      var tile = world.Space[x, y, z];
      var nextTile = world.Space[x, y - 1, z];

      var useBackIcon = TerrainUtil.IsWall(tile) && TerrainUtil.IsWall(nextTile);
      return useBackIcon ? tile.Type.BackIcon : tile.Type.Icon;
    }


    void DrawSprite(double x, double y, int frame)
    {
      Gfx.DrawSprite(
        x, y, frame,
        spriteWidth, spriteHeight, tao.Textures[spriteTexture],
        16, 16);
    }


    void DrawMirroredSprite(double x, double y, int frame)
    {
      Gfx.DrawMirroredSprite(
        x, y, frame,
        spriteWidth, spriteHeight, tao.Textures[spriteTexture],
        16, 16);
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
              str, x + xOff * fontPixelScale, y + yOff * fontPixelScale,
              fontSize, tao.Textures[fontTexture], outlineColor);
          }
        }
      }
      Gfx.DrawString(str, x, y, fontSize, tao.Textures[fontTexture], color);
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
      IDictionary<String, int> tilesets;
      IList<Tuple2<String, int[]>> layers;

      TiledImport.LoadMapData(
        Media.GetPfsFileData(name),
        out width, out height,
        out tilesets,
        out layers);

      // XXX: Not verifying that the map data has the expected layers & tilesets
      // (terrain, entity and zone layers, terrain and zone tilesets)

      int[] tiles = layers[0].Second;
      int[] entities = layers[1].Second;

// TODO: Do zones too.
//      int[] zones = layers[2].Second;

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          int idx = x + (height - y - 1) * width;

          world.Space[x + xOff, y + yOff, zOff] =
            new TerrainTile(world.GetTerrain(tiles[idx] - 1));

          if (entities[idx] > 0)
          {
            string spawn = null;
            switch ((Sprite)(entities[idx] - 1))
            {
            case Sprite.Chest:
              spawn = "chest";
              break;
            case Sprite.Beastman:
              spawn = "beastman";
              break;
            case Sprite.Ooze:
              spawn = "ooze";
              break;
            case Sprite.Zombie:
              spawn = "zombie";
              break;
            case Sprite.DeathKnight:
              spawn = "deathKnight";
              break;
            default:
              Console.WriteLine("Warning: Unknown entity {0} in map.", entities[idx]);
              break;
            }

            if (spawn != null)
            {
              var entity = world.Spawn(spawn);
              entity.Get<CCore>().SetPos(x + xOff, y + yOff, zOff);
              world.Add(entity);
            }
          }
        }
      }
    }


    void MoveCmd(int dir8)
    {
      Action.AttackMove(Player, dir8);
      DoLos();
      NewTurn();
    }


    void DoLos()
    {
      Player.Get<CLos>().DoLos();
    }


    void NewTurn()
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

    public Rng Rng { get { return rng; } }

    /// <summary>
    /// Rpg.Service is a shortcut for
    /// Application.Instance.GetService<IRpgService>().
    /// </summary>
    public static IRpgService Service
    {
      get { return Application.Instance.GetService<IRpgService>(); }
    }


    private World world = new World();

    private List<string> messages = new List<string>();

    private ITaoService tao;

    private bool gameOver = false;

    private Rng rng = new DefaultRng();


    public const int pixelWidth = 640;
    public const int pixelHeight = 480;

    public const int spriteWidth = 32;
    public const int spriteHeight = 32;

    public const string spriteTexture = "tiles.png";

    public const string fontTexture = "font8x8.png";

    public const double fontSize = 16.0;
    public const double fontPixelScale = 2.0;

  }
}
