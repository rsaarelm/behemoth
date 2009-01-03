using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Behemoth.Alg;

namespace Behemoth.Alg
{
  using ComponentFamily = String;


  [TestFixture]
  public class TestEntities
  {
    [Test]
    public void TestEntitySetup()
    {
      // Create some components.
      Component ticker = new TickComponent();
      Component pos = new PosComponent();

      // Create an entity, it starts with nothing but an id.
      Entity ent = new Entity("1")
        // Give it some components (using a handly fluent interface idiom).
        .Set(ticker)
        .Set(pos);

      // Read the components back. Thanks to generic magic, we don't need to
      // refer to component families explicitly at all.
      TickComponent ticker2;
      Assert.IsTrue(ent.TryGet(out ticker2));
      Assert.AreEqual(ticker, ticker2);

      PosComponent pos2;
      Assert.IsTrue(ent.TryGet(out pos2));
      Assert.AreEqual(pos, pos2);

      // Now for a somewhat subtle pitfall from the generic magic:
      try
      {
        // We declare something as just a Component and try to get it.
        Component comp;
        ent.TryGet(out comp);
        Assert.Fail("Exception didn't happen.");
      }
      catch(ApplicationException)
      {
        // The base component class doesn't have a valid family. Trying to get
        // a variable without a subcomponent type that has a specified family
        // causes a runtime exception.
      }

      // Another bad thing to do is to add a Component without a proper
      // GetFamily method:
      try
      {
        ent.Set(new BadComponent());
        Assert.Fail("Exception didn't happen.");
      }
      catch(ApplicationException)
      {
        // This also crashes, since the component type fails to describe
        // itself properly.
      }
    }
  }


  [Serializable]
  class TickComponent : Component
  {
    new public static ComponentFamily GetFamily()
    {
      return "tick";
    }

    public int Tick()
    {
      return tick++;
    }

    int tick = 0;
  }


  [Serializable]
  class PosComponent : Component
  {
    new public static ComponentFamily GetFamily()
    {
      return "pos";
    }

    public int X;
    public int Y;
  }


  [Serializable]
  class BadComponent : Component
  {
    // The GetFamily method isn't defined. Description above on how this causes trouble.
  }
}
