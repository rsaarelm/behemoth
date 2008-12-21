using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.DevIl;
using Tao.PhysFs;

namespace Shooter
{
  public class Shooter
  {
    const byte KEY_ESC = 27;

    static uint texture;


    public static void Main(string[] args)
    {
      Init();

      Glut.glutMainLoop();
    }


    static void Init()
    {
      Il.ilInit();

      Fs.PHYSFS_init("init");

      Fs.PHYSFS_addToSearchPath("Shooter.zip", 1);

      // Make the zip file found from build subdir too, so that it's easy to
      // run the exe from the project root dir.
      Fs.PHYSFS_addToSearchPath(Path.Combine("build", "Shooter.zip"), 1);


      InitGlut();

      texture = LoadTexture("sprites.png");
    }


    static void InitGlut()
    {
      Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
      Glut.glutInit();
      Glut.glutInitWindowSize(800, 600);
      Glut.glutCreateWindow("Behemoth Shooter");

      Glut.glutDisplayFunc(new Glut.DisplayCallback(Display));
      Glut.glutReshapeFunc(new Glut.ReshapeCallback(Reshape));
      Glut.glutKeyboardFunc(new Glut.KeyboardCallback(KeyboardCallback));
      Glut.glutIdleFunc(new Glut.IdleCallback(IdleCallback));
    }


    static void Reshape(int w, int h)
    {
      Console.WriteLine("Window size is now: " + w + ", " + h);
      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluPerspective(45.0, (double)w / (double)h, 0.1, 100.0);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
    }


    static void KeyboardCallback(byte ch, int mouseX, int mouseY)
    {
      if (ch == KEY_ESC)
      {
        Quit();
      }
    }


    static void Quit()
    {
      Glut.glutLeaveMainLoop();
    }


    static void IdleCallback()
    {
    }


    static void Display()
    {
      Gl.glClearColor(0f, 0.1f, 0.1f, 1f);
      Gl.glShadeModel(Gl.GL_SMOOTH);
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
      Gl.glLoadIdentity();

      Glu.gluLookAt(0, 1.0, 5.0, 0, 0, 0, 0, 1.0, 0);
      Glut.glutWireTeapot(1.0);

      Glut.glutSwapBuffers();
    }


    static uint MakeRgbaTexture(IntPtr pixels, int w, int h, bool clampEdge)
    {
      uint textureHandle;

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

      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);

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


    static void IOError(string msg)
    {
      throw new IOException(msg);
    }
  }
}