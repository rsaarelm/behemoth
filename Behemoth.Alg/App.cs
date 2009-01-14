using System;
using System.Collections.Generic;
using System.Linq;

namespace Behemoth.Alg
{
  /// <summary>
  /// Component-based Application class.
  /// </summary>
  public class App
  {
    public App()
    {
      if (App.instance != null)
      {
        throw new ApplicationException("Trying to instantiate multiple Apps.");
      }
      App.instance = this;
    }


    public static App Instance { get { return instance; } }


    public T GetService<T>() where T : IAppService
    {
      return (T)services[typeof(T)];
    }


    public void Add(AppComponent component)
    {
      if (isRunning)
      {
        // XXX The current sequence is to add all components before the app is
        // started and initialize the components when the app starts running.
        // Since the initialization doesn't get called any more when the app
        // is running, attempts to add components when running cause an
        // exception. Might change this later if I need more flexible
        // components.
        throw new ApplicationException("Can't add components when app is running.");
      }
      components.Add(component);
      component.App = this;
    }


    public void RegisterService(Type service, IAppService provider)
    {
      if (service.GetInterface("Behemoth.Alg.IAppService") == null)
      {
        throw new ArgumentException(
          "Service type doesn't implement service base interface.",
          "service");
      }
      if (!service.IsInstanceOfType(provider))
      {
        throw new ArgumentException(
          "Provider does not implement the specified interface.",
          "provider");
      }

      services[service] = provider;
    }


    public void Run()
    {
      isRunning = true;

      Init();

      foreach (AppComponent c in components)
      {
        c.Init();
      }

      double lastTime = CurrentSeconds;
      try
      {
        while (isRunning)
        {
          if (CurrentSeconds >= lastTime + TargetUpdateInterval)
          {
            Update(TargetUpdateInterval);
            Draw(TargetUpdateInterval);
            lastTime += TargetUpdateInterval;
          }
        }
      }
      finally
      {
        foreach (AppComponent c in components)
        {
          c.Uninit();
        }

        Uninit();

        isRunning = false;
      }
    }


    public void Exit()
    {
      isRunning = false;
    }


    /// <summary>
    /// Targeted frame rate in seconds.
    /// </summary>
    public double TargetUpdateInterval = 1.0 / 30.0;


    public static double CurrentSeconds
    { get { return (double)DateTime.Now.Ticks / 1e7; } }


    public int Tick { get { return tick; } }


    protected virtual void Update(double timeElapsed)
    {
      var updatees = from c in components
        where c.Enabled
        orderby c.UpdateOrder
        select c;

      foreach (var c in updatees)
      {
        c.Update(timeElapsed);
      }

      tick++;
    }


    protected virtual void Draw(double timeElapsed)
    {
      var drawees = from c in components
        where c is DrawableAppComponent && ((DrawableAppComponent)c).Visible
        orderby ((DrawableAppComponent)c).DrawOrder
        select (DrawableAppComponent)c;

      foreach (DrawableAppComponent c in drawees)
      {
        c.Draw(timeElapsed);
      }
    }


    protected virtual void Init() {}


    protected virtual void Uninit() {}


    private List<AppComponent> components = new List<AppComponent>();

    private IDictionary<Type, IAppService> services =
      new Dictionary<Type, IAppService>();

    private bool isRunning = false;

    private int tick = 0;


    private static App instance = null;
  }
}