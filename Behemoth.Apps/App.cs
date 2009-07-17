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
        service = default(T);
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


    protected void UninitServices()
    {
      foreach (var serv in services.Values)
      {
        serv.Uninit();
      }
    }


    public void Run()
    {
      try
      {
        InitApp();
        AppMain();
      }
      finally
      {
        UninitApp();
        UninitServices();
      }
    }


    protected virtual void InitApp() {}
    

    /// <summary>
    /// The main function of the app, called from Run.
    /// </summary>
    protected virtual void AppMain() {}


    protected virtual void UninitApp() {}


    private IDictionary<Type, IAppService> services =
      new Dictionary<Type, IAppService>();

    private static App instance = null;
  }
}
