using System;
using System.Collections.Generic;

using NUnit.Framework;

using Behemoth.Util;

namespace Behemoth.Util
{
  [TestFixture]
  public class TestNum
  {
    [Test]
    public void TestGaussian()
    {
      double precision = 0.0001;

      Assert.AreEqual(0.2419, Num.Gaussian(-1.0, 0.0, 1.0),
                      precision);
      Assert.AreEqual(0.3989, Num.Gaussian(0.0, 0.0, 1.0),
                      precision);

      Assert.AreEqual(0.1587,
                      Num.CumulativeGaussian(-1.0, 0.0, 1.0),
                      precision);
      Assert.AreEqual(0.5, Num.CumulativeGaussian(0.0, 0.0, 1.0),
                      precision);
      Assert.AreEqual(0.8413, Num.CumulativeGaussian(1.0, 0.0, 1.0),
                      precision);
      Assert.AreEqual(0.9772, Num.CumulativeGaussian(2.0, 0.0, 1.0),
                      precision);
    }
  }
}
