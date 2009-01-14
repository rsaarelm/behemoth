using System;
using System.Collections.Generic;

namespace Behemoth.Util
{
  public class ComponentNotFoundException : Exception
  {
    public ComponentNotFoundException(string type)
      : base("Component '"+type+"' not found.") {}
  }

  [Serializable]
  public class Entity
  {
    public Entity(string id)
    {
      this.id = id;
    }


    public bool TryGet<T>(out T component)
      where T : Component
    {
      Type type = Component.FamilyOf<T>();

      if (components.ContainsKey(type))
      {
        component = (T)components[type];
        return true;
      }
      else
      {
        return false;
      }
    }


    public T Get<T>()
      where T : Component
    {
      Type type = Component.FamilyOf<T>();

      if (components.ContainsKey(type))
      {
        return (T)components[type];
      }
      else
      {
        throw new ComponentNotFoundException(type.ToString());
      }
    }


    public Entity Set(Component c)
    {
      if (c == null)
      {
        throw new ArgumentNullException("c");
      }

      Type type = c.Family;

      if (components.ContainsKey(type)) {
        components[type].Detach();
      }

      components[type] = c;
      c.Attach(this);

      return this;
    }


    public void Clear(Type type)
    {
      if (components.ContainsKey(type)) {
        components[type].Detach();
      }

      components.Remove(type);
    }


    public void Clear()
    {
      components.Clear();
    }


    public string Id { get { return id; } }


    private string id;

    private IDictionary<Type, Component> components =
      new Dictionary<Type, Component>();
  }
}