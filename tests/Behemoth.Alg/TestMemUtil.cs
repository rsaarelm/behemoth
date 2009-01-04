using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Behemoth.Alg;

namespace Behemoth.Alg
{
  [TestFixture]
  public class TestMemUtil
  {
    [Test]
    public void TestStaticMethodLookup()
    {
      Assert.AreEqual(
        1, (int)MemUtil.CallInheritedStaticMethod(typeof(Derived21), "Foo", null));
      Assert.AreEqual(
        2, (int)MemUtil.CallInheritedStaticMethod(typeof(Derived11), "Foo", null));
    }
  }


  class Base
  {
    public static int Foo()
    {
      return 1;
    }
  }


  class Derived1 : Base
  {
    new public static int Foo()
    {
      return 2;
    }
  }


  class Derived2 : Base
  {}


  class Derived11 : Derived1
  {}


  class Derived21 : Derived2
  {}
}