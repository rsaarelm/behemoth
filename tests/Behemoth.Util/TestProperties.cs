using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Behemoth.Util;

namespace Behemoth.Util
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

      try
      {
        Assert.AreEqual(666, props["xyzzy"]);
        Assert.Fail("Exception not triggered.");
      }
      catch (KeyNotFoundException)
      {}

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

      try
      {
        Assert.AreEqual(666, props2["xyzzy"]);
        Assert.Fail("Exception not triggered.");
      }
      catch (KeyNotFoundException)
      {}

      // Setting a value must cancel hiding.
      props2["xyzzy"] = 777;
      Assert.AreEqual(777, props2["xyzzy"]);
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
      var props = new Properties<KeySet, Object>().Add(
        KeySet.Title, "The C Programming Language",
        KeySet.ISBN, "0131103628",
        KeySet.DatePublished, new DateTime(1988, 4, 1));

      Assert.AreEqual("0131103628", props[KeySet.ISBN]);
    }


    [Test]
    public void TestConstraints()
    {
      var props = new SchemaProperties<string, Object>();
      props.SetConstraint("intKey", Alg.TypeP<Object>(typeof(int)));

      props["fooKey"] = "bar";
      props["intKey"] = 123;

      try
      {
        // Can't use non-integer keys.
        props["intKey"] = "quux";
        Assert.Fail("Exception not triggered.");
      }
      catch (ArgumentException)
      {}
    }


    [Test]
    public void TestComplexConstraints()
    {
      var props = new SchemaProperties<string, Object>();

      // Use the shortcut for defining type properties.
      props.SetConstraint("intKey", typeof(int));

      props.AddConstraint("intKey", val => (int)val > 10 ? "Too large!" : null);
      props.AddConstraint("intKey", val => (int)val < 0 ? "Too small!" : null);

      props["intKey"] = 5;

      try
      {
        // Can't use non-integer keys.
        props["intKey"] = "quux";
        Assert.Fail("Exception not triggered.");
      }
      catch (ArgumentException)
      {}

      try
      {
        // Range constraint 1
        props["intKey"] = 15;
        Assert.Fail("Exception not triggered.");
      }
      catch (ArgumentException)
      {}

      try
      {
        // Range constraint 2
        props["intKey"] = -10;
        Assert.Fail("Exception not triggered.");
      }
      catch (ArgumentException)
      {}
    }

    [Test]
    public void TestSerialization()
    {
      var props = new Properties<string, int>();
      props["xyzzy"] = 666;

      var props2 = TestUtil.RoundtripSerialize(props);

      Assert.AreEqual(666, props2["xyzzy"]);
    }
  }
}