using System;

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
    // XXX: Basic stats don't really belong in Brain, but for now they only
    // matter to active creatures.
    public double Health = 1.0;

    public double Resistance = 1.0;

    public double Might = 10.0;

    public double Craft = 10.0;

    public bool AiActive = true;

    public int Alignment = 0;

    public bool Gibs = true;


    public virtual void Damage(Entity cause, double amount)
    {
      Health -= amount / Resistance * Might;
      if (Health < 0.0)
      {
        Action.Kill(Entity, cause);
      }
    }


    public virtual void Update()
    {
      if (AiActive) {
        Action.AttackMove(Entity, new Random().Next(8));
      }
    }


    new public static Type GetFamily()
    {
      return typeof(CBrain);
    }
  }


  [Serializable]
  public class BrainTemplate : ComponentTemplate
  {
    public BrainTemplate()
    {}


    public override Type Family { get { return CBrain.GetFamily(); } }


    protected override Component BuildComponent(Entity entity)
    {
      CBrain result = new CBrain();

      return result;
    }

    // TODO: Stats speccing.
  }
}