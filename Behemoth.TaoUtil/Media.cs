using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using Tao.DevIl;
using Tao.OpenAl;
using Tao.OpenGl;
using Tao.PhysFs;

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


    /// <summary>
    /// Init the media handling facilities.
    /// </summary>
    public static void InitFacilities()
    {
      Il.ilInit();
      // XXX: Not really sure what the "init" parameter is for here, just copying the examples.
      Fs.PHYSFS_init("init");
      Alut.alutInit();
    }


    public static void UninitFacilities()
    {
      foreach (int buffer in soundBuffers.Values)
      {
        int buf = buffer;
        Al.alDeleteBuffers(1, ref buf);
      }
      soundBuffers.Clear();

      foreach (int source in soundSources)
      {
        int s = source;
        Al.alDeleteSources(1, ref s);
      }
      soundSources.Clear();

      Alut.alutExit();
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
    public static uint LoadGlTexture(string filename, int texFlags, int ilImageType)
    {
      int imageId = LoadImage(filename, ilImageType);
      ConvertToTextureImage(imageId);

      int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
      int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);

      uint result = MakeGlTexture(Il.ilGetData(), width, height, texFlags);

      DeleteImage(imageId);

      return result;
    }


    /// <summary>
    /// Make an OpenGL texture from a Png image loaded from PhysFS.
    /// </summary>
    public static uint LoadGlTexture(string filename, int texFlags)
    {
      return LoadGlTexture(filename, texFlags, Il.IL_PNG);
    }


    /// <summary>
    /// Make an OpenGL texture from pixel data.
    /// </summary>
    public static uint MakeGlTexture(IntPtr pixels, int w, int h, int texFlags)
    {
      uint textureHandle;

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
    /// Convert a DevIL image onto one that can be used as an OpenGL texture.
    /// </summary>
    public static void ConvertToTextureImage(int ilImageId)
    {
      Il.ilBindImage(ilImageId);

      // Flip to compensate for the Y axis flip when moving to OpenGL coordinates.
      Ilu.iluFlipImage();

      // Convert to 32 bits.
      Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE);
    }


    /// <summary>
    /// Load a DevIL image from a named PhysFS file.
    /// </summary>
    public static int LoadImage(string filename, int ilImageType)
    {
      byte[] data = GetPfsFileData(filename);

      int imageId;
      Il.ilGenImages(1, out imageId);
      Il.ilBindImage(imageId);

      if (!Il.ilLoadL(ilImageType, data, data.Length))
      {
        throw new IOException("Failed to load image "+filename);
      }

      Il.ilSave(Il.IL_PNG, "/tmp/test.png");

      return imageId;
    }


    /// <summary>
    /// Load a DevIL image from a named PhysFS Png file.
    /// </summary>
    public static int LoadImage(string filename)
    {
      return LoadImage(filename, Il.IL_PNG);
    }


    /// <summary>
    /// Delete a DevIL image.
    /// </summary>
    public static void DeleteImage(int imageId)
    {
      Il.ilDeleteImages(1, ref imageId);
    }

    // TODO: Freeing the buffers and sources for sounds.

    /// <summary>
    /// Load a sound from PhysFs into OpenAL.
    /// </summary>
    static int LoadSound(string filename)
    {
      int buffer;
      // Generate an OpenAL buffer
      Al.alGenBuffers(1, out buffer);
      if (Al.alGetError() != Al.AL_NO_ERROR) {
        throw new ApplicationException("Couldn't generate OpenAL buffer.");
      }

      int format;
      int size;
      float frequency;

      IntPtr data;
      long dataSize;

      GetPfsFileData(filename, out data, out dataSize);

      data = Alut.alutLoadMemoryFromFileImage(
        data, (int)dataSize,
        out format, out size, out frequency);

      if (data == IntPtr.Zero) {
        throw new IOException("Failed to load sound "+filename);
      }

      Al.alBufferData(buffer, format, data, size, (int)frequency);

      FreePfsData(data);

      soundBuffers[filename] = buffer;

      return buffer;
    }


    /// <summary>
    /// Play a sound using default settings.
    /// </summary>
    static void PlaySound(int buffer)
    {
      // Before we make new ones, clean up old sources that've stopped playing.

      // XXX: This might be better done in a thread, the connection between
      // clearing old sources and playing a new sound isn't terribly well
      // motivated.
      ClearStoppedSources();

      if (Al.alIsBuffer(buffer) == 0)
      {
        throw new ArgumentException("Not a valid OpenAL buffer.", "buffer");
      }

      int source;
      Al.alGenSources(1, out source);
      if (Al.alGetError() != Al.AL_NO_ERROR) {
        throw new ApplicationException("Couldn't generate OpenAL sound source.");
      }

      soundSources.Add(source);

      Al.alSourcei(source, Al.AL_BUFFER, buffer);
      Al.alSourcePlay(source);
    }


    /// <summary>
    /// Play a sound from a file, loading it first if necessary.
    /// </summary>
    public static void PlaySound(string filename)
    {
      if (!soundBuffers.ContainsKey(filename))
      {
        LoadSound(filename);
      }
      PlaySound(soundBuffers[filename]);
    }


    static bool IsStoppedSource(int source)
    {
      int state;
      Al.alGetSourcei(source, Al.AL_SOURCE_STATE, out state);
      return state == Al.AL_STOPPED;
    }


    static void ClearStoppedSources()
    {
      foreach (int source in new List<int>(soundSources))
      {
        if (IsStoppedSource(source))
        {
          int s = source;
          Al.alDeleteSources(1, ref s);
          soundSources.Remove(source);
        }
      }
    }


    /// <summary>
    /// Get the data from a PhysFS file as a pointer and a length.
    /// </summary>
    public static void GetPfsFileData(
      string filename, out IntPtr data, out long size)
    {
      if (Fs.PHYSFS_exists(filename) == 0)
      {
        throw new IOException("File "+filename+" not found.");
      }

      IntPtr file = Fs.PHYSFS_openRead(filename);
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
    public static byte[] GetPfsFileData(
      string filename)
    {
      if (Fs.PHYSFS_exists(filename) == 0)
      {
        throw new IOException("File "+filename+" not found.");
      }

      IntPtr file = Fs.PHYSFS_openRead(filename);
      long size = Fs.PHYSFS_fileLength(file);
      byte[] data;
      Fs.PHYSFS_read(file, out data, 1, (uint)size);
      Fs.PHYSFS_close(file);
      return data;
    }

    private static Dictionary<string, int> soundBuffers = new Dictionary<string, int>();
    private static List<int> soundSources = new List<int>();
  }
}