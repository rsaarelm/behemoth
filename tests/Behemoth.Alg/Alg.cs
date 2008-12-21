using System;
using System.Collections.Generic;

using NUnit.Framework;

using Behemoth.Alg;

namespace Behemoth.Alg
{
  [TestFixture()]
  public class TestAlg
  {
    [Test]
    public void Concat()
    {
      Assert.AreEqual(Alg.L<int>(), Alg.Concat(Alg.L(Alg.L<int>(), Alg.L<int>())));
      Assert.AreEqual(Alg.L(1, 2), Alg.Concat(Alg.L(Alg.L<int>(), Alg.L(1, 2))));
      Assert.AreEqual(Alg.L(1, 2), Alg.Concat(Alg.L(Alg.L(1, 2), Alg.L<int>())));
      Assert.AreEqual(Alg.L(1, 2), Alg.Concat(Alg.L(Alg.L(1), Alg.L(2))));
      Assert.AreEqual(Alg.L(1, 2, 3, 4), Alg.Concat(Alg.L(Alg.L(1, 2), Alg.L(3, 4))));
    }
  }
}