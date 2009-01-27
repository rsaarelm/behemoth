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
    }


    void InitLighting()
    {
      Gl.glEnable(Gl.GL_LIGHTING);
      Gl.glEnable(Gl.GL_LIGHT0);
      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 1.0f, 1.0f, 1.0f });
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

      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadIdentity();
      Gl.glTranslatef(0, 0, -10);
      Gl.glRotatef(15, 0, 0, 1);
      Gl.glRotatef(App.Instance.Tick, 0, 1, 0);
      Gl.glRotatef(App.Instance.Tick / 2, 1, 0, 0);


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


    void Tetrahedron()
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


    void Octahedron()
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


    double TerrainHeight(double x, double y)
    {
      // TODO: Perlin noise..
      return (1.0 + Math.Sin(x / 10.0) * Math.Sin(y / 10.0)) * 5.0;
    }


  }
}