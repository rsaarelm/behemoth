using System;
using System.Collections.Generic;

using NUnit.Framework;

using Behemoth.Alg;

namespace Behemoth.Alg
{
  [TestFixture]
  public class TestGeom
  {
    String RectString(double x, double y, double w, double h)
    {
      return "[("+x+", "+y+") (+"+w+", +"+h+")]";
    }


    void WillIntersect(
      double x1, double y1, double w1, double h1,
      double x2, double y2, double w2, double h2)
    {
      String id = RectString(x1, y1, w1, h1)+" "+RectString(x2, y2, w2, h2);
      Assert.IsTrue(Geom.RectanglesIntersect(
                      x1, y1, w1, h1,
                      x2, y2, w2, h2),
        id);
      Assert.IsTrue(Geom.RectanglesIntersect(
                      x2, y2, w2, h2,
                      x1, y1, w1, h1),
        id);
    }


    void WontIntersect(
      double x1, double y1, double w1, double h1,
      double x2, double y2, double w2, double h2)
    {
      String id = RectString(x1, y1, w1, h1)+" "+RectString(x2, y2, w2, h2);
      Assert.IsFalse(Geom.RectanglesIntersect(
                      x1, y1, w1, h1,
                      x2, y2, w2, h2),
        id);
      Assert.IsFalse(Geom.RectanglesIntersect(
                      x2, y2, w2, h2,
                      x1, y1, w1, h1),
        id);
    }


    [Test]
    public void TestRectanglesIntersect()
    {
      WillIntersect(
        10, 10, 10, 10,
        10, 10, 10, 10);
      WillIntersect(
        0, 0, 0, 0,
        0, 0, 0, 0);
      WillIntersect(
        10, 10, 10, 10,
        0, 0, 30, 30);
      WillIntersect(
        10, 10, 10, 10,
        12, 0, 2, 30);
      WillIntersect(
        10, 10, 10, 10,
        0, 12, 30, 2);
      WillIntersect(
        10, 10, 10, 10,
        15, 15, 10, 10);

      WontIntersect(
        10, 10, 10, 10,
        30, 10, 10, 10);
      WontIntersect(
        10, 10, 10, 10,
        10, 30, 10, 10);
      WontIntersect(
        10, 10, 10, 10,
        30, 30, 10, 10);
    }

    [Test]
    public void TestMakeScaledViewport()
    {
      int x, y, w, h;
      int pixelWidth = 320;
      int pixelHeight = 240;

      Geom.MakeScaledViewport(pixelWidth, pixelHeight,
                         80, 80,
                         out x, out y, out w, out h);
      Assert.AreEqual(Alg.L(0, 10, 80, 60), Alg.L(x, y, w, h));

      Geom.MakeScaledViewport(pixelWidth, pixelHeight,
                         500, 500,
                         out x, out y, out w, out h);
      Assert.AreEqual(Alg.L(90, 130, 320, 240), Alg.L(x, y, w, h));

      Geom.MakeScaledViewport(pixelWidth, pixelHeight,
                         700, 400,
                         out x, out y, out w, out h);
      Assert.AreEqual(Alg.L(190, 80, 320, 240), Alg.L(x, y, w, h));

      Geom.MakeScaledViewport(pixelWidth, pixelHeight,
                         700, 700,
                         out x, out y, out w, out h);
      Assert.AreEqual(Alg.L(30, 110, 640, 480), Alg.L(x, y, w, h));

    }


    [Test]
    public void TestHexadecant()
    {
      Assert.AreEqual(0, Geom.Hexadecant(0, 1));
      Assert.AreEqual(0, Geom.Hexadecant(0.001, 1));
      Assert.AreEqual(2, Geom.Hexadecant(1, 1));
      Assert.AreEqual(4, Geom.Hexadecant(1, 0));
      Assert.AreEqual(8, Geom.Hexadecant(0, -1));
      Assert.AreEqual(12, Geom.Hexadecant(-1, 0));
      Assert.AreEqual(15, Geom.Hexadecant(-0.001, 1));
    }


    [Test]
    public void TestVecToDir()
    {
      Assert.AreEqual(0, Geom.VecToDir4(new Vec3(-0.9, 1, 0)));
      Assert.AreEqual(0, Geom.VecToDir4(new Vec3(0, 1, 0)));
      Assert.AreEqual(0, Geom.VecToDir4(new Vec3(0.9, 1, 0)));

      Assert.AreEqual(1, Geom.VecToDir4(new Vec3(1.1, 1, 0)));
      Assert.AreEqual(2, Geom.VecToDir4(new Vec3(-0.9, -1, 0)));
      Assert.AreEqual(3, Geom.VecToDir4(new Vec3(-1.1, -1, 0)));


      Assert.AreEqual(7, Geom.VecToDir8(new Vec3(-0.6, 1, 0)));
      Assert.AreEqual(0, Geom.VecToDir8(new Vec3(-0.4, 1, 0)));
      Assert.AreEqual(0, Geom.VecToDir8(new Vec3(0, 1, 0)));
      Assert.AreEqual(0, Geom.VecToDir8(new Vec3(0.4, 1, 0)));
      Assert.AreEqual(1, Geom.VecToDir8(new Vec3(0.6, 1, 0)));

      Assert.AreEqual(1, Geom.VecToDir8(new Vec3(1, 1, 0)));
      Assert.AreEqual(2, Geom.VecToDir8(new Vec3(1, 0, 0)));
      Assert.AreEqual(3, Geom.VecToDir8(new Vec3(1, -1, 0)));
      Assert.AreEqual(4, Geom.VecToDir8(new Vec3(0, -1, 0)));
      Assert.AreEqual(5, Geom.VecToDir8(new Vec3(-1, -1, 0)));
      Assert.AreEqual(6, Geom.VecToDir8(new Vec3(-1, 0, 0)));
      Assert.AreEqual(7, Geom.VecToDir8(new Vec3(-1, 1, 0)));

    }
  }
}
