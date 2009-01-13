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
  [Obsolete("To be replaced by Behemoth.Alg.App")]
  public abstract class App
  {
    public App(int pixelWidth, int pixelHeight, string title)
    {
      this.pixelWidth = pixelWidth;
      this.pixelHeight = pixelHeight;
      this.windowTitle = title;

      if (App.instance != null)
      {
        throw new ApplicationException("Trying to instantiate multiple Apps.");
      }

      App.instance = this;

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
        textureCache.Dispose();
        Media.UninitFacilities();
        Sdl.SDL_Quit();
      }
    }


    protected virtual void Init()
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

      // Scale up small pixelsize for the initial window. Assume that the user
      // has at least 800x600 resolution.
      const int targetW = 800;
      const int targetH = 600;
      int scale = 1;
      while (pixelWidth * (scale + 1) <= targetW && pixelHeight * (scale + 1) <= targetH)
      {
        scale++;
      }

      Resize(pixelWidth * scale, pixelHeight * scale);
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

      // Resizing seems to mess up OpenGL state on Windows. Need to set
      // textures to be regenerated by clearing the texture cache and reinput
      // OpenGL settings.
      textureCache.Clear();
      InitGl();

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


    protected virtual void Update()
    {
      tick++;
    }


    protected abstract void Display();


    protected abstract void ReadInput();


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


    public static void ClearScreen()
    {
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
    }


    public static App Instance { get { return instance; } }


    public int PixelWidth { get { return pixelWidth; } }

    public int PixelHeight { get { return pixelHeight; } }

    public double PixelScale { get { return pixelScale; } }

    public TextureCache Textures { get { return textureCache; } }

    public int Tick { get { return tick; } }

    public int TargetFps = 30;

    public bool UseSound = true;

    public bool IsRunning = true;


    private int pixelWidth;
    private int pixelHeight;
    private double pixelScale;

    private int tick = 0;

    private string windowTitle;

    private double timeStamp = CurrentSeconds;

    private TextureCache textureCache = new TextureCache(new ImageCache(), 0);

    private static App instance = null;
  }
}