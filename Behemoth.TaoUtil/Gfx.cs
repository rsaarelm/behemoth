using System;
using System.Collections.Generic;
using Tao.OpenGl;
using Behemoth.Alg;

namespace Behemoth.TaoUtil
{
  /// <summary>
  /// Graphics utility functions.
  /// </summary>
  public static class Gfx
  {
    /// <summary>
    /// Draw a sprite from a sheet of sprites as a texture mapped quad.
    /// </summary>
    public static void DrawSprite(
      double x, double y, int frame,
      double spriteWidth, double spriteHeight,
      int sheetTexture, int sheetRows, int sheetColumns,
      int xFlip, int yFlip, Color color)
    {
      float x0 = (float)(frame % sheetColumns) / (float)sheetColumns + (float)xFlip / (float)sheetColumns;
      float y0 = (float)((sheetColumns + frame) / sheetRows) / (float)sheetRows - (float)yFlip / (float)sheetRows;

      float x1 = x0 + (1.0f - 2.0f * xFlip) / (float)sheetColumns;
      float y1 = y0 - (1.0f - 2.0f * yFlip) / (float)sheetRows;

      GlColor(color);

      Gl.glPushMatrix();

      Gl.glTranslatef((float)x, (float)y, 0.0f);

      Gl.glBindTexture(Gl.GL_TEXTURE_2D, sheetTexture);

      Gl.glBegin(Gl.GL_QUADS);

      Gl.glTexCoord2f(x0, y0);
      Gl.glVertex3f(0.0f, 0.0f, 0.0f);

      Gl.glTexCoord2f(x1, y0);
      Gl.glVertex3f((float)spriteWidth, 0.0f, 0.0f);

      Gl.glTexCoord2f(x1, y1);
      Gl.glVertex3f((float)spriteWidth, (float)spriteHeight, 0.0f);

      Gl.glTexCoord2f(x0, y1);
      Gl.glVertex3f(0.0f, (float)spriteHeight, 0.0f);

      Gl.glEnd();

      Gl.glPopMatrix();
    }


    /// <summary>
    /// Draw a sprite from a sheet of sprites as a texture mapped quad.
    /// </summary>
    public static void DrawSprite(
      double x, double y, int frame,
      double spriteWidth, double spriteHeight,
      int sheetTexture, int sheetRows, int sheetColumns)
    {
      DrawSprite(x, y, frame, spriteWidth, spriteHeight,
                 sheetTexture, sheetColumns, sheetColumns,
                 0, 0, Color.White);
    }


    /// <summary>
    /// Draw a sprite from a sheet of sprites as a texture mapped quad
    /// mirrored along the vertical axis.
    /// </summary>
    public static void DrawMirroredSprite(
      double x, double y, int frame,
      double spriteWidth, double spriteHeight,
      int sheetTexture, int sheetRows, int sheetColumns)
    {
      DrawSprite(x, y, frame, spriteWidth, spriteHeight,
                 sheetTexture, sheetColumns, sheetColumns,
                 1, 0, Color.White);
    }


    /// <summary>
    /// Draw a sprite from a sheet of sprites as a texture mapped quad
    /// mirrored along the horizontal axis.
    /// </summary>
    public static void DrawFlippedSprite(
      double x, double y, int frame,
      double spriteWidth, double spriteHeight,
      int sheetTexture, int sheetRows, int sheetColumns)
    {
      DrawSprite(x, y, frame, spriteWidth, spriteHeight,
                 sheetTexture, sheetColumns, sheetColumns,
                 0, 1, Color.White);
    }



    /// <summary>
    /// Draw a vertically scrolling starfield
    /// </summary>
    public static void DrawStarfield(
      IEnumerable<Vec3> points, double t,
      double pointSize, double screenWidth)
    {
      const double span = 1000.0;
      const double depthFactor = 100.0;


      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

      Gl.glColor3f(1.0f, 1.0f, 1.0f);

      Gl.glPointSize((float)pointSize);

      Gl.glBegin(Gl.GL_POINTS);

      foreach (var point in points)
      {
        Gl.glVertex3f(
          (float)((screenWidth / 2 + point.X) * depthFactor / (depthFactor + point.Z)),
          (float)((point.Y - t % span) * depthFactor / (depthFactor + point.Z)),
          0.0f);
        Gl.glVertex3f(
          (float)((screenWidth / 2 + point.X) * depthFactor / (depthFactor + point.Z)),
          (float)((point.Y + span - t % span) * depthFactor / (depthFactor + point.Z)),
          0.0f);
      }

      Gl.glEnd();
    }


    public static void DrawChar(
      char ch, double x, double y, double size, int sheetTexture, Color color)
    {
      int frame = (int)ch;
      if (frame > 0xff)
      {
        throw new ArgumentException("Can't render chars above 255.", "ch");
      }

      DrawSprite(x, y, frame, size, size, sheetTexture, 16, 16, 0, 0, color);
    }


    public static void DrawString(
      String str, double x, double y, double size, int sheetTexture, Color color)
    {
      for (int i = 0; i < str.Length; i++)
      {
        DrawChar(str[i], x + i * size, y, size, sheetTexture, color);
      }
    }


    public static void GlColor(Color color)
    {
      Gl.glColor4f((float)color.R / 256.0f, (float)color.G / 256.0f, (float)color.B / 256.0f, (float)color.A / 256.0f);
    }


    public static void DrawRect(double x, double y, double w, double h, byte r, byte g, byte b)
    {
      // Clear the bound texture.
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

      Gl.glColor3f((float)r / 256, (float)g / 256, (float)b / 256);

      Gl.glBegin(Gl.GL_QUADS);

      Gl.glVertex3f((float)x, (float)y, 0.0f);

      Gl.glVertex3f((float)(x + w), (float)y, 0.0f);

      Gl.glVertex3f((float)(x + w), (float)(y + h), 0.0f);

      Gl.glVertex3f((float)x, (float)(y + h), 0.0f);

      Gl.glEnd();
    }
  }
}