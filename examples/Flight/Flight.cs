using System;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Apps;
using Behemoth.TaoUtil;
using Behemoth.Util;

namespace Flight
{
  public interface IFlightService : IAppService
  {
    IEnumerable<Thing> Things { get; }

    void AddThing(Thing thing);

    void RemoveThing(Thing thing);

    Rng Rng { get; }
  }


  /// <summary>
  /// A 3D flight demo.
  /// </summary>
  public class Flight : IScreen, IFlightService
  {
    const int terrainSize = 128;

    static float[] lightPos = {0.2f, 1.0f, 1.0f};


    public static void Main(String[] args)
    {
      var app = new TaoApp(640, 480, "Flight");
      var flight = new Flight();
      app.RegisterService(typeof(IScreen), flight);
      app.RegisterService(typeof(IFlightService), flight);
      app.Run();
    }


    public Flight() {}


    public void Init() {
      Gl.glEnable(Gl.GL_DEPTH_TEST);

      InitLighting();
      InitShading();

      Media.AddPhysFsPath("Flight.zip");
      Media.AddPhysFsPath("build", "Flight.zip");

      heightmap = Media.LoadPixels("heightmap.png");

      particleTexture = Media.PixelsToGlTexture(
        MemUtil.MakeScaledArray(
          32, 32,
          (double x, double y) =>
          new Color(1.0, 1.0, 1.0, Num.CosSpread(x, y))),
          0);

      landscape = new Model();

      HeightMap(TerrainHeight, 0.0, 0.0, 1.0, 1.0, terrainSize, terrainSize,
                out landscape.Vertices,
                out landscape.Normals,
                out landscape.Faces);

      things.Add(new Creep(new Vec3(0, 0, 15), new Vec3(0, -5, 0)));
      things.Add(new Creep(new Vec3(0, 5, 15), new Vec3(0, -5, 0)));

      things.Add(new Tower(new Vec3(10, -30, 15)));
      things.Add(new Tower(new Vec3(-10, -30, 15)));

    }


    void InitLighting()
    {
      Gl.glEnable(Gl.GL_LIGHTING);
      Gl.glEnable(Gl.GL_LIGHT0);
    }


    void InitShading()
    {
      Gl.glEnable(Gl.GL_COLOR_MATERIAL);
      Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
      Gl.glShadeModel(Gl.GL_SMOOTH);
    }


    public void Uninit() {}


    public void Update(double timeElapsed)
    {
      foreach (Thing o in new List<Thing>(things))
      {
        o.Update(timeElapsed);
        if (!o.IsAlive)
        {
          RemoveThing(o);
        }
      }
    }



    public void Draw(double timeElapsed)
    {
      Gfx.ClearScreen();
      Camera();

      Gl.glRotatef(App.Instance.Tick, 0, 0, 1);

      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPos);

      foreach (Thing o in things)
      {
        o.Draw(timeElapsed);
      }


      Gl.glPushMatrix();
      Gl.glTranslatef(-terrainSize / 2, -terrainSize / 2, -16);
      Gfx.GlColor(Color.Olivedrab);
      landscape.Draw();
      Gl.glPopMatrix();


      Gl.glPushMatrix();
      Gl.glLoadIdentity();
      Gl.glTranslatef(0, 0, -10);

      Gl.glNormal3f(0, 0, 1);
      Gfx.DrawSprite(0, 0, 0, 1, 1, particleTexture, 1, 1, 0, 0, Color.White);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
;

      Gl.glPopMatrix();
    }


    public void KeyPressed(int keycode, int keyMod, char ch)
    {
      switch (keycode)
      {
      case Sdl.SDLK_ESCAPE:
        App.Instance.Exit();
        break;
      }
    }


    public void KeyReleased(int keycode)
    {
    }


    void Camera()
    {
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluPerspective(
        60.0,
        (float)App.Service<ITaoService>().PixelWidth / (float)App.Service<ITaoService>().PixelHeight,
        0.1, 1000.0);

      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadIdentity();
      Glu.gluLookAt(-10, -50, 40,
                    0, 0, 0,
                    0, 0, 1);
    }


    public static void MengerSponge(
      int degree,
      float x, float y, float z, float w, float h, float d)
    {
      if (degree < 1)
      {
        Gfx.DrawCube(x, y, z, w, h, d);
      }
      else
      {
        var pw = w / 3;
        var ph = h / 3;
        var pd = d / 3;

        for (int zp = 0; zp < 3; zp++)
        {
          for (int yp = 0; yp < 3; yp++)
          {
            for (int xp = 0; xp < 3; xp++)
            {
              var form = (xp == 1 ? 0 : 1) + (yp == 1 ? 0 : 1) + (zp == 1 ? 0 : 1);
              if (form > 1)
              {
                MengerSponge(
                  degree - 1,
                  x + xp * pw, y + yp * ph, z + zp * pd,
                  pw, ph , pd);
              }
            }
          }
        }
      }
    }


    public static Vec3 HeightMapNormal(
      Func<double, double, double> heightFunc,
      double x, double y, double xScale, double yScale)
    {
      Vec3 result = new Vec3(0, 0, 0);

      // Sum the normals of four cardinal arcs from this point to +/-
      // xScale/yScale.

      result += Vec3.Cross(
        HeightLine(heightFunc, x, y, x - xScale, y),
        new Vec3(0, -1, 0));
      result += Vec3.Cross(
        HeightLine(heightFunc, x, y, x, y - yScale),
        new Vec3(1, 0, 0));
      result += Vec3.Cross(
        HeightLine(heightFunc, x, y, x + xScale, y),
        new Vec3(0, 1, 0));
      result += Vec3.Cross(
        HeightLine(heightFunc, x, y, x, y + yScale),
        new Vec3(-1, 0, 0));

      return result.Unit();

    }


    static Vec3 HeightLine(
      Func<double, double, double> heightFunc,
      double x0, double y0, double x1, double y1)
    {
      return
        new Vec3(x1, y1, heightFunc(x1, y1)) -
        new Vec3(x0, y0, heightFunc(x0, y0));
    }


    public static void HeightMap(
      Func<double, double, double> heightFunc,
      double x0, double y0, double xScale, double yScale,
      int w, int h,
      out float[,] vertices,
      out float[,] normals,
      out short[,] faces)
    {
      int nVertices = (w + 1) * (h + 1);
      vertices = new float[nVertices, 3];
      normals = new float[nVertices, 3];
      faces = new short[w * h * 2, 3];

      for (int y = 0; y <= h; y++)
      {
        for (int x = 0; x <= w; x++)
        {
          int idx = x + y * (w + 1);
          double z = heightFunc(x0 + x * xScale, y0 + y * yScale);
          vertices[idx, 0] = (float)(x * xScale);
          vertices[idx, 1] = (float)(y * yScale);
          vertices[idx, 2] = (float)z;

          var normal = HeightMapNormal(
            heightFunc, x0 + x * xScale, y0 + y * yScale, xScale, yScale);
          normals[idx, 0] = (float)normal.X;
          normals[idx, 1] = (float)normal.Y;
          normals[idx, 2] = (float)normal.Z;
        }
      }

      int faceIdx = 0;
      for (int y = 0; y < h; y++)
      {
        for (int x = 0; x < w; x++)
        {
          faces[faceIdx, 0] = (short)(x + y * (w + 1));
          faces[faceIdx, 1] = (short)((x + 1) + (y + 1) * (w + 1));
          faces[faceIdx, 2] = (short)(x + (y + 1) * (w + 1));
          faceIdx++;

          faces[faceIdx, 0] = (short)(x + y * (w + 1));
          faces[faceIdx, 1] = (short)((x + 1) + y * (w + 1));
          faces[faceIdx, 2] = (short)((x + 1) + (y + 1) * (w + 1));
          faceIdx++;
        }
      }
    }


    public static void Tetrahedron()
    {
      float[,] vertices = {
        { 1,  1,  1},
        {-1, -1,  1},
        {-1,  1, -1},
        { 1, -1, -1},
      };

      float c = (float)Math.Sqrt(3);
      float[,] normals = {
        { c,  c,  c},
        {-c, -c,  c},
        {-c,  c, -c},
        { c, -c, -c},
      };

      short[,] faces = {
        {0, 3, 1},
        {0, 3, 2},
        {0, 2, 1},
        {1, 2, 3},
      };

      Gfx.DrawTriMesh(vertices, normals, faces);
    }


    public static Model MakeOctahedron()
    {
      float[,] vertices = {
        { 0,  0,  1},
        {-1,  0,  0},
        { 0, -1,  0},
        { 1,  0,  0},
        { 0,  1,  0},
        { 0,  0, -1},
      };

      float[,] normals = {
        {  0,  0,  1},
        { -1,  0,  0},
        {  0, -1,  0},
        {  1,  0,  0},
        {  0,  1,  0},
        {  0,  0, -1},
      };

      short[,] faces = {
        {0, 2, 1},
        {0, 3, 2},
        {0, 4, 3},
        {0, 1, 4},
        {5, 1, 2},
        {5, 2, 3},
        {5, 3, 4},
        {5, 4, 1},
      };

      return new Model(vertices, normals, faces);
    }


    public static Model Octahedron()
    {
      if (octahedron == null)
      {
        octahedron = MakeOctahedron();
      }
      return (Model)octahedron;
    }

    static Model? octahedron = null;


    static double TerrainHeight(double x, double y)
    {
      int xPix = Num.Mod((int)x, heightmap.GetLength(1));
      int yPix = Num.Mod((int)y, heightmap.GetLength(0));
      return ((double)heightmap[xPix, yPix].R) / 8.0;
    }


    public IDictionary<string, Model> Models { get { return models; } }


    public IEnumerable<Thing> Things { get { return things; } }


    public void AddThing(Thing thing)
    {
      things.Add(thing);
    }


    public void RemoveThing(Thing thing)
    {
      things.Remove(thing);
    }


    public static void Spawn(Thing thing)
    {
      App.Service<IFlightService>().AddThing(thing);
    }


    public Rng Rng { get { return rng; } }


    Model landscape;

    static Color[,] heightmap;

    public Rng rng = new DefaultRng();

    IDictionary<string, Model> models = new Dictionary<string, Model>();

    ICollection<Thing> things = new List<Thing>();

    int particleTexture;
  }


  public struct Model
  {
    public float[,] Vertices;
    public float[,] Normals;
    public short[,] Faces;


    public Model(float[,] vertices, float[,] normals, short[,] faces)
    {
      this.Vertices = vertices;
      this.Normals = normals;
      this.Faces = faces;
    }


    public void Draw()
    {
      Gfx.DrawTriMesh(Vertices, Normals, Faces);
    }
  }


  public enum ThingType { Creep = 1, Tower = 2 }


  public abstract class Thing
  {
    public Vec3 Pos;

    public Model Model;

    public ThingType Type;

    public bool IsAlive = true;

    public Color Color = Color.Aliceblue;

    public abstract void Update(double timeElapsed);

    public virtual void Draw(double timeElapsed)
    {
      Gl.glPushMatrix();
      Gfx.GlColor(Color);

      Gl.glTranslatef((float)Pos.X, (float)Pos.Y, (float)Pos.Z);
      // TODO: Rotation.
      Model.Draw();
      Gl.glPopMatrix();
    }


    public virtual void Damage(double amount)
    {
    }


    public virtual double Dist(Thing other)
    {
      return (this.Pos - other.Pos).Abs();
    }
  }


  public class Creep : Thing
  {
    public Vec3 velocity;
    public double health = 100.0;

    public Creep(Vec3 pos, Vec3 velocity)
    {
      this.Model = Flight.Octahedron();
      this.Type = ThingType.Creep;
      this.velocity = velocity;
      this.Pos = pos;
    }


    public override void Update(double timeElapsed)
    {
      Pos += velocity * timeElapsed;
    }


    public override void Damage(double amount)
    {
      health -= amount;
      for (int i = 0; i < (int)amount + 1; i++)
      {
        Flight.Spawn(new Particle(
                       Pos,
                       App.Service<IFlightService>().Rng.UnitVec() * 10.0,
                       Color.Red,
                       1.0,
                       0.1));

      }
      if (health < 0.0)
      {
        IsAlive = false;
      }
    }
  }


  public class Tower : Thing
  {
    public Tower(Vec3 pos)
    {
      this.Type = ThingType.Tower;
      this.Pos = pos;
      this.Color = Color.Orange;
    }


    public override void Draw(double timeElapsed)
    {
      // XXX: Repeating the matrix & color & translation code from parent. Ugly.
      Gl.glPushMatrix();
      Gfx.GlColor(Color);

      Gl.glTranslatef((float)Pos.X, (float)Pos.Y, (float)Pos.Z);

      Flight.MengerSponge(Math.Abs(stage), -2, -2, -2, 4, 4, 4);

      Gl.glPopMatrix();
    }


    public override void Update(double timeElapsed)
    {
      time += timeElapsed;
      if (time > switchDelay)
      {
        time -= switchDelay;
        stage++;
        if (stage > 2)
        {
          stage = -2;
        }
      }

      Thing target = ClosestCreep();
      if (target != null && Dist(target) < KillRange)
      {
        Shoot(target);
      }
    }


    Thing ClosestCreep()
    {
      Thing result = null;
      foreach (Thing o in App.Service<IFlightService>().Things)
      {
        if (o.Type == ThingType.Creep)
        {
          if (result == null || Dist(o) < Dist(result))
          {
            result = o;
          }
        }
      }
      return result;
    }


    void Shoot(Thing target)
    {
      // TODO: Shoot effect
      // TODO: Cooldown until next shot.
      target.Damage(power);
    }



    double time = 0.0;
    int stage = -2;

    double power = 10.0;

    const double switchDelay = 1.0;

    const double KillRange = 15.0;
  }


  public class Particle : Thing
  {
    public Particle(Vec3 pos, Vec3 velocity, Color color, double lifetime, double scale)
    {
      // XXX: Use proper particles.
      this.Model = Flight.Octahedron();

      this.Pos = pos;
      this.Color = color;
      this.velocity = velocity;
      this.lifetime = lifetime;
      this.scale = scale;
    }


    public override void Update(double timeElapsed)
    {
      Pos += velocity * timeElapsed;
      lifetime -= timeElapsed;
      if (lifetime < 0)
      {
        IsAlive = false;
      }
    }


    public override void Draw(double timeElapsed)
    {
      // XXX: Code repetition
      Gl.glPushMatrix();
      Gfx.GlColor(Color);

      Gl.glTranslatef((float)Pos.X, (float)Pos.Y, (float)Pos.Z);
      Gl.glScalef((float)scale, (float)scale, (float)scale);
      // TODO: Use proper simple particles, not meshes.
      Model.Draw();
      Gl.glPopMatrix();
    }


    Vec3 velocity;
    double lifetime;
    double scale;
  }
}