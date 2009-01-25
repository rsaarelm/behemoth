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


    public void Init() {}


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


      Gfx.GlColor(Color.Orange);

      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);

      Gfx.DrawCube(-1, -1, -1, 1, 1, 1);
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


    public void Camera()
    {
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluPerspective(
        60.0,
        (float)App.Service<ITaoService>().PixelWidth / (float)App.Service<ITaoService>().PixelHeight,
        0.1, 1000.0);
    }
  }
}