using System;

using Behemoth.Util;

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

    public bool IsObstacle;

    /// <summary>
    /// Static objects are treated like the background tiles and shown on
    /// remembered map.
    /// </summary>
    public bool IsStatic;

    /// <summary>
    /// What layer will the entity be drawn in. Entities with larger draw
    /// priority get drawn on top of entities with a smaller one.
    /// </summary>
    public int DrawPriority;

    new public static Type GetFamily()
    {
      return typeof(CCore);
    }
  }


  public class CoreTemplate : ComponentTemplate
  {
    private CoreTemplate() {}


    public static CoreTemplate Default(string name, int icon)
    {
      var result = new CoreTemplate();
      result.name = name;
      result.icon = icon;
      return result;
    }


    public static CoreTemplate FloorStatic(string name, int icon)
    {
      var result = Default(name, icon);
      result.drawPriority = -1;
      result.isObstacle = false;
      result.isStatic = true;
      return result;
    }


    public static CoreTemplate BlockStatic(string name, int icon)
    {
      var result = Default(name, icon);
      result.isStatic = true;
      return result;
    }


    public override Type Family { get { return CCore.GetFamily(); } }


    protected override Component BuildComponent(Entity entity)
    {
      CCore result = new CCore();
      result.Name = name;
      result.Icon = icon;
      result.DrawPriority = drawPriority;
      result.IsStatic = isStatic;
      result.IsObstacle = isObstacle;

      return result;
    }


    private string name;

    private int icon;

    private int drawPriority = 0;

    private bool isStatic = false;

    private bool isObstacle = true;
  }

}