using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Tao.OpenGl;
using Tao.Glfw;
using Tao.DevIl;
using Tao.PhysFs;

using Behemoth.Alg;

namespace Shooter
{
  abstract class Entity
  {
    public virtual void Display(int xOffset, int yOffset)
    {
      Shooter.DrawSprite((int)X + xOffset, (int)Y + yOffset, Frame);
    }


    public virtual void Update(EntityManager context)
    {
    }


    public double X;
    public double Y;
    public int Frame;
  }


  class Explosion : Entity
  {
    public Explosion(double x, double y)
    {
      X = x;
      Y = y;
      Frame = startFrame;
      cooldown = rate;
    }


    public override void Update(EntityManager context)
    {
      if (cooldown-- <= 0)
      {
        cooldown = rate;
        Frame++;
        if (Frame == endFrame)
        {
          context.Remove(this);
        }
      }
    }


    private const int rate = 4;
    private const int startFrame = 8;
    private const int endFrame = 12;
    private int cooldown;
  }


  class Avatar : Entity
  {
    public Avatar()
    {
      Y = 8.0;
      X = Shooter.pixelWidth / 2 - Shooter.spriteWidth / 2;
      Frame = 16;
    }


    public override void Update(EntityManager context)
    {
      if (!IsAlive)
      {
        return;
      }

      if (IsShooting)
      {
        if (cooldown-- <= 0)
        {
          Fire();
        }
      }

      if (IsMovingLeft && !IsMovingRight)
      {
        X = Math.Max(minX, X - speed);
      }
      else if (IsMovingRight && !IsMovingLeft)
      {
        X = Math.Min(maxX, X + speed);
      }
    }


    public override void Display(int xOff, int yOff)
    {
      if (!IsAlive)
      {
        return;
      }

      base.Display(xOff, yOff);
    }


    public void Fire()
    {
      // TODO: Spawn bullets.
      cooldown = firingRate;
    }


    public void Die(EntityManager context)
    {
      if (!IsAlive)
      {
        return;
      }

      isAlive = false;
      context.StartGameOver();
      context.Add(new Explosion(X, Y));
    }


    public bool IsMovingLeft;

    public bool IsMovingRight;

    public bool IsShooting
    {
      get { return isShooting; }
      set
      {
        isShooting = value;
        cooldown = 0;
      }
    }

    public bool IsAlive { get { return isAlive; } }

    private bool isAlive = true;

    private bool isShooting;

    private int cooldown = 0;

    private const double speed = 4.0;
    private const int firingRate = 4;
    private const double minX = 0.0;
    private const double maxX = Shooter.pixelWidth - Shooter.spriteWidth;
  }


  class EntityManager
  {
    public void Update()
    {
      // Clone the entity list so the update operations can modify the
      // original list without breaking iteration.
      foreach (Entity e in new List<Entity>(Entities))
      {
        e.Update(this);
      }
    }


    public void Display(int xOff, int yOff)
    {
      foreach (Entity e in Entities)
      {
        e.Display(xOff, yOff);
      }
    }


    public void Add(Entity entity)
    {
      Entities.Add(entity);
    }


    public void Remove(Entity entity)
    {
      Entities.Remove(entity);
    }


    public void StartGameOver()
    {
      Shooter.StartGameOver();
    }


    public IList<Entity> Entities = new List<Entity>();
  }


  public class Shooter : IDisposable
  {
    public const int pixelWidth = 240;
    public const int pixelHeight = 320;

    public const int spriteWidth = 16;
    public const int spriteHeight = 16;

    const int fps = 30;

    static uint texture;

    static bool isRunning = true;

    static EntityManager entities = new EntityManager();

    static double timeStamp = CurrentSeconds;

    static Avatar avatar = new Avatar();

    static bool isGameOver = false;

    // How many ticks does the game keep going after game over.
    static int gameOverCounter = 40;


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

      entities.Add(avatar);
    }


    static void InitGlfw()
    {
      Glfw.glfwInit();
      Glfw.glfwOpenWindow(800, 600, 8, 8, 8, 8, 16, 0, Glfw.GLFW_WINDOW);
      Glfw.glfwSetWindowTitle("Behemoth Shooter");

      Glfw.glfwSetWindowSizeCallback(new Glfw.GLFWwindowsizefun(Resize));
      Glfw.glfwSetWindowCloseCallback(new Glfw.GLFWwindowclosefun(Close));
      Glfw.glfwSetKeyCallback(new Glfw.GLFWkeyfun(Keypress));

    }


    static void InitGl()
    {
      Gl.glEnable(Gl.GL_TEXTURE_2D);
      Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
      Gl.glClearColor(0f, 0.0f, 0.0f, 1f);
      Gl.glShadeModel(Gl.GL_SMOOTH);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
      Gl.glEnable(Gl.GL_BLEND);
    }


    static void Resize(int w, int h)
    {
      int x, y, width, height;
      Geom.MakeScaledViewport(
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


    public static void StartGameOver()
    {
      isGameOver = true;
    }


    static void MainLoop()
    {
      while (isRunning)
      {
        if (TimeToUpdate)
        {
          Update();
          Display();
        }
      }
    }


    static void Update()
    {
      entities.Update();
      if (isGameOver) {
        if (gameOverCounter-- <= 0) {
          isRunning = false;
        }
      }
    }


    static void Keypress(int key, int action)
    {
      if (action == Glfw.GLFW_PRESS)
      {
        switch (key)
        {
        case Glfw.GLFW_KEY_ESC:
          avatar.Die(entities);
          break;
        case 'E':
          int x, y;
          RandomPoint(out x, out y);

          entities.Add(new Explosion(x, y));
          break;
        }
      }

      switch (key)
      {
      case Glfw.GLFW_KEY_LEFT:
        avatar.IsMovingLeft = action == Glfw.GLFW_PRESS;
        break;
      case Glfw.GLFW_KEY_RIGHT:
        avatar.IsMovingRight = action == Glfw.GLFW_PRESS;
        break;
      }
    }


    static void Display()
    {
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadIdentity();

      double second = CurrentSeconds;

      DrawSprite(160 + (float)(100 * Math.Sin(second)), 120, (int)((second * 10) % 8));

      entities.Display(0, 0);

      Glfw.glfwSwapBuffers();
    }


    /// Return whether it's time for the next frame with the current FPS. A
    /// side effect of returning true is moving the frame tracking logic to
    /// the next frame, so make sure to cache the result.
    public static bool TimeToUpdate
    {
      get
      {
        double interval = 1.0 / fps;
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


    public static double CurrentSeconds { get { return (double)DateTime.Now.Ticks / 1e7; } }


    public static void RandomPoint(out int x, out int y)
    {
      Random rng = new Random();

      x = rng.Next(0, pixelWidth);
      y = rng.Next(0, pixelHeight);
    }


    public static void DrawSprite(float x, float y, int frame)
    {
      const int rows = 8;
      const int columns = 8;

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