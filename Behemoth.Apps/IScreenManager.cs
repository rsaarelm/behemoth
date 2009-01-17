using System;

namespace Behemoth.Apps
{
  public interface IScreenManager : IAppService
  {
    void PushScreen(IScreen screen);

    IScreen PopScreen();

    IScreen SwapScreen(IScreen screen);
  }
}