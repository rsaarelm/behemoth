using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Alg;
using Behemoth.TaoUtil;

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


    public void SetHitBox(int x, int y, int w, int h)
    {
      hitBoxX = x;
      hitBoxY = y;
      hitBoxW = w;
      hitBoxH = h;
    }


    public bool Intersects(Entity other)
    {
      return Geom.RectanglesIntersect(
        hitBoxX + X, hitBoxY + Y, hitBoxW, hitBoxH,
        other.hitBoxX + other.X, other.hitBoxY + other.Y, other.hitBoxW, other.hitBoxH);
    }


    public double X;
    public double Y;
    public int Frame;

    private int hitBoxX = 0;
    private int hitBoxY = 0;
    private int hitBoxW = Shooter.spriteWidth;
    private int hitBoxH = Shooter.spriteHeight;
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

      SetHitBox(6, 5, 4, 7);
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
          Fire(context);
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


    public void Fire(EntityManager context)
    {
      Media.PlaySound(Shooter.playerShotFx);
      context.Add(new AvatarShot(X - 4, Y + 3));
      context.Add(new AvatarShot(X + 5, Y + 3));
      cooldown = firingRate;
    }


    public void Die(EntityManager context)
    {
      if (!IsAlive)
      {
        return;
      }

      Media.PlaySound(Shooter.playerExplodeFx);
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
    private const int firingRate = 2;
    private const double minX = 0.0;
    private const double maxX = Shooter.pixelWidth - Shooter.spriteWidth;
  }


  class AvatarShot : Entity
  {
    public AvatarShot(double x, double y)
    {
      X = x;
      Y = y;
      Frame = 12;
      SetHitBox(6, 6, 3, 4);
    }

    public override void Update(EntityManager context)
    {
      foreach (Entity o in new List<Entity>(context.Entities))
      {
        var enemy = o as Enemy;
        if (enemy != null && this.Intersects(enemy))
        {
          enemy.Hurt(context);
        }
      }

      Y += speed;

      if (Y > Shooter.pixelHeight + Shooter.spriteHeight)
      {
        context.Remove(this);
      }
    }

    private const double speed = 5.0;
  }


  class Enemy : Entity
  {
    public Enemy(double x, double y, double dx, double dy)
    {
      X = x;
      Y = y;
      DX = dx;
      DY = dy;
    }


    public override void Display(int xOffset, int yOffset)
    {
      Shooter.DrawSprite(
        (int)X + xOffset, (int)Y + yOffset,
        damageBlink > 1 ? blinkFrame : Frame);

      if (damageBlink > 0)
      {
        damageBlink--;
      }
    }


    public override void Update(EntityManager context)
    {
      // Collision detection.

      // XXX: Wasteful iterating through all, could optimize by having
      // collision groups as sublists.

      // XXX: Repeating the iteration pattern from EntityManager...
      foreach (Entity o in new List<Entity>(context.Entities))
      {
        var avatar = o as Avatar;
        if (avatar != null && this.Intersects(avatar))
        {
          avatar.Die(context);
        }
      }

      X += DX;
      Y += DY;

      if (HasLeftStage) {
        context.Remove(this);
      }

      frameCount = (frameCount + 1) % (numFrames * frameDelay);
      Frame = startFrame + (frameCount / frameDelay);
    }


    public void Hurt(EntityManager context)
    {
      if (life-- < 0)
      {
        Die(context);
      }
      if (damageBlink == 0)
      {
        damageBlink = 2;
      }
    }


    public void Die(EntityManager context)
    {
      Media.PlaySound(Shooter.enemyExplodeFx);

      context.Add(new Explosion(X, Y));
      context.Remove(this);
    }


    bool HasLeftStage { get { return Y < -Shooter.spriteHeight; } }


    public double DX;
    public double DY;

    private int frameCount = 0;

    private int life = 10;

    private int damageBlink = 0;

    const int startFrame = 0;
    const int numFrames = 8;
    const int frameDelay = 4;
    const int blinkFrame = 14;
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


  public class Shooter
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

    static Random rng = new Random();

    static List<Vec3<double>> starfield = new List<Vec3<double>>();

    static double pixelScale;

    public const string playerShotFx = "pew.wav";
    public const string playerExplodeFx = "player_explode.wav";
    public const string enemyExplodeFx = "enemy_explode.wav";


    public static void Main(string[] args)
    {
      Init();

      MainLoop();
    }


    static void Init()
    {
      Sdl.SDL_Init(Sdl.SDL_INIT_EVERYTHING);
      Media.InitFacilities();
      Media.AddPhysFsPath("Shooter.zip");

      // Make the zip file found from build subdir too, so that it's easy to
      // run the exe from the project root dir.
      Media.AddPhysFsPath("build", "Shooter.zip");

      InitSdl();

      InitGl();

      texture = Media.LoadGlTexture("sprites.png", 0);

      entities.Add(avatar);

      InitStarfield();
    }


    static void InitSdl()
    {
      Sdl.SDL_WM_SetCaption("Behemoth Shooter", "");
      Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DOUBLEBUFFER, 1);

      Resize(800, 600);
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

      Sdl.SDL_SetVideoMode(w, h, 32, Sdl.SDL_RESIZABLE | Sdl.SDL_OPENGL);

      Geom.MakeScaledViewport(
        pixelWidth, pixelHeight, w, h, out x, out y, out width, out height);

      pixelScale = width / pixelWidth;

      Gl.glViewport(x, y, width, height);

      Gl.glMatrixMode(Gl.GL_PROJECTION);
      Gl.glLoadIdentity();
      Glu.gluOrtho2D(0, pixelWidth, 0, pixelHeight);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
    }


    static void InitStarfield()
    {
      for (int i = 0; i < 1000; i++)
      {
        starfield.Add(new Vec3<double>(rng.Next(-pixelWidth, pixelWidth), rng.Next(1000), rng.Next(100)));
      }
    }


    public static void StartGameOver()
    {
      isGameOver = true;
    }


    static void MainLoop()
    {
      try {
        while (isRunning)
        {
          ReadInput();
          if (TimeToUpdate)
          {
            Update();
            Display();
          }
        }
      }
      finally
      {
        Media.UninitFacilities();
        Sdl.SDL_Quit();
      }
    }


    static void ReadInput()
    {
      Sdl.SDL_Event evt;

      while (Sdl.SDL_PollEvent(out evt) != 0)
      {
        switch (evt.type)
        {
        case Sdl.SDL_QUIT:
          Quit();
          break;

        case Sdl.SDL_KEYDOWN:
          switch (evt.key.keysym.sym)
          {
          case Sdl.SDLK_ESCAPE:
            Quit();
            break;
          case Sdl.SDLK_LEFT:
            avatar.IsMovingLeft = true;
            break;
          case Sdl.SDLK_RIGHT:
            avatar.IsMovingRight = true;
            break;
          case Sdl.SDLK_SPACE:
            avatar.IsShooting = true;
            break;
          }
          break;

        case Sdl.SDL_KEYUP:
          switch (evt.key.keysym.sym)
          {
          case Sdl.SDLK_LEFT:
            avatar.IsMovingLeft = false;
            break;
          case Sdl.SDLK_RIGHT:
            avatar.IsMovingRight = false;
            break;
          case Sdl.SDLK_SPACE:
            avatar.IsShooting = false;
            break;
          }
          break;

        case Sdl.SDL_VIDEORESIZE:
          Resize(evt.resize.w, evt.resize.h);
          break;

        case Sdl.SDL_VIDEOEXPOSE:
          // Might want some kind of repaint here?
          break;
        }
      }
    }


    static void Quit()
    {
      isRunning = false;
    }


    static void Update()
    {
      entities.Update();
      if (isGameOver) {
        if (gameOverCounter-- <= 0) {
          isRunning = false;
        }
      }

      if (rng.Next(5) == 0)
      {
        SpawnEnemy();
      }
    }


    static void Display()
    {
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadIdentity();

      DrawStarfield(starfield, CurrentSeconds * 100);

      entities.Display(0, 0);

      Sdl.SDL_GL_SwapBuffers();
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
      x = rng.Next(0, pixelWidth - spriteWidth);
      y = rng.Next(0, pixelHeight - spriteHeight);
    }


    public static void SpawnEnemy()
    {
      double x = pixelWidth / 2 - spriteWidth / 2;
      x += (rng.NextDouble() - 0.5) * pixelWidth * 2.0;

      double dx = 6.0 * (rng.NextDouble() - 0.5);
      entities.Add(new Enemy(x, pixelHeight + spriteHeight, dx, -4.0));
    }


    public static void DrawSprite(float x, float y, int frame)
    {
      const int rows = 8;
      const int columns = 8;

      float x0 = (float)(frame % columns) / (float)columns;
      float y0 = 1.0f - (float)((frame + columns) / rows) / (float)rows;

      float x1 = x0 + 1.0f / (float)columns;
      float y1 = y0 + 1.0f / (float)rows;

      Gl.glColor3f(1.0f, 1.0f, 1.0f);

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


    /// <summary>
    /// Draw a vertically scrolling starfield
    /// </summary>
    public static void DrawStarfield(IEnumerable<Vec3<double>> points, double t)
    {
      const double span = 1000.0;
      const float depthFactor = 100.0f;


      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

      Gl.glColor3f(1.0f, 1.0f, 1.0f);

      // TODO: Do we need point scaling?

      Gl.glPointSize((float)pixelScale);

      // TODO: Apply offset
      Gl.glBegin(Gl.GL_POINTS);

      foreach (Vec3<double> point in points)
      {
        Gl.glVertex3f(
          (float)(pixelWidth / 2 + point.X) * depthFactor / (depthFactor + (float)point.Z),
          (float)(point.Y - t % span) * depthFactor / (depthFactor + (float)point.Z),
          0.0f);
        Gl.glVertex3f(
          (float)(pixelWidth / 2 + point.X) * depthFactor / (depthFactor + (float)point.Z),
          (float)(point.Y + span - t % span) * depthFactor / (depthFactor + (float)point.Z),
          0.0f);
      }

      Gl.glEnd();
    }
  }
}