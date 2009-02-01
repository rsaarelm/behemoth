using System;
using System.Collections.Generic;
using Tao.OpenGl;
using Behemoth.Util;

namespace Behemoth.TaoUtil
{
  /// <summary>
  /// Graphics utility functions.
  /// </summary>
  public static class Gfx
  {
    public enum VertexFlags
    {
      Pos = 1 << 0,
      Normal = 1 << 1,
      Color = 1 << 2,
      Texture = 1 << 3,
    }


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
      char ch, double x, double y, double w, double h, int sheetTexture, Color color)
    {
      int frame = (int)ch;
      if (frame > 0xff)
      {
        throw new ArgumentException("Can't render chars above 255.", "ch");
      }

      DrawSprite(x, y, frame, w, h, sheetTexture, 16, 16, 0, 0, color);
    }


    public static void DrawString(
      String str, double x, double y, double fontW, double fontH, int sheetTexture, Color color)
    {
      for (int i = 0; i < str.Length; i++)
      {
        DrawChar(str[i], x + i * fontW, y, fontW, fontH, sheetTexture, color);
      }
    }


    public static void GlColor(Color color)
    {
      Gl.glColor4f((float)color.R / 256.0f, (float)color.G / 256.0f, (float)color.B / 256.0f, (float)color.A / 256.0f);
    }


    /// <summary>
    /// Set OpenGL color for emissive objects.
    /// </summary>
    public static void GlEmissionColor(Color color)
    {
      GlColor(color);
      Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, color.FloatArray);
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


    public static void ClearScreen()
    {
      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
    }


    /// <summary>
    /// Draw an axis-aligned OpenGL cube.
    /// </summary>
    public static void DrawCube(
      float x, float y, float z, float w, float h, float d)
    {
      Gl.glBegin(Gl.GL_QUADS);

      Gl.glNormal3f(-1, 0, 0);
      Gl.glVertex3f(x, y, z);
      Gl.glVertex3f(x, y, z + d);
      Gl.glVertex3f(x, y + h, z + d);
      Gl.glVertex3f(x, y + h, z);

      Gl.glNormal3f(1, 0, 0);
      Gl.glVertex3f(x + w, y, z);
      Gl.glVertex3f(x + w, y + h, z);
      Gl.glVertex3f(x + w, y + h, z + d);
      Gl.glVertex3f(x + w, y, z + d);

      Gl.glNormal3f(0, -1, 0);
      Gl.glVertex3f(x, y, z);
      Gl.glVertex3f(x + w, y, z);
      Gl.glVertex3f(x + w, y, z + d);
      Gl.glVertex3f(x, y, z + d);

      Gl.glNormal3f(0, 1, 0);
      Gl.glVertex3f(x, y + h, z);
      Gl.glVertex3f(x, y + h, z + d);
      Gl.glVertex3f(x + w, y + h, z + d);
      Gl.glVertex3f(x + w, y + h, z);

      Gl.glNormal3f(0, 0, -1);
      Gl.glVertex3f(x, y, z);
      Gl.glVertex3f(x, y + h, z);
      Gl.glVertex3f(x + w, y + h, z);
      Gl.glVertex3f(x + w, y, z);

      Gl.glNormal3f(0, 0, 1);
      Gl.glVertex3f(x, y, z + d);
      Gl.glVertex3f(x + w, y, z + d);
      Gl.glVertex3f(x + w, y + h, z + d);
      Gl.glVertex3f(x, y + h, z + d);
      Gl.glEnd();
    }


    /// <summary>
    /// Draw a triangle mesh. Vertices and normals are arrays which contain
    /// positions and normal coordinates of the mesh vertices. Indices is a
    /// list of the vertex triplets that make the faces of the mesh.
    /// </summary>
    public static void DrawTriMesh(float[,] vertices, float[,] normals, short[,] indices)
    {
      var types = VertexFlags.Pos | VertexFlags.Normal;

      InitArrayState(types);

      Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, vertices);
      Gl.glNormalPointer(Gl.GL_FLOAT, 0, normals);
      Gl.glDrawElements(Gl.GL_TRIANGLES, indices.Length, Gl.GL_UNSIGNED_SHORT, indices);

      UninitArrayState(types);
    }


    /// <summary>
    /// Initialize OpenGL for drawing an element array.
    /// </summary>
    public static void InitArrayState(VertexFlags types)
    {
      if ((types & VertexFlags.Pos) != 0)
        Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
      if ((types & VertexFlags.Normal) != 0)
        Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);
      if ((types & VertexFlags.Color) != 0)
        Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
      if ((types & VertexFlags.Texture) != 0)
        Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
    }


    /// <summary>
    /// Uninitialize OpenGL element array drawing.
    /// </summary>
    public static void UninitArrayState(VertexFlags types)
    {
      if ((types & VertexFlags.Pos) != 0)
        Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
      if ((types & VertexFlags.Normal) != 0)
        Gl.glDisableClientState(Gl.GL_NORMAL_ARRAY);
      if ((types & VertexFlags.Color) != 0)
        Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);
      if ((types & VertexFlags.Texture) != 0)
        Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
    }


    /// <summary>
    /// Draws a single particle using point sprites. Might be less efficient
    /// than drawing a batch of them at once. Doesn't bind texture or set
    /// color, do these before calling.
    /// </summary>
    public static void DrawParticle(float x, float y, float z, float size)
    {
      Gl.glPointSize(size);
      Gl.glTexEnvf(Gl.GL_POINT_SPRITE_ARB, Gl.GL_COORD_REPLACE_ARB, Gl.GL_TRUE);
      Gl.glEnable(Gl.GL_POINT_SPRITE_ARB);

      Gl.glBegin(Gl.GL_POINTS);
      // Set normal so that existing normal state doesn't affect particle shading.
      Gl.glNormal3f(0, 0, 1);
      Gl.glVertex3f(x, y, z);
      Gl.glEnd();

      Gl.glDisable(Gl.GL_POINT_SPRITE_ARB);
    }


    /// <summary>
    /// Draw a glowing laser beam.
    /// </summary>
    public static void DrawBeam(
      Vec3 start, Vec3 end, double size, Color inner, Color outer)
    {
      // TODO: Push translation and rotation to get the result point from start to end.

      Vec3 dir = end - start;

      Vec3 axis;
      double angle;
      Geom.OrientTowards(new Vec3(1, 0, 0), dir, out axis, out angle);

      float length = (float)dir.Abs();
      float unit = (float)size / 4;

      Gl.glPushMatrix();

      Gl.glTranslated(start.X, start.Y, start.Z);
      Gl.glRotated(Geom.Rad2Deg(angle), axis.X, axis.Y, axis.Z);

      Gl.glPushAttrib(Gl.GL_LIGHTING_BIT | Gl.GL_ENABLE_BIT);
      Gl.glBlendFunc(Gl.GL_ONE, Gl.GL_ONE);

      Gfx.GlEmissionColor(inner);
      Gfx.DrawCube(unit, -unit, -unit, length - 2 * unit, 2 * unit, 2 * unit);

      Gfx.GlEmissionColor(outer);
      Gfx.DrawCube(0, -2 * unit, -2 * unit, length, 4 * unit, 4 * unit);

      Gl.glBlendFunc(Gl.GL_ONE, Gl.GL_ZERO);
      Gl.glPopAttrib();

      Gl.glPopMatrix();
    }
  }
}