using System;
using System.Collections.Generic;

using Behemoth.Apps;
using Behemoth.Util;

namespace Behemoth.TaoUtil
{
  /// <summary>
  /// General Tao application utilities service.
  /// </summary>
  public interface ITaoService : IAppService
  {
    void Resize(int w, int h);

    void FlipScreen();

    void PixelProjection();

    void AddTexture(string name, Color[,] pixels);

    int PixelWidth { get; }

    int PixelHeight { get; }

    double PixelScale { get; }

    TextureCache Textures { get; }

    bool UseSound { get; set; }
  }
}