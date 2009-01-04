using System;
using System.Collections.Generic;

namespace Behemoth.Alg
{
  /// <summary>
  /// A collection of component templates for generating new entities.
  /// </summary>
  public class EntityTemplate
  {
    public EntityTemplate AddComponent(ComponentTemplate template)
    {
      components.Add(template);
      return this;
    }


    public Entity Make(string id)
    {
      Entity result = new Entity(id);

      foreach (ComponentTemplate template in components)
      {
        template.Make(result);
      }

      return result;
    }


    public string Name = "";


    private IList<ComponentTemplate> components = new List<ComponentTemplate>();
  }
}