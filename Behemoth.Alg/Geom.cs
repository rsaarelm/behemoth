using System;
using System.Diagnostics;

namespace Behemoth.Alg
{
  /// <summary>
  /// Geometric utilities.
  /// </summary>
  public static class Geom
  {
    /// <description>
    /// Fit the largest centered (pixelWidth * scale, pixelHeight * scale)
    /// viewport with integer 'scale' into the given viewport. If the viewport
    /// is too small to for unit scale (pixelWidth, pixelHeight) viewport,
    /// scale the pixel viewport down into the largest non-integer fractional
    /// size that fits in the viewport.
    /// </description>
    public static void MakeScaledViewport(
      int pixelWidth, int pixelHeight,
      int viewportWidth, int viewportHeight,
      out int x, out int y, out int width, out int height)
    {
      Debug.Assert(
        pixelWidth > 0 && pixelHeight > 0 && viewportWidth > 0 && viewportHeight > 0,
        "Zero or negative viewport dimensions are not allowed.");
      double pixelAspect = (double)pixelWidth / (double)pixelHeight;
      double viewportAspect = (double)viewportWidth / (double)viewportHeight;
      int scale = Math.Min(viewportWidth / pixelWidth, viewportHeight / pixelHeight);
      if (scale == 0) {
        // Viewport is too small to do proper pixels, just fit the largest rect we can in it.
        if (viewportAspect > pixelAspect)
        {
          // Fit Y, adjust X
          width = (int)(pixelAspect * viewportHeight);
          x = viewportWidth / 2 - width / 2;
          height = viewportHeight;
          y = 0;
        }
        else
        {
          // Fit X, adjust Y
          height = (int)(viewportWidth / pixelAspect);
          y = viewportHeight / 2 - height / 2;
          width = viewportWidth;
          x = 0;
        }
      }
      else
      {
        width = scale * pixelWidth;
        height = scale * pixelHeight;

        x = viewportWidth / 2 - width / 2;
        y = viewportHeight / 2 - height / 2;
      }
    }


    public static bool RectanglesIntersect(
      int x1, int y1, int w1, int h1,
      int x2, int y2, int w2, int h2)
    {
      return !(x1 + w1 < x2 ||
               y1 + h1 < y2 ||
               x2 + w2 < x1 ||
               y2 + h2 < y2);
    }
  }
}