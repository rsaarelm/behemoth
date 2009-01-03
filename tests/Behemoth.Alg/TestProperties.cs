using System;
using System.Collections.Generic;

using NUnit.Framework;

using Behemoth.Alg;

namespace Behemoth.Alg
{
  using Props = Properties<string, Object>;

  [TestFixture]
  public class TestProperties
  {
    Props SimpleProps
    {
      get
      {
        var props = new Props();
        props["xyzzy"] = 666;
        return props;
      }
    }


    [Test]
    public void TestSimple()
    {
      var props = new Props();

      Assert.IsFalse(props.ContainsKey("xyzzy"));

//      Assert.Throws(
//        typeof(KeyNotFoundException),
//        delegate { return props["xyzzy"]; });

      props["xyzzy"] = 666;

      Assert.AreEqual(666, props["xyzzy"]);
    }

    [Test]
    public void TestInherit()
    {
      var props = SimpleProps;
      var props2 = new Props(props);

      Assert.AreEqual(666, props["xyzzy"]);
      Assert.AreEqual(666, props2["xyzzy"]);
      Assert.IsFalse(props2.ContainsKey("quux"));
    }

    [Test]
    public void TestInheritAndChange()
    {
      var props = SimpleProps;
      var props2 = new Props(props);

      props2["xyzzy"] = 777;

      // The original mustn't change.
      Assert.AreEqual(666, props["xyzzy"]);
      Assert.AreEqual(777, props2["xyzzy"]);
    }

    [Test]
    public void TestInheritAndHide()
    {
      var props = SimpleProps;
      var props2 = new Props(props);

      props2.Hide("xyzzy");

      // The original mustn't change.
      Assert.AreEqual(666, props["xyzzy"]);
//      Assert.Throws(
//        typeof(KeyNotFoundException),
//        delegate { return props2["xyzzy"]; });
    }

    [Test]
    public void TestUnset()
    {
      var props = SimpleProps;
      var props2 = new Props(props);

      props2.Hide("xyzzy");

      // Wipe all local modifications for "xyzzy" from props2.
      props2.Unset("xyzzy");
      Assert.AreEqual(666, props["xyzzy"]);
      Assert.AreEqual(666, props2["xyzzy"]);
    }


    enum KeySet
    {
      Title,
      ISBN,
      DatePublished,
    }

    [Test]
    public void TestKeyEnum()
    {
      var props = new Properties<KeySet, Object>();

      props[KeySet.Title] = "The C Programming Language";
      props[KeySet.ISBN] = "0131103628";
      props[KeySet.DatePublished] = new DateTime(1988, 4, 1);

      Assert.AreEqual("0131103628", props[KeySet.ISBN]);
    }

  }
}