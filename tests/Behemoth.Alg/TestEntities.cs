using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Behemoth.Alg;

namespace Behemoth.Alg
{
  [TestFixture]
  public class TestEntities
  {
    [Test]
    public void TestEntitySetup()
    {
      // Create some components.
      Component stats = new StatsComponent();
      Component pos = new PosComponent();

      // Create an entity, it starts with nothing but an id.
      Entity ent = new Entity("1")
        // Give it some components (using a handly fluent interface idiom).
        .Set(stats)
        .Set(pos);

      // Read the components back. Thanks to generic magic, we don't need to
      // refer to component families explicitly at all.
      StatsComponent stats2;
      Assert.IsTrue(ent.TryGet(out stats2));
      Assert.AreEqual(stats, stats2);

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


    [Test]
    public void TestEntityTemplates()
    {
      // Mostly just demoing how you're supposed to use the template system.

      // Use a guid dispenser to give entities unique IDs.
      Guid guids = new Guid();

      // Set up some entity templates built from component templates.
      EntityTemplate brogmoidTemplate = new EntityTemplate()
        .AddComponent(new StatsTemplate(6, 2, 4))
        .AddComponent(new RandomPosTemplate());

      EntityTemplate grueTemplate = new EntityTemplate()
        .AddComponent(new StatsTemplate(4, 6, 8))
        .AddComponent(new RandomPosTemplate());

      // Use the templates to build entities.
      Entity brogmoid1 = brogmoidTemplate.Make(guids.Next());
      Entity brogmoid2 = brogmoidTemplate.Make(guids.Next());
      Entity grue1 = grueTemplate.Make(guids.Next());

      Assert.AreEqual(6, brogmoid1.Get<StatsComponent>().Might);
      Assert.AreEqual(6, brogmoid2.Get<StatsComponent>().Might);
      Assert.AreEqual(8, grue1.Get<StatsComponent>().Speed);

    }
  }


  [Serializable]
  class StatsComponent : Component
  {
    new public static String GetFamily()
    {
      return "stats";
    }

    public int Might;
    public int Craft;
    public int Speed;
  }


  [Serializable]
  class PosComponent : Component
  {
    new public static String GetFamily()
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


  class StatsTemplate : ComponentTemplate
  {
    public StatsTemplate(int might, int craft, int speed)
    {
      this.might = might;
      this.craft = craft;
      this.speed = speed;
    }


    public override String Family { get { return StatsComponent.GetFamily(); } }


    protected override Component BuildComponent(Entity entity)
    {
      StatsComponent result = new StatsComponent();
      result.Might = might;
      result.Craft = craft;
      result.Speed = speed;
      return result;
    }


    private int might;
    private int craft;
    private int speed;
  }


  class RandomPosTemplate : ComponentTemplate
  {
    public override String Family { get { return PosComponent.GetFamily(); } }


    protected override Component BuildComponent(Entity entity)
    {
      PosComponent result = new PosComponent();
      Random random = new Random();
      result.X = random.Next(80);
      result.Y = random.Next(80);

      return result;
    }

  }
}
