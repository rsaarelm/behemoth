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


    protected String family;
  }
}