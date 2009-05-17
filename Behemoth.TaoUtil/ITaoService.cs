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
    /// <summary>
    /// Resize the application window.
    /// </summary>
    void Resize(int w, int h);

    /// <summary>
    /// Display graphics drawn into the back buffer on the visible screen.
    /// </summary>
    void FlipScreen();

    /// <summary>
    /// Helper function for reading the SDL event queue and calling the
    /// relevant delegate. The keyDown callback takes the keycode, the key
    /// modifier mask and the printable key char (if any). The keyUp callback
    /// only takes the keycode. Keycodes and key modifier masks are from the
    /// SDL event structure. The callbacks can be null, in which case they are
    /// ignored.
    /// </summary>
    /// <return>
    /// True if an event was processed from the event queue (whether it
    /// triggered a callback or not), false if the event queue was empty.
    /// </return>
    bool HandleInput(
      Action<int, int, char> keyDownCallback,
      Action<int> keyUpCallback,
      Action quitCallback);

    /// <summary>
    /// A 2d projection where 1 unit corresponds to 1 pixel in the desired
    /// pixel dimensions.
    /// </summary>
    void PixelProjection();

    /// <summary>
    /// Add a procedurally generated texture with a given name to the texture
    /// cache.
    /// </summary>
    void AddTexture(string name, Color[,] pixels);

    /// <summary>
    /// The logical pixel width.
    /// </summary>
    int PixelWidth { get; }

    /// <summary>
    /// The logical pixel height.
    /// </summary>
    int PixelHeight { get; }

    /// <summary>
    /// The scale of the logical pixels in physical pixels.
    /// </summary>
    double PixelScale { get; }

    TextureCache Textures { get; }

    bool UseSound { get; set; }
  }
}