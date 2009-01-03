using System;

namespace Behemoth.Alg
{
  using ComponentFamily = String;

  [Serializable]
  public class Component
  {
    public ComponentFamily Family { get { return FamilyOf(this.GetType()); } }


    public static ComponentFamily FamilyOf<T>()
      where T : Component
    {
      return FamilyOf(typeof(T));
    }

    public static ComponentFamily FamilyOf(Type type)
    {
      var result = (ComponentFamily)MemUtil.InheritedStaticMethod(type, "GetFamily", null);
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
    public static ComponentFamily GetFamily()
    {
      throw new ApplicationException("GetFamily lookup fell back to root Component class.");
    }


    protected ComponentFamily family;
  }
}