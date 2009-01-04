using System;

namespace Behemoth.Alg
{
  [Serializable]
  public class Component
  {
    public String Family { get { return FamilyOf(this.GetType()); } }


    public static String FamilyOf<T>()
      where T : Component
    {
      return FamilyOf(typeof(T));
    }

    public static String FamilyOf(Type type)
    {
      var result = (String)MemUtil.InheritedStaticMethod(type, "GetFamily", null);
      if (result == null)
      {
        throw new ApplicationException(
          "No static GetFamily method found in Component subtype. Shouldn't happen.");
      }
      return result;
    }


    /// <summary>
    /// A failsafe method that family lookup falls into when a component is
    /// not rooted in a subfamily that defines a static family lookup method.
    /// Causes a run-time error when called.
    /// </summary>
    public static String GetFamily()
    {
      throw new ApplicationException("GetFamily lookup fell back to root Component class.");
    }


    /// <summary>
    /// The entity this component is currently attached to. Null if none.
    /// </summary>
    public Entity Entity { get { return entity; } }


    /// <summary>
    /// Sets the entity the component is attached to.
    /// </summary>
    /// <remarks>
    /// Use Entity.Set to manage components. Do not call this directly.
    /// </remarks>
    internal void Attach(Entity entity)
    {
      this.entity = entity;
      InnerAttach(entity);
    }


    /// <summary>
    /// Sets the component to be detached from an entity.
    /// </summary>
    /// <remarks>
    /// Use Entity.Set to manage components. Do not call this directly.
    /// </remarks>
    internal void Detach()
    {
      InnerDetach();
      this.entity = null;
    }


    /// <summary>
    /// Component-specific hook for attaching to entities.
    /// </summary>
    protected virtual void InnerAttach(Entity entity) {}


    /// <summary>
    /// Component-specific hook for detaching from entities.
    /// </summary>
    protected virtual void InnerDetach() {}


    protected String family;

    private Entity entity;
  }
}