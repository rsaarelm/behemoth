using System;

using Behemoth.Alg;

namespace Rpg
{
  [Serializable]
  public class CCore : Component
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

    /// <summary>
    /// What layer will the entity be drawn in. Entities with larger draw
    /// priority get drawn on top of entities with a smaller one.
    /// </summary>
    public int DrawPriority;

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


    public CoreTemplate(
      string name,
      int icon,
      int drawPriority) : this(name, icon)
    {
      this.drawPriority = drawPriority;
    }


    public override string Family { get { return "core"; } }


    protected override Component BuildComponent(Entity entity)
    {
      CCore result = new CCore();
      result.Name = name;
      result.Icon = icon;
      result.DrawPriority = drawPriority;

      return result;
    }


    private string name;

    private int icon;

    private int drawPriority = 0;
  }

}