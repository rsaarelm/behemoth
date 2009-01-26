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
      Gl.glEnable(Gl.GL_LIGHTING);
      Gl.glEnable(Gl.GL_COLOR_MATERIAL);
      Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
      Gl.glEnable(Gl.GL_LIGHT0);
      Gl.glEnable(Gl.GL_DEPTH_TEST);
      Gl.glShadeModel(Gl.GL_SMOOTH);

      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 1.0f, 1.0f, 1.0f });
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

//      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);

//      Gfx.DrawCube(-1, -1, -1, 2, 2, 2);
      MengerSponge(3, -2, -2, -2, 4, 4, 4);
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
  }
}