namespace Behemoth.Apps
{
  public interface IAppService
  {
    /// <summary>
    /// Method called when the service is registered to an app.
    /// </summary>
    void Init();


    /// <summary>
    /// Method called when the service is removed from an app.
    /// </summary>
    void Uninit();
  }
}