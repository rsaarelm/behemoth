using System;
using System.Collections.Generic;
using System.Reflection;

namespace Behemoth.Alg
{
  /// <summary>
  /// Utils related to the innards of the runtime.
  /// </summary>
  public static class MemUtil
  {
    /// <summary>
    /// Look for a static method from a class and then from its parents.
    /// </summary>
    public static Object InheritedStaticMethod(
      Type type, string methodName, Object[] args)
    {
      try
      {
        return type.InvokeMember(
          methodName, BindingFlags.Static | BindingFlags.InvokeMethod,
          null, null, args);
      }
      catch (MissingMethodException)
      {
        if (type.BaseType != null)
        {
          return InheritedStaticMethod(type.BaseType, methodName, args);
        }
        else
        {
          return null;
        }
      }
    }
  }
}