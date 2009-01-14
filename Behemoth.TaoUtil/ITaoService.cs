using System;
using System.Collections.Generic;

using Behemoth.App;

namespace Behemoth.TaoUtil
{
  /// <summary>
  /// General Tao application utilities service.
  /// </summary>
  public interface ITaoService : IAppService
  {
    void Resize(int w, int h);

    void PixelProjection();

    int PixelWidth { get; }

    int PixelHeight { get; }

    double PixelScale { get; }

    TextureCache Textures { get; }

    bool UseSound { get; set; }
  }
}