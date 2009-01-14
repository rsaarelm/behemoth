using System;
using System.Collections.Generic;

using NUnit.Framework;

using Behemoth.Util;

namespace Behemoth.Util
{
  [TestFixture]
  public class TestAlg
  {
    [Test]
    public void TestConcat()
    {
      Assert.AreEqual(Alg.L<int>(), Alg.Concat(Alg.L(Alg.L<int>(), Alg.L<int>())));
      Assert.AreEqual(Alg.L(1, 2), Alg.Concat(Alg.L(Alg.L<int>(), Alg.L(1, 2))));
      Assert.AreEqual(Alg.L(1, 2), Alg.Concat(Alg.L(Alg.L(1, 2), Alg.L<int>())));
      Assert.AreEqual(Alg.L(1, 2), Alg.Concat(Alg.L(Alg.L(1), Alg.L(2))));
      Assert.AreEqual(Alg.L(1, 2, 3, 4), Alg.Concat(Alg.L(Alg.L(1, 2), Alg.L(3, 4))));
    }


    [Test]
    public void TestDictLiteral()
    {
      var dict = Alg.Dict<string, int>(
        "foo", 1,
        "bar", 2,
        "quux", 17);

      Assert.AreEqual(1, dict["foo"]);
      Assert.AreEqual(2, dict["bar"]);
      Assert.AreEqual(17, dict["quux"]);

    }
  }
}