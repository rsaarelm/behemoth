using System;
using System.Collections.Generic;
using Tao.OpenGl;
using Tao.Sdl;
using Behemoth.Util;

namespace Behemoth.TaoUtil
{
  /// <summary>
  /// A cache for OpenGL textures. Built on top of an image cache.
  /// </summary>
  public class TextureCache : Cache<String, int>
  {
    public TextureCache(ImageCache imageCache, int texFlags)
    {
      this.imageCache = imageCache;
      this.texFlags = texFlags;
    }


    protected override int Load(string name)
    {
      IntPtr surfacePtr = imageCache[name];

      Sdl.SDL_Surface surface = Media.GetSdlSurface(surfacePtr);

      int result = Media.MakeGlTexture(
        surface.pixels,
        surface.w,
        surface.h,
        texFlags);

      return result;
    }


    protected override void Free(int item)
    {
      Gl.glDeleteTextures(1, new int[] { item });
    }


    public override void Dispose()
    {
      imageCache.Dispose();
      base.Dispose();
    }


    private ImageCache imageCache;

    private int texFlags;
  }
}