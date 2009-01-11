using System;

using Behemoth.Alg;

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


    public void Damage(Entity cause, double amount)
    {
      Health -= amount / Resistance * Might;
      if (Health < 0.0)
      {
        Action.Kill(Entity, cause);
      }
    }


    new public static String GetFamily()
    {
      return "brain";
    }
  }


  [Serializable]
  public class BrainTemplate : ComponentTemplate
  {
    public BrainTemplate()
    {}


    public override string Family { get { return "brain"; } }


    protected override Component BuildComponent(Entity entity)
    {
      CBrain result = new CBrain();

      return result;
    }

    // TODO: Stats speccing.
  }
}