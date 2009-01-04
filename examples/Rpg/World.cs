using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Behemoth.Alg;

namespace Rpg
{
  [Serializable]
  public class World
  {
    /// <summary>
    /// Make a new entity using an entity template.
    /// </summary>
    public Entity Spawn(EntityTemplate template)
    {
      string id = guids.Next(template.Name);
      Entity result = template.Make(id);
      entities[id] = result;
      return result;
    }


    /// <summary>
    /// Make a new empty entity in the world.
    /// </summary>
    /// <params name="prefix">
    /// A mnemonic prefix string for the entity's guid.
    /// </params>
    public Entity MakeEntity(String prefix)
    {
      string id = guids.Next(prefix);
      Entity result = new Entity(id);
      entities[id] = result;
      return result;
    }


    public IEnumerable<Entity> Entities
    {
      get
      {
        return new List<Entity>(entities.Values);
      }
    }


    public Field3<Terrain> Space { get { return space; } }


    public Random Rng { get { return rng; } }


    /// <summary>
    /// Freeform global flags and variables.
    /// </summary>
    public Properties<String, Object> Globals { get { return globals; } }


    // XXX: Use something like Mersenne Twister instead of the default rng.
    private Random rng = new Random();

    private Behemoth.Alg.Guid guids = new Behemoth.Alg.Guid();
    private IDictionary<String, Entity> entities = new Dictionary<String, Entity>();

    Field3<Terrain> space = new Field3<Terrain>();

    private Properties<String, Object> globals = new Properties<String, Object>();
  }
}