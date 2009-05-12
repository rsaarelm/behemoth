using System;
using System.Linq;

using Behemoth.Util;

namespace Rpg
{
  /// <summary>
  /// A component for entities that are actively moving around and doing
  /// stuff.
  /// </summary>
  [Serializable]
  public class CBrain : Component
  {
    public enum States
    {
      Sleep,
      Wander,
      Hunt,
    }

    // XXX: Basic stats don't really belong in Brain, but for now they only
    // matter to active creatures.
    public double Health = 1.0;

    public double Resistance = 1.0;

    public double Might = 10.0;

    public double Craft = 10.0;

    public bool AiActive = true;

    public int Alignment = 0;

    public bool Gibs = true;

    public Entity Target = null;

    public States State = States.Sleep;

    public virtual void Damage(Entity cause, double amount)
    {
      Health -= amount / (Resistance * Might);
      if (Health < 0.0)
      {
        Action.Kill(Entity, cause);
      }
    }


    public virtual void Update()
    {
      if (AiActive) {
        State = StateTransition(State);
        switch (State)
        {
        case States.Sleep:
          break;
        case States.Wander:
          RandomMove();
          break;
        case States.Hunt:
          MoveTowards(Target.Get<CCore>().Pos);
          break;
        }
      }
    }


    void RandomMove()
    {
      Action.AttackMove(
        Entity,
        Rpg.Service.Rng.RandInt(8));
    }


    void MoveTowards(Vec3 targetPos)
    {
      var pos = Entity.Get<CCore>().Pos;

      if (pos == targetPos)
      {
        return;
      }

      int moveDir;
      if (Geom.PointTo(
            pos,
            targetPos,
            out moveDir))
      {
        var success = Action.AttackMove(Entity, moveDir);
        if (!success)
        {
          RandomMove();
        }
      }
      else
      {
        RandomMove();
      }

    }


    States StateTransition(States state)
    {
      switch (state)
      {
      case States.Sleep:
        if (FindTarget(out Target))
        {
          UI.Msg("The {0} wakes up.", Entity.Get<CCore>().Name);
          return States.Hunt;
        }
        break;
      case States.Hunt:
        if (!Query.IsAlive(Target))
        {
          // Stop hunting dead things.
          return States.Wander;
        }

        if (Query.Distance(Entity, Target) > 8.0)
        {
          // Get bored if target is too far away.
          if (Rpg.Service.Rng.OneChanceIn(4))
          {
            return States.Wander;
          }
        }

        break;
      case States.Wander:
        if (FindTarget(out Target))
        {
          return States.Hunt;
        }
        break;
      }
      return state;
    }


    bool FindTarget(out Entity target)
    {
      target = null;

      var targets =
        from e in Query.NearbyEnemies(Entity)
        where Query.Distance(Entity, e) < 10
        orderby Query.Distance(Entity, e)
        select e;

      foreach (var e in targets)
      {
        if (Query.Notices(Entity, e))
        {
          target = e;
          return true;
        }
      }

      return false;
    }



    new public static Type GetFamily()
    {
      return typeof(CBrain);
    }
  }


  [Serializable]
  public class BrainTemplate : ComponentTemplate
  {
    private BrainTemplate()
    {}


    /// <summary>
    /// The basic brain builder. Expect the interface to change a lot. Needs
    /// to get an AI style parameter and more stats parameters, if we even
    /// keep the stats coupled with the AI component in the future...
    ///
    /// Might represents the general physical prowess and durability of a
    /// creature. Big and strong creatures are mighty, small and weak ones are
    /// not. An average human has might 10. Creatures with high might tend to
    /// be monsters.
    ///
    /// Craft represents the skill and cunning of a creature. Crafty creatures
    /// can use complex skills and items, and can be dangerous even if they
    /// aren't mighty. An unskilled human has craft 10, but human experts can
    /// have very high craft values.
    /// </summary>
    public static BrainTemplate Default(double might, double craft)
    {
      var result = new BrainTemplate();

      result.might = might;
      result.craft = craft;

      return result;
    }


    // Some fluent interface setup stuff.

    public BrainTemplate Alignment(int alignment)
    {
      this.alignment = alignment;
      return this;
    }


    public override Type Family { get { return CBrain.GetFamily(); } }


    protected override Component BuildComponent(Entity entity)
    {
      CBrain result = new CBrain();

      result.Might = might;
      result.Craft = craft;

      result.Resistance = might / 4;

      return result;
    }

    double might;
    double craft;

    int alignment = 0;
  }
}