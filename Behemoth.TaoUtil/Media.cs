using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;


using Tao.OpenGl;
using Tao.PhysFs;
using Tao.Sdl;

namespace Behemoth.TaoUtil
{
  /// <summary>
  /// Dealing with media assets using Tao.
  /// </summary>
  public static class Media
  {
    // Texture flags.
    public const int TEX_CLAMP_EDGE = 1 << 0;
    public const int TEX_USE_FILTERING = 1 << 1;

    // Settings
    public static bool UseSound = true;

    /// <summary>
    /// Init the media handling facilities.
    /// </summary>
    public static void InitFacilities()
    {
      // XXX: Not really sure what the "init" parameter is for here, just copying the examples.
      Fs.PHYSFS_init("init");

      if (UseSound)
      {
        if (SdlMixer.Mix_OpenAudio(44100, (short)SdlMixer.MIX_DEFAULT_FORMAT, 2, 512) != 0)
        {
          Console.WriteLine("Couldn't start audio.");
          UseSound = false;
        }
      }
    }


    public static void UninitFacilities()
    {
      foreach (IntPtr buffer in soundBuffers.Values)
      {
        SdlMixer.Mix_FreeChunk(buffer);
      }
      soundBuffers.Clear();
    }


    public static void AddPhysFsPath(params string[] elts)
    {
      // Always append to the end of path.
      int append = 1;

      if (elts.Length == 0)
      {
        return;
      }
      else if (elts.Length == 1)
      {
        Fs.PHYSFS_addToSearchPath(elts[0], append);
      }
      else
      {
        string path = null;
        for (int i = 1; i < elts.Length; i++)
        {
          path = Path.Combine((i == 1 ? elts[0] : path), elts[i]);
        }
        Fs.PHYSFS_addToSearchPath(path, append);
      }
    }


    /// <summary>
    /// Make an OpenGL texture from an image loaded from PhysFS.
    /// </summary>
    public static int LoadGlTexture(string filename, int texFlags)
    {
      IntPtr texturePtr =
        SdlImage.IMG_Load_RW(GetPfsFileRwop(filename), 1);

      IntPtr texture32BitPtr = SdlSurfaceTo32Bit(texturePtr);
      Sdl.SDL_FreeSurface(texturePtr);

      Sdl.SDL_Surface texture = GetSdlSurface(texture32BitPtr);

      int result = MakeGlTexture(
        texture.pixels, texture.w, texture.h, texFlags);

      Sdl.SDL_FreeSurface(texture32BitPtr);

      return result;
    }


    /// <summary>
    /// Make an OpenGL texture from pixel data.
    /// </summary>
    public static int MakeGlTexture(IntPtr pixels, int w, int h, int texFlags)
    {
      int textureHandle;

      var clamp = (texFlags & TEX_CLAMP_EDGE) != 0 ? Gl.GL_CLAMP : Gl.GL_REPEAT;
      var filtering = (texFlags & TEX_USE_FILTERING) != 0 ? Gl.GL_LINEAR : Gl.GL_NEAREST;

      // Setup new texture.
      Gl.glGenTextures(1, out textureHandle);
      Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle);

