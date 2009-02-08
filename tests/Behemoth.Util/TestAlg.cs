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


    [Test]
    public void TestSeqSetOps()
    {
      var seq1 = Alg.L(1, 2, 3, 5);
      var seq2 = Alg.L(2, 4, 5, 6);

      Assert.AreEqual(Alg.L(1, 2, 3, 4, 5, 6), new List<int>(Alg.SortedUnion(seq1, seq2)));
      Assert.AreEqual(Alg.L(2, 5), new List<int>(Alg.SortedIntersection(seq1, seq2)));
      Assert.AreEqual(Alg.L(1, 3), new List<int>(Alg.SortedDifference(seq1, seq2)));
    }
  }
}