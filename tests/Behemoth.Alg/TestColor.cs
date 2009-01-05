using System;
using System.Collections.Generic;

using NUnit.Framework;

using Behemoth.Alg;

namespace Behemoth.Alg
{
  [TestFixture]
  public class TestColor
  {
    [Test]
    public void TestParseColor()
    {
      Assert.AreEqual(new Color(0x12, 0x34, 0x56), new Color("#123456"));
      Assert.AreEqual(new Color(0x12, 0x34, 0x56, 0xff), new Color("#123456"));
      Assert.AreEqual(new Color(0xab, 0xcd, 0xef), new Color("#abcdef"));
      Assert.AreEqual(new Color(0xab, 0xcd, 0xef), new Color("#aBCdEf"));
      Assert.AreEqual(new Color(0x12, 0x34, 0x56, 0x78), new Color("#12345678"));

      Assert.AreEqual(new Color(0x11, 0x22, 0x33, 0xff), new Color("#123"));
      Assert.AreEqual(new Color(0x11, 0x22, 0x33, 0x44), new Color("#1234"));
    }
  }
}