      // Set texture parameters.
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, clamp);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, clamp);

      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, filtering);
      Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, filtering);

      // Create the texture.
      Gl.glTexImage2D(
        Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0,
        Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);

      return textureHandle;
    }


    /// <summary>
    /// Load a sound from PhysFs into SDL Mixer.
    /// </summary>
    static IntPtr LoadSound(string filename)
    {
      IntPtr rwop = GetPfsFileRwop(filename);
      IntPtr chunk = SdlMixer.Mix_LoadWAV_RW(rwop, 1);
      if (chunk == IntPtr.Zero) {
        throw new ApplicationException("Error loading sound "+filename);
      }
      return chunk;
    }


    /// <summary>
    /// Play a sound using SDL Mixer.
    /// </summary>
    static void PlaySound(IntPtr chunk)
    {
      SdlMixer.Mix_PlayChannel(-1, chunk, 0);
    }


    /// <summary>
    /// Play a sound from a file, loading it first if necessary.
    /// </summary>
    public static void PlaySound(string filename)
    {
      if (!UseSound)
      {
        return;
      }

      if (!soundBuffers.ContainsKey(filename))
      {
        soundBuffers[filename] = LoadSound(filename);
      }
      PlaySound(soundBuffers[filename]);
    }


    /// <summary>
    /// Get the data from a PhysFS file as a pointer and a length.
    /// </summary>
    public static void GetPfsFileData(
      string filename, out IntPtr data, out long size)
    {
      IntPtr file = PfsOpenRead(filename);
      size = Fs.PHYSFS_fileLength(file);
      Fs.PHYSFS_read(file, out data, 1, (uint)size);
      Fs.PHYSFS_close(file);
    }


    public static void FreePfsData(IntPtr data)
    {
      Marshal.FreeHGlobal(data);
    }


    /// <summary>
    /// Get the data from a PhysFS file as a byte array.
    /// </summary>
    public static byte[] GetPfsFileData(string filename)
    {
      IntPtr file = PfsOpenRead(filename);
      long size = Fs.PHYSFS_fileLength(file);
      byte[] data;
      Fs.PHYSFS_read(file, out data, 1, (uint)size);
      Fs.PHYSFS_close(file);
      return data;
    }


    /// <summary>
    /// Get the data from a PhysFS file as an string.
    /// </summary>
    public static string GetPfsFileText(string filename, Encoding encoding)
    {
      return encoding.GetString(GetPfsFileData(filename));
    }


    /// <summary>
    /// Get the data from a PhysFS file as an UTF-8 string.
    /// </summary>
    public static string GetPfsFileText(string filename)
    {
      return GetPfsFileText(filename, Encoding.UTF8);
    }



    /// <summary>
    /// Load a PhysFS file into a SDL RWops structure.
    /// </summary>
    public static IntPtr GetPfsFileRwop(string filename)
    {
      byte[] data = GetPfsFileData(filename);
      return Sdl.SDL_RWFromMem(data, data.Length);
    }


    /// <summary>
    /// Open a PhysFS file for reading.
    /// </summary>
    private static IntPtr PfsOpenRead(string filename)
    {
      if (Fs.PHYSFS_exists(filename) == 0)
      {
        throw new IOException("File "+filename+" not found.");
      }

      IntPtr file = Fs.PHYSFS_openRead(filename);

      return file;
    }


    public static Sdl.SDL_Surface GetSdlSurface(IntPtr surfacePtr)
    {
      return (Sdl.SDL_Surface)Marshal.PtrToStructure(surfacePtr, typeof(Sdl.SDL_Surface));
    }


    public static Sdl.SDL_PixelFormat GetPixelFormat(Sdl.SDL_Surface surface)
    {
      return (Sdl.SDL_PixelFormat)Marshal.PtrToStructure(surface.format, typeof(Sdl.SDL_PixelFormat));
    }


    /// <summary>
    /// Convert a SDL surface into 32 bit format. Frees the old surface and
    /// allocates a new one.
    /// </summary>
    public unsafe static IntPtr SdlSurfaceTo32Bit(IntPtr sdlSurfacePtr)
    {
      Sdl.SDL_PixelFormat format = PixelFormat32Bit;
      return Sdl.SDL_ConvertSurface(
        sdlSurfacePtr,
        (IntPtr)(&format),
        0);
    }


    private static Sdl.SDL_PixelFormat PixelFormat32Bit
    {
      get
      {
        byte rloss = 0, gloss = 0, bloss = 0, aloss = 0;
        byte rshift, gshift, bshift, ashift;
        uint rmask, gmask, bmask, amask;

        byte bitsPerPixel = 32;
        byte bytesPerPixel = 4;
        int colorkey = 0;
        byte alpha = 0;

        if (Sdl.SDL_BYTEORDER == Sdl.SDL_BIG_ENDIAN)
        {
          rshift = 24;
          gshift = 16;
          bshift = 8;
          ashift = 0;

          rmask = 0xff000000;
          gmask = 0x00ff0000;
          bmask = 0x0000ff00;
          amask = 0x000000ff;
        }
        else
        {
          rshift = 0;
          gshift = 8;
          bshift = 16;
          ashift = 24;

          rmask = 0x000000ff;
          gmask = 0x0000ff00;
          bmask = 0x00ff0000;
          amask = 0xff000000;
        }

        var result = new Sdl.SDL_PixelFormat(
          IntPtr.Zero,
          bitsPerPixel, bytesPerPixel,
          rloss, gloss, bloss, aloss,
          rshift, gshift, bshift, ashift,
          (int)rmask, (int)gmask, (int)bmask, (int)amask,
          colorkey,
          alpha);

        return result;
      }
    }


    private static Dictionary<string, IntPtr> soundBuffers =
      new Dictionary<string, IntPtr>();
  }
}