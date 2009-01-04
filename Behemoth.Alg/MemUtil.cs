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
    /// Look for a static method from a class and then from its parents. Call
    /// the method if found and return the result.
    /// </summary>
    /// <params name="type">
    /// The type of the class where the static method lookup starts.
    /// </params>
    /// <type name="methodName">
    /// The name of the method to look for.
    /// </type>
    /// <type name="args">
    /// Arguments passed to the static method call.
    /// </type>
    /// <returns>
    /// The result of the static method call, if the method was found. Null
    /// otherwise.
    /// </returns>
    public static Object CallInheritedStaticMethod(
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
          return CallInheritedStaticMethod(type.BaseType, methodName, args);
        }
        else
        {
          return null;
        }
      }
    }
  }
}