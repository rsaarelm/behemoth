using System;
using System.Collections.Generic;

namespace Behemoth.Alg
{
  using ComponentFamily = String;

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
      ComponentFamily type = Component.FamilyOf<T>();

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


    public Entity Set(Component c)
    {
      ComponentFamily type = c.Family;

      components[type] = c;

      return this;
    }


    public void Clear()
    {
      components.Clear();
    }


    public string Id { get { return id; } }


    private string id;

    private IDictionary<ComponentFamily, Component> components =
      new Dictionary<ComponentFamily, Component>();
  }
}