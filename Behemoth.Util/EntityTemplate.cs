using System;
using System.Collections.Generic;

namespace Behemoth.Util
{
  /// <summary>
  /// A collection of component templates for generating new entities.
  /// </summary>
  public class EntityTemplate
  {
    public EntityTemplate() {}


    public EntityTemplate(params ComponentTemplate[] templates)
    {
      foreach (var t in templates)
      {
        AddComponent(t);
      }
    }


    public EntityTemplate AddComponent(ComponentTemplate template)
    {
      components.Add(template);
      template.AddToEntityTemplateHook(this);
      return this;
    }

    
    public EntityTemplate AddProps(params Object[] args)
    {
      Alg.ApplyPairs<string, Object>((k, v) => Prop.Set(k, v), args);
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


    public string Name = String.Empty;


    /// <summary>
    /// Miscellaneous data about this template.
    /// </summary>
    public Properties<string, Object> Prop = new Properties<string, Object>();


    private IList<ComponentTemplate> components = new List<ComponentTemplate>();
  }
}