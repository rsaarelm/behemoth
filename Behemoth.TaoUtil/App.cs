using System;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Alg;

namespace Behemoth.TaoUtil
{
  /// <summary>
  /// Base class for game applications.
  /// </summary>
  public abstract class App
  {
    public App(int pixelWidth, int pixelHeight, string title)
    {
      this.pixelWidth = pixelWidth;
      this.pixelHeight = pixelHeight;
      this.windowTitle = title;

      Init();
    }


    public void MainLoop()
    {
      try {
        while (IsRunning)
        {
          ReadInput();
          if (TimeToUpdate)
          {
            Update();
            Display();
            Sdl.SDL_GL_SwapBuffers();
          }
        }
      }
      finally
      {
        Media.UninitFacilities();
        Sdl.SDL_Quit();
      }
    }


    public virtual void Init()
    {
      Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
      Media.InitFacilities();

      InitSdl();
      InitGl();
    }


    void InitSdl()
    {
      Sdl.SDL_WM_SetCaption(windowTitle, "");
      Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DOUBLEBUFFER, 1);

      Resize(pixelWidth, pixelHeight);
    }


    void InitGl()
    {
      Gl.glEnable(Gl.GL_TEXTURE_2D);
      Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
      Gl.glClearColor(0f, 0.0f, 0.0f, 1f);
      Gl.glShadeModel(Gl.GL_SMOOTH);

      // Blending function for sprite transparency
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
      Gl.glEnable(Gl.GL_BLEND);
    }


    public void Resize(int w, int h)
    {
      int x, y, width, height;

      Sdl.SDL_SetVideoMode(w, h, 32, Sdl.SDL_RESIZABLE | Sdl.SDL_OPENGL);

      Geom.MakeScaledViewport(
        pixelWidth, pixelHeight, w, h, out x, out y, out width, out height);

      pixelScale = width / pixelWidth;

      Gl.glViewport(x, y, width, height);

      PixelProjection();
    }


    /// <summary>
    /// A 2d projection where 1 unit corresponds to 1 pixel in the desired
    /// pixel dimensions.
    /// </summary>
    public void PixelProjection()
    {
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluOrtho2D(0, pixelWidth, 0, pixelHeight);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
    }


    public abstract void Update();


    public abstract void Display();


    public abstract void ReadInput();


    public void Quit()
    {
      IsRunning = false;
    }


    /// Return whether it's time for the next frame with the current FPS. A
    /// side effect of returning true is moving the frame tracking logic to
    /// the next frame, so make sure to cache the result.
    bool TimeToUpdate
    {
      get
      {
        double interval = 1.0 / TargetFps;
        if (CurrentSeconds - timeStamp > interval)
        {
          timeStamp += interval;
          return true;
        }
        else
        {
          return false;
        }
      }
    }


    public static double CurrentSeconds
    { get { return (double)DateTime.Now.Ticks / 1e7; } }


    public static void DrawRect(double x, double y, double w, double h, byte r, byte g, byte b)
    {
      // Clear the bound texture.
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

      Gl.glColor3f((float)r / 256, (float)g / 256, (float)b / 256);

      Gl.glBegin(Gl.GL_QUADS);

      Gl.glVertex3f((float)x, (float)y, 0.0f);

      Gl.glVertex3f((float)(x + w), (float)y, 0.0f);

      Gl.glVertex3f((float)(x + w), (float)(y + h), 0.0f);

      Gl.glVertex3f((float)x, (float)(y + h), 0.0f);

      Gl.glEnd();
    }



    private int pixelWidth;
    private int pixelHeight;
    private double pixelScale;

    private int tick = 0;

    private string windowTitle;

    private double timeStamp = CurrentSeconds;


    public int TargetFps = 30;

    public bool UseSound = true;

    public bool IsRunning = true;

    public int PixelWidth { get { return pixelWidth; } }
    public int PixelHeight { get { return pixelHeight; } }
  }
}