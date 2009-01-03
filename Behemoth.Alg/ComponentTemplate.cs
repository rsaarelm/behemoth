using System;

namespace Behemoth.Alg
{
  /// <summary>
  /// Base class for entity component constructors.
  /// </summary>
  public abstract class ComponentTemplate
  {
    public abstract String Family { get; }


    /// <summary>
    /// Create a new component instance and assign it to an entity.
    /// </summary>
    public Component Make(Entity entity)
    {
      Component component = BuildComponent(entity);
      if (component.Family != Family)
      {
        throw new ApplicationException(
          "Component template for family '"+Family+
          "' produced a component of family '"+
          Component.GetFamily()+"'.");
      }
      entity.Set(component);
      return component;
    }


    protected abstract Component BuildComponent(Entity entity);
  }
}