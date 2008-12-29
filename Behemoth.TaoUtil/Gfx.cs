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
      float x, float y, int frame,
      float spriteWidth, float spriteHeight,
      int sheetTexture, int sheetRows, int sheetColumns)
    {
      float x0 = (float)(frame % sheetColumns) / (float)sheetColumns;
      float y0 = (float)((sheetColumns + frame) / sheetRows) / (float)sheetRows;

      float x1 = x0 + 1.0f / (float)sheetColumns;
      float y1 = y0 - 1.0f / (float)sheetRows;

      Gl.glColor3f(1.0f, 1.0f, 1.0f);

      Gl.glPushMatrix();

      Gl.glTranslatef(x, y, 0.0f);

      Gl.glBindTexture(Gl.GL_TEXTURE_2D, sheetTexture);

      Gl.glBegin(Gl.GL_QUADS);

      Gl.glTexCoord2f(x0, y0);
      Gl.glVertex3f(0.0f, 0.0f, 0.0f);

      Gl.glTexCoord2f(x1, y0);
      Gl.glVertex3f(spriteWidth, 0.0f, 0.0f);

      Gl.glTexCoord2f(x1, y1);
      Gl.glVertex3f(spriteWidth, spriteHeight, 0.0f);

      Gl.glTexCoord2f(x0, y1);
      Gl.glVertex3f(0.0f, spriteHeight, 0.0f);

      Gl.glEnd();

      Gl.glPopMatrix();
    }


    /// <summary>
    /// Draw a vertically scrolling starfield
    /// </summary>
    public static void DrawStarfield(
      IEnumerable<Vec3<double>> points, double t,
      double pointSize, double screenWidth)
    {
      const double span = 1000.0;
      const float depthFactor = 100.0f;


      Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

      Gl.glColor3f(1.0f, 1.0f, 1.0f);

      Gl.glPointSize((float)pointSize);

      Gl.glBegin(Gl.GL_POINTS);

      foreach (Vec3<double> point in points)
      {
        Gl.glVertex3f(
          (float)(screenWidth / 2 + point.X) * depthFactor / (depthFactor + (float)point.Z),
          (float)(point.Y - t % span) * depthFactor / (depthFactor + (float)point.Z),
          0.0f);
        Gl.glVertex3f(
          (float)(screenWidth / 2 + point.X) * depthFactor / (depthFactor + (float)point.Z),
          (float)(point.Y + span - t % span) * depthFactor / (depthFactor + (float)point.Z),
          0.0f);
      }

      Gl.glEnd();
    }

  }
}