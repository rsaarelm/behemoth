using System;
using System.Collections.Generic;
using System.Linq;

using Behemoth.Util;

namespace Behemoth.Apps
{
  /// <summary>
  /// Component-based App class.
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


    /// <summary>
    /// Get the singleton App instance.
    /// </summary>
    public static App Instance { get { return instance; } }


    /// <summary>
    /// Shortcut for App.Instance.GetService<T>().
    /// </summary>
    public static T Service<T>() where T : IAppService
    {
      return Instance.GetService<T>();
    }


    public T GetService<T>() where T : IAppService
    {
      return (T)services[typeof(T)];
    }


    public bool TryGetService<T>(out T service) where T : IAppService
    {
      IAppService s;
      if (services.TryGetValue(typeof(T), out s))
      {
        service = (T)s;
        return true;
      }
      else
      {
        return false;
      }
    }


    public bool ContainsService(Type serviceType)
    {
      return services.ContainsKey(serviceType);
    }


    public void RegisterService(Type service, IAppService provider)
    {
      if (service.GetInterface("Behemoth.Apps.IAppService") == null)
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

      IAppService oldService;

      if (services.TryGetValue(service, out oldService))
      {
        oldService.Uninit();
      }

      services[service] = provider;

      provider.Init();
    }


    public void Run()
    {
      isRunning = true;

      InitApp();

      double lastTime = TimeUtil.CurrentSeconds;
      try
      {
        while (isRunning)
        {
          if (TimeUtil.CurrentSeconds >= lastTime + TargetUpdateInterval)
          {
            Update(TargetUpdateInterval);
            Draw(TargetUpdateInterval);
            lastTime += TargetUpdateInterval;
          }
        }
      }
      finally
      {
        foreach (var serv in services.Values)
        {
          serv.Uninit();
        }

        UninitApp();

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


    public int Tick { get { return tick; } }


    protected virtual void Update(double timeElapsed)
    {
      IScreen screen;
      if (TryGetService(out screen))
      {
        screen.Update(timeElapsed);
      }

      tick++;
    }


    protected virtual void Draw(double timeElapsed)
    {
      IScreen screen;
      if (TryGetService(out screen))
      {
        screen.Draw(timeElapsed);
      }
    }


    protected virtual void InitApp() {}


    protected virtual void UninitApp() {}


    private IDictionary<Type, IAppService> services =
      new Dictionary<Type, IAppService>();

    private bool isRunning = false;

    private int tick = 0;


    private static App instance = null;
  }
}