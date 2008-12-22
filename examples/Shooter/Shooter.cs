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
      Gl.glClearColor(0f, 0.1f, 0.1f, 1f);
      Gl.glShadeModel(Gl.GL_SMOOTH);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_DST_ALPHA);
      Gl.glEnable(Gl.GL_BLEND);
    }


    static void Resize(int w, int h)
    {
      Gl.glViewport(0, 0, w, h);
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluOrtho2D(-w/2.0, w/2.0, -h/2.0, h/2.0);
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

      Gl.glScaled(16.0, 16.0, 1.0);

      DrawSprite((DateTime.Now.Millisecond / 128) % 8);

      Gl.glPopMatrix();

      Glfw.glfwSwapBuffers();
    }


    static void DrawSprite(int frame)
    {
      const int rows = 8;
      const int columns = 8;

      float x0 = (float)(frame % columns) / (float)columns;
      float y0 = 1.0f - (float)((frame + columns) / rows) / (float)rows;

      float x1 = x0 + 1.0f / (float)columns;
      float y1 = y0 + 1.0f / (float)rows;

      Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);

      Gl.glBegin(Gl.GL_QUADS);

      Gl.glTexCoord2f(x0, y0);
      Gl.glVertex3f(-1.0f, -1.0f, 0.0f);

      Gl.glTexCoord2f(x1, y0);
      Gl.glVertex3f( 1.0f, -1.0f, 0.0f);

      Gl.glTexCoord2f(x1, y1);
      Gl.glVertex3f( 1.0f, 1.0f, 0.0f);

      Gl.glTexCoord2f(x0, y1);
      Gl.glVertex3f(-1.0f, 1.0f, 0.0f);

      Gl.glEnd();

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