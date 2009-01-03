using System;
using System.Collections.Generic;

namespace Behemoth.Alg
{
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
      component = Get<T>();
      return component != null;
    }


    public T Get<T>()
      where T : Component
    {
      String type = Component.FamilyOf<T>();

      if (components.ContainsKey(type))
      {
        return (T)components[type];
      }
      else
      {
        return null;
      }
    }


    public Entity Set(Component c)
    {
      String type = c.Family;

      components[type] = c;

      return this;
    }


    public void Clear()
    {
      components.Clear();
    }


    public string Id { get { return id; } }


    private string id;

    private IDictionary<String, Component> components =
      new Dictionary<String, Component>();
  }
}