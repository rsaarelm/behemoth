using System;
using Tao.Sdl;
using Behemoth.Util;

namespace Behemoth.TaoUtil
{
  /// <summary>
  /// A cache class for image data. Currently based on SDL surfaces.
  /// </summary>
  public class ImageCache : Cache<String, IntPtr>
  {
    protected override IntPtr Load(string name)
    {
      // Load the initial data.
      IntPtr imagePtr =
        SdlImage.IMG_Load_RW(Media.GetPfsFileRwop(name), 1);

      // Make a copy that's converted to a texture-friendly format.
      IntPtr result = Media.SdlSurfaceTo32Bit(imagePtr);

      // Free the initial data.
      Sdl.SDL_FreeSurface(imagePtr);

      return result;
    }


    protected override void Free(IntPtr item)
    {
      Sdl.SDL_FreeSurface(item);
    }


    protected override long Size(IntPtr item)
    {
      Sdl.SDL_Surface surface = Media.GetSdlSurface(item);
      return surface.pitch * surface.h * Media.GetPixelFormat(surface).BytesPerPixel;
    }

  }
}