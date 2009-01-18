using System;

namespace Behemoth.Apps
{
  /// <summary>
  /// A special service that implements the toplevel game screen.
  /// </summary>
  public interface IScreen : IAppService
  {
    void Draw(double timeElapsed);

    void Update(double timeElapsed);
  }
}