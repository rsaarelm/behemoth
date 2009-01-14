using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

using Behemoth.Util;
using Behemoth.App;
using Behemoth.TaoUtil;

namespace Shooter
{
  abstract class Entity
  {
    public virtual void Display(Shooter shooter, int xOffset, int yOffset)
    {
      shooter.DrawSprite((int)X + xOffset, (int)Y + yOffset, Frame);
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


    public override void Display(Shooter shooter, int xOff, int yOff)
    {
      if (!IsAlive)
      {
        return;
      }

      base.Display(shooter, xOff, yOff);
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


    public override void Display(Shooter shooter, int xOffset, int yOffset)
    {
      shooter.DrawSprite(
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
    public EntityManager(Shooter shooter)
    {
      ShooterApp = shooter;
    }


    public void Update()
    {
      // Clone the entity list so the update operations can modify the
      // original list without breaking iteration.
      foreach (Entity e in new List<Entity>(Entities))
      {
        e.Update(this);
      }
    }


    public void Display(Shooter shooter, int xOff, int yOff)
    {
      foreach (Entity e in Entities)
      {
        e.Display(shooter, xOff, yOff);
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
      ShooterApp.StartGameOver();
    }


    public IList<Entity> Entities = new List<Entity>();

    public Shooter ShooterApp;
  }


  public class Shooter : DrawableAppComponent
  {
    public const int pixelWidth = 240;
    public const int pixelHeight = 320;

    public const int spriteWidth = 16;
    public const int spriteHeight = 16;

    public const string playerShotFx = "pew.wav";
    public const string playerExplodeFx = "player_explode.wav";
    public const string enemyExplodeFx = "enemy_explode.wav";


    const string spriteTexture = "sprites.png";

    EntityManager entities;

    Avatar avatar = new Avatar();

    bool isGameOver = false;

    // How many ticks does the game keep going after game over.
    int gameOverCounter = 40;

    Random rng = new Random();

    List<Vec3> starfield = new List<Vec3>();

    public static void Main(string[] args)
    {
      var app = new TaoApp(pixelWidth, pixelHeight, "Behemoth Shooter");
      app.Add(new Shooter());
      app.Run();
    }


    public override void Init()
    {
      Media.AddPhysFsPath("Shooter.zip");
      // Make the zip file found from build subdir too, so that it's easy to
      // run the exe from the project root dir.
      Media.AddPhysFsPath("build", "Shooter.zip");

      entities = new EntityManager(this);

      entities.Add(avatar);

      InitStarfield();

    }



    void InitStarfield()
    {
      for (int i = 0; i < 1000; i++)
      {
        starfield.Add(new Vec3(rng.Next(-pixelWidth, pixelWidth), rng.Next(1000), rng.Next(100)));
      }
    }


    public void StartGameOver()
    {
      isGameOver = true;
    }


    void ReadInput()
    {
      Sdl.SDL_Event evt;

      while (Sdl.SDL_PollEvent(out evt) != 0)
      {
        switch (evt.type)
        {
        case Sdl.SDL_QUIT:
          App.Exit();
          break;

        case Sdl.SDL_KEYDOWN:
          switch (evt.key.keysym.sym)
          {
          case Sdl.SDLK_ESCAPE:
            App.Exit();
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
          App.GetService<ITaoService>().Resize(evt.resize.w, evt.resize.h);
          break;

        case Sdl.SDL_VIDEOEXPOSE:
          // Might want some kind of repaint here?
          break;
        }
      }
    }


    public override void Update(double timeElapsed)
    {
      ReadInput();
      entities.Update();
      if (isGameOver) {
        if (gameOverCounter-- <= 0) {
          App.Exit();
        }
      }

      if (rng.Next(5) == 0)
      {
        SpawnEnemy();
      }
    }


    public override void Draw(double timeElapsed)
    {
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
      Gl.glMatrixMode(Gl.GL_MODELVIEW);
      Gl.glLoadIdentity();

      Gfx.DrawStarfield(starfield, TimeUtil.CurrentSeconds * 100,
                        App.GetService<ITaoService>().PixelScale,
                        App.GetService<ITaoService>().PixelWidth);

      entities.Display(this, 0, 0);

      Sdl.SDL_GL_SwapBuffers();
    }


    public void RandomPoint(out int x, out int y)
    {
      x = rng.Next(0, pixelWidth - spriteWidth);
      y = rng.Next(0, pixelHeight - spriteHeight);
    }


    public void SpawnEnemy()
    {
      double x = pixelWidth / 2 - spriteWidth / 2;
      x += (rng.NextDouble() - 0.5) * pixelWidth * 2.0;

      double dx = 6.0 * (rng.NextDouble() - 0.5);
      entities.Add(new Enemy(x, pixelHeight + spriteHeight, dx, -4.0));
    }


    public void DrawSprite(float x, float y, int frame)
    {
      Gfx.DrawSprite(
        x, y, frame, spriteWidth, spriteHeight,
        App.GetService<ITaoService>().Textures[spriteTexture], 8, 8);
    }
  }
}