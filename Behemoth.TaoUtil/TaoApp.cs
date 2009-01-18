using System;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Util;
using Behemoth.Apps;

namespace Behemoth.TaoUtil
{
  public class TaoApp : App, ITaoService
  {
    public TaoApp(int pixelWidth, int pixelHeight, string title)
    {
      this.pixelWidth = pixelWidth;
      this.pixelHeight = pixelHeight;
      this.windowTitle = title;

      LibInit();

      RegisterService(typeof(ITaoService), this);
    }


    private void LibInit()
    {
      Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
      Media.InitFacilities();

      InitSdl();
      InitGl();
    }


    protected override void Draw(double timeElapsed)
    {
      base.Draw(timeElapsed);
      Sdl.SDL_GL_SwapBuffers();
    }


    protected override void InitApp()
    {
    }


    protected override void UninitApp()
    {
      textureCache.Dispose();
      Media.UninitFacilities();
      Sdl.SDL_Quit();
    }


    public void Init()
    {
    }

    public void Uninit() {}


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



    public int PixelWidth { get { return pixelWidth; } }

    public int PixelHeight { get { return pixelHeight; } }

    public double PixelScale { get { return pixelScale; } }

    public TextureCache Textures { get { return textureCache; } }

    public bool UseSound { get { return useSound; } set { useSound = value; } }


    private int pixelWidth;
    private int pixelHeight;
    private double pixelScale;

    private bool useSound = true;

    private string windowTitle;

    private TextureCache textureCache = new TextureCache(new ImageCache(), 0);

  }
}