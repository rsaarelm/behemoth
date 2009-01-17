using System;

namespace Behemoth.Apps
{
  public interface IScreen
  {
    void Draw(double timeElapsed);
    void Update(double timeElapsed);
  }
}