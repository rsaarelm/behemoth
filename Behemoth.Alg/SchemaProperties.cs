using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Behemoth.Alg
{
  /// <summary>
  /// A version of the Properties class which uses typing schemas on the
  /// fields.
  /// </summary>
  // XXX: Not serializable because of the lambdas.
  public class SchemaProperties<TKey, TValue> : Properties<TKey, TValue>
  {
    /// <summary>
    /// Create a properties object without a parent.
    /// </summary>
    public SchemaProperties() : base()
    {
    }


    /// <summary>
    /// Create a properties object with a parent.
    /// </summary>
    public SchemaProperties(Properties<TKey, TValue> parent) : base(parent)
    {
    }


    public override Properties<TKey, TValue> Parent
    {
      get { return parent; }
      set {
        // If parent is also SchemaProperties, make the constraints link to the parent's constraints.
        SchemaProperties<TKey, TValue> schemaParent =
          value as SchemaProperties<TKey, TValue>;
        if (schemaParent != null)
        {
          constraints.Parent = schemaParent.constraints;
        }

        parent = value;
      }
    }


    public override Properties<TKey, TValue> Set(TKey key, TValue val)
    {
      Func<TValue, string> pred;
      if (constraints.TryGet(key, out pred))
      {
        string errorMsg = pred(val);
        if (errorMsg != null)
        {
          throw new ArgumentException(
            "Constraint failed for "+val.ToString()+" -> "+
            key.ToString()+": " + errorMsg, "val");
        }
      }

      return base.Set(key, val);
    }


    /// <summary>
    /// Add a value constraint for a key. The constraint is a function that
    /// takes a proposed value and returns null for valid values and a
    /// diagnostic error message for invalid values.
    /// </summary>
    public void SetConstraint(TKey key, Func<TValue, string> constraint)
    {
      constraints[key] = constraint;
    }


    /// <summary>
    /// Shortcut for setting a type constraint.
    /// </summary>
    public void SetConstraint(TKey key, Type type)
    {
      SetConstraint(key, Alg.TypeP<TValue>(type));
    }


    /// <summary>
    /// Sets a new constraint if a key doesn't have any constraints yet.
    /// Otherwise sets the key's constraint as a conjunction of the existing
    /// and the new constraint.
    /// </summary>
    /// <remarks>
    /// Constraints are short-circuiting and earlier constraints are evaluated
    /// before later constraints.
    /// </remarks>
    public void AddConstraint(TKey key, Func<TValue, string> constraint)
    {
      if (constraints.ContainsKey(key))
      {
        constraints[key] = Alg.Both(constraints[key], constraint);
      }
      else
      {
        constraints[key] = constraint;
      }
    }


    public void HideConstraint(TKey key)
    {
      constraints.Hide(key);
    }


    public void UnsetConstraint(TKey key)
    {
      constraints.Unset(key);
    }


    private Properties<TKey, Func<TValue, string>> constraints =
      new Properties<TKey, Func<TValue, string>>();
  }
}