using System;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Apps;
using Behemoth.TaoUtil;
using Behemoth.Util;

namespace Flight
{
  /// <summary>
  /// A 3D flight demo.
  /// </summary>
  public class Flight : IScreen
  {
    const int terrainSize = 64;

    static float[] lightPos = {0.2f, 1.0f, 1.0f};


    public static void Main(String[] args)
    {
      var app = new TaoApp(640, 480, "Flight");
      app.RegisterService(typeof(IScreen), new Flight());
      app.Run();
    }


    public Flight() {}


    public void Init() {
      Gl.glEnable(Gl.GL_DEPTH_TEST);

      InitLighting();
      InitShading();

      landscape = new Model();

      HeightMap(TerrainHeight, 0.0, 0.0, 1.0, 1.0, terrainSize, terrainSize,
                out landscape.Vertices,
                out landscape.Normals,
                out landscape.Faces);
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


    public void Update(double timeElapsed) {}



    public void Draw(double timeElapsed)
    {
      Gfx.ClearScreen();
      Camera();

      Gl.glRotatef(App.Instance.Tick, 0, 0, 1);

      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPos);

      Gl.glPushMatrix();
      Gl.glTranslatef(-terrainSize / 2, -terrainSize / 2, -16);

      Gfx.GlColor(Color.Olivedrab);
      landscape.Draw();
      Gl.glPopMatrix();

      Gl.glTranslatef(0, 0, 10);

      Gfx.GlColor(Color.Orange);

      MengerSponge((App.Instance.Tick / 30) % 4, -2, -2, -2, 4, 4, 4);
      Gl.glTranslatef(0, 4, 0);

      Gfx.GlColor(Color.Orchid);
      Octahedron();

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
      Glu.gluLookAt(-10, -10, 30,
                    0, 0, 0,
                    0, 0, 1);
    }


    void MengerSponge(
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


    public void Octahedron()
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

      Gfx.DrawTriMesh(vertices, normals, faces);
    }


    static double TerrainHeight(double x, double y)
    {
      // TODO: Perlin noise..
      return (1.0 + Math.Sin(x / 5.0) * Math.Sin(y / 5.0)) * 5.0 + Num.Noise((int)x, (int)y);
    }


    Model landscape;

    static Rng rng = new DefaultRng();
  }


  struct Model
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
}