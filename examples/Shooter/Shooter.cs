using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tao.OpenGl;
using Tao.Glfw;
using Tao.DevIl;
using Tao.PhysFs;

namespace Shooter
{
  public class Shooter : IDisposable
  {
    const byte KEY_ESC = 27;

    static uint texture;

    static bool isRunning = true;

    static int pixelWidth = 320;
    static int pixelHeight = 240;


    public static void Main(string[] args)
    {
      Init();

      MainLoop();
    }


    static void Init()
    {
      Il.ilInit();

      Fs.PHYSFS_init("init");

      Fs.PHYSFS_addToSearchPath("Shooter.zip", 1);

      // Make the zip file found from build subdir too, so that it's easy to
      // run the exe from the project root dir.
      Fs.PHYSFS_addToSearchPath(Path.Combine("build", "Shooter.zip"), 1);

      InitGlfw();

      InitGl();

      texture = LoadTexture("sprites.png");
    }


    static void InitGlfw()
    {
      Glfw.glfwInit();
      Glfw.glfwOpenWindow(800, 600, 8, 8, 8, 8, 16, 0, Glfw.GLFW_WINDOW);
      Glfw.glfwSetWindowTitle("Behemoth Shooter");

      Glfw.glfwSetWindowSizeCallback(new Glfw.GLFWwindowsizefun(Resize));
      Glfw.glfwSetWindowCloseCallback(new Glfw.GLFWwindowclosefun(Close));

    }


    static void InitGl()
    {
      Gl.glEnable(Gl.GL_TEXTURE_2D);
      Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
      Gl.glClearColor(0f, 1.0f, 1.0f, 1f);
      Gl.glShadeModel(Gl.GL_SMOOTH);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
      Gl.glEnable(Gl.GL_BLEND);
    }


    static void Resize(int w, int h)
    {
      int x, y, width, height;
      MakeScaledViewport(
        pixelWidth, pixelHeight, w, h, out x, out y, out width, out height);
      Gl.glViewport(x, y, width, height);

      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluOrtho2D(0, pixelWidth, 0, pixelHeight);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
    }


    static int Close()
    {
      isRunning = false;
      return 0;
    }


    static void MainLoop()
    {
      while (isRunning)
      {
        Glfw.glfwPollEvents();

        if (Glfw.glfwGetKey(Glfw.GLFW_KEY_ESC) == Glfw.GLFW_PRESS)
        {
          isRunning = false;
        }

        Display();
      }
    }


    static void Display()
    {
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadIdentity();

      Gl.glPushMatrix();

      double second = (double)DateTime.Now.Ticks / 1e7;

      DrawSprite(160 + (float)(100 * Math.Sin(second)), 120, (int)(second * 10) % 8);
      DrawSprite(160, 104, 16);

      Gl.glPopMatrix();

      Glfw.glfwSwapBuffers();
    }


    static void DrawSprite(float x, float y, int frame)
    {
      const int rows = 8;
      const int columns = 8;

      const float spriteWidth = 16.0f;
      const float spriteHeight = 16.0f;

      float x0 = (float)(frame % columns) / (float)columns;
      float y0 = 1.0f - (float)((frame + columns) / rows) / (float)rows;

      float x1 = x0 + 1.0f / (float)columns;
      float y1 = y0 + 1.0f / (float)rows;

      Gl.glPushMatrix();

      Gl.glTranslatef(x, y, 0.0f);

      Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);

      Gl.glBegin(Gl.GL_QUADS);

      Gl.glTexCoord2f(x0, y0);
      Gl.glVertex3f(0.0f, 0.0f, 0.0f);

      Gl.glTexCoord2f(x1, y0);
      Gl.glVertex3f(spriteWidth, 0.0f, 0.0f);

      Gl.glTexCoord2f(x1, y1);
      Gl.glVertex3f(spriteWidth, spriteHeight, 0.0f);

      Gl.glTexCoord2f(x0, y1);
      Gl.glVertex3f(0.0f, spriteHeight, 0.0f);

      Gl.glEnd();

      Gl.glPopMatrix();
    }


    /// <description>
    /// Fit the largest centered (pixelWidth * scale, pixelHeight * scale)
    /// viewport with integer 'scale' into the given viewport. If the viewport
    /// is too small to for unit scale (pixelWidth, pixelHeight) viewport,
    /// scale the pixel viewport down into the largest non-integer fractional
    /// size that fits in the viewport.
    /// </description>
    static void MakeScaledViewport(
      int pixelWidth, int pixelHeight,
      int viewportWidth, int viewportHeight,
      out int x, out int y, out int width, out int height)
    {
      Debug.Assert(
        pixelWidth > 0 && pixelHeight > 0 && viewportWidth > 0 && viewportHeight > 0,
        "Zero or negative viewport dimensions are not allowed.");
      double pixelAspect = (double)pixelWidth / (double)pixelHeight;
      double viewportAspect = (double)viewportWidth / (double)viewportHeight;
      int scale = Math.Min(viewportWidth / pixelWidth, viewportHeight / pixelHeight);
      if (scale == 0) {
        // Viewport is too small to do proper pixels, just fit the largest rect we can in it.
        if (viewportAspect > pixelAspect)
        {
          // Fit Y, adjust X
          width = (int)(pixelAspect * viewportHeight);
          x = viewportWidth / 2 - width / 2;
          height = viewportHeight;
          y = 0;
        }
        else
        {
          // Fit X, adjust Y
          height = (int)(viewportWidth / pixelAspect);
          y = viewportHeight / 2 - height / 2;
          width = viewportWidth;
          x = 0;
        }
      }
      else
      {
        width = scale * pixelWidth;
        height = scale * pixelHeight;

        x = viewportWidth / 2 - width / 2;
        y = viewportHeight / 2 - height / 2;
      }
    }


    static uint MakeRgbaTexture(IntPtr pixels, int w, int h, bool clampEdge)
    {
      uint textureHandle;

      bool filterTexture = false;

      var filtering = filterTexture ? Gl.GL_LINEAR : Gl.GL_NEAREST;

      // Setup new texture.
      Gl.glGenTextures(1, out textureHandle);
      Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle);

      // Set texture parameters.
      Gl.glTexParameteri(
        Gl.GL_TEXTURE_2D,
        Gl.GL_TEXTURE_WRAP_S,
        (clampEdge ? Gl.GL_CLAMP : Gl.GL_REPEAT));
      Gl.glTexParameteri(
        Gl.GL_TEXTURE_2D,
        Gl.GL_TEXTURE_WRAP_T,
        (clampEdge ? Gl.GL_CLAMP : Gl.GL_REPEAT));

      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, filtering);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, filtering);

      // Create the texture.
      Gl.glTexImage2D(
        Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0,
        Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);

      return textureHandle;
    }


    // Load an image from PhysFS using DevIL and turn it into an OpenGL texture.
    static uint LoadTexture(string filename)
    {
      int imageId = LoadImage(filename);

      // Flip to compensate for the Y axis flip when moving to OpenGL coordinates.
      Ilu.iluFlipImage();

      Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE);
      int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
      int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);

      uint result = MakeRgbaTexture(Il.ilGetData(), width, height, true);

      // XXX: Mixing abstraction layers, there's a sublayer for creating IL
      // images but they are deleted here at a higher layer.
      Il.ilDeleteImages(1, ref imageId);

      return result;
    }


    unsafe static int LoadImage(string filename)
    {
      int imageId;
      Il.ilGenImages(1, out imageId);
      Il.ilBindImage(imageId);

      if (Fs.PHYSFS_exists(filename) == 0)
      {
        IOError("File "+filename+" not found.");
      }

      IntPtr file = Fs.PHYSFS_openRead(filename);
      long fileSize = Fs.PHYSFS_fileLength(file);
      byte[] fileData;
      Fs.PHYSFS_read(file, out fileData, 1, (uint)fileSize);
      Fs.PHYSFS_close(file);

      // XXX: Only loads PNG.
      IntPtr dataPtr = (IntPtr)Marshal.UnsafeAddrOfPinnedArrayElement(fileData, 0);
      if (!Il.ilLoadL(Il.IL_PNG, dataPtr, (int)fileSize))
      {
        IOError("Failed to load PNG image "+filename);
      }

      return imageId;
    }


    public void Dispose()
    {
      Glfw.glfwCloseWindow();
      Glfw.glfwTerminate();
    }


    static void IOError(string msg)
    {
      throw new IOException(msg);
    }
  }
}