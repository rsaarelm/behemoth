using System;

namespace Behemoth.Util
{
  /// <summary>
  /// Base class for entity component constructors.
  /// </summary>
  public abstract class ComponentTemplate
  {
    public abstract Type Family { get; }


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


    /// <summary>
    /// A method for actually building the component specified by this
    /// template. The entity reference is given so that the process can look
    /// things up from the entity. This methdod is _not_ supposed to actually
    /// add the component to the entity. Just return it to the non-virtual
    /// Make method which does the actual addition.
    /// </summary>
    protected abstract Component BuildComponent(Entity entity);


    /// <summary>
    /// A method that is called when this component template is added to an
    /// entity template. May modify the entity template (set it's name for
    /// example)
    /// </summary>
    public virtual void AddToEntityTemplateHook(EntityTemplate template)
    {
    }
  }
}