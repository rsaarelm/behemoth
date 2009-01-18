using System;
using System.Collections.Generic;

namespace Behemoth.Apps
{
  public class ScreenManager : IScreen, IScreenManager
  {
    public ScreenManager(params IScreen[] screens)
    {
      foreach (var screen in screens)
      {
        PushScreen(screen);
      }
    }


    public void Register(App app)
    {
      app.RegisterService(typeof(IScreen), this);
      app.RegisterService(typeof(IScreenManager), this);
    }


    public void Init() {}


    public void Uninit() {}


    public void Update(double timeElapsed)
    {
      if (screens.Count > 0)
      {
        screens.Peek().Update(timeElapsed);
      }
    }


    public void Draw(double timeElapsed)
    {
      if (screens.Count > 0)
      {
        screens.Peek().Draw(timeElapsed);
      }
    }


    public void PushScreen(IScreen screen)
    {
      screens.Push(screen);
    }


    public IScreen PopScreen()
    {
      return screens.Pop();
    }


    public IScreen SwapScreen(IScreen screen)
    {
      var result = PopScreen();
      PushScreen(screen);
      return result;
    }


    Stack<IScreen> screens = new Stack<IScreen>();
  }
}