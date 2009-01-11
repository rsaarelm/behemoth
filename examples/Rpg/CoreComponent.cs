using System;

using Behemoth.Alg;

namespace Rpg
{
  [Serializable]
  public class CoreComponent : Component
  {
    public Vec3 Pos;

    public void SetPos(int x, int y, int z)
    {
      Pos.X = x;
      Pos.Y = y;
      Pos.Z = z;
    }

    public int Icon;

    public String Name;

    public byte Facing;

    public World World;

    public bool ActionPose;

    new public static String GetFamily()
    {
      return "core";
    }
  }


  public class CoreTemplate : ComponentTemplate
  {
    public CoreTemplate(
      string name,
      int icon)
    {
      this.name = name;
      this.icon = icon;
    }


    public override string Family { get { return "core"; } }


    protected override Component BuildComponent(Entity entity)
    {
      CoreComponent result = new CoreComponent();
      result.Name = name;
      result.Icon = icon;

      return result;
    }


    private string name;

    private int icon;
  }

}