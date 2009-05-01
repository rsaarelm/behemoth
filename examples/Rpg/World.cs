using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Behemoth.Util;

namespace Rpg
{
  [Serializable]
  public class World
  {
    public World()
    {
    }


    /// <summary>
    /// Make a new entity using an entity template.
    /// </summary>
    public Entity Create(EntityTemplate template)
    {
      string id = guids.Next(template.Name);
      Entity result = template.Make(id);

      return result;
    }


    public Entity Create(string templateName)
    {
      return Create(templates[templateName]);
    }


    /// <summary>
    /// Make a new entity usind a template and place it in the world.
    /// </summary>
    public Entity Spawn(EntityTemplate template, Vec3 pos)
    {
      var result = Create(template);
      result.Get<CCore>().Pos = pos;
      Add(result);
      return result;
    }


    public Entity Spawn(string templateName, Vec3 pos)
    {
      return Spawn(templates[templateName], pos);
    }


    public void Add(string name, EntityTemplate template)
    {
      templates[name] = template;
    }


    /// <summary>
    /// Build a new empty entity. The entity isn't added to the world yet, but
    /// does get a valid Id.
    /// </summary>
    /// <params name="prefix">
    /// A mnemonic prefix string for the entity's guid.
    /// </params>
    public Entity MakeEntity(String prefix)
    {
      string id = guids.Next(prefix);
      Entity result = new Entity(id);
      return result;
    }


    public void Add(Entity entity)
    {
      if (entities.ContainsKey(entity.Id))
      {
        throw new ArgumentException("Entity already present in world", "entity");
      }
      Register(entity);
    }


    public void Remove(Entity entity)
    {
      if (!entities.ContainsKey(entity.Id))
      {
        throw new ArgumentException("Entity to be removed not found in world", "entity");
      }
      Unregister(entity);
    }


    public IEnumerable<Entity> Entities
    {
      get
      {
        return new List<Entity>(entities.Values);
      }
    }


    public IEnumerable<Entity> EntitiesIn(Vec3 pos)
    {
      // XXX: Iterating through every entity. May be very inefficient.
      return
        from e in Entities where Query.Pos(e) == pos select e;
    }


    public IEnumerable<Entity> EntitiesInRect(int x, int y, int z, int width, int height)
    {
      // XXX: Iterating through every entity. May be very inefficient.
      return
        from e in Entities
        where Query.IsInRect(e, x, y, z, width, height)
        select e;
    }


    /// <summary>
    /// Enumerate the entities in the rougly gameplay-relevant region around
    /// the given pos. Things dealing with AI perception, UI display of nearby
    /// objects and similar distance-constrained operations should use this.
    /// </summary>
    public IEnumerable<Entity> EntitiesNear(Vec3 pos)
    {
      // XXX: Doesn't do any spatial indexing yet, just iterates everything.
      return Entities;
    }



    public Field3<TerrainTile> Space { get { return space; } }


    public Random Rng { get { return rng; } }


    /// <summary>
    /// Freeform global flags and variables.
    /// </summary>
    public Properties<String, Object> Globals { get { return globals; } }


    public World AddTerrain(TerrainData data)
    {
      terrainData[data.Name] = data;
      return this;
    }


    public TerrainData GetTerrain(string name)
    {
      return terrainData[name];
    }


    private void Register(Entity entity)
    {
      CCore core;
      if (entity.TryGet(out core)) {
        core.World = this;
      }
      else
      {
        Console.WriteLine(
          "Warning: Registering entity without a core component. "+
          "It won't be able to refer back to World.");
      }

      entities[entity.Id] = entity;
    }


    private void Unregister(Entity entity)
    {
      CCore core;
      if (entity.TryGet(out core)) {
        core.World = null;
      }
      else
      {
        Console.WriteLine(
          "Warning: Unregistering entity without a core component.");
      }

      entities.Remove(entity.Id);
    }




    // XXX: Use something like Mersenne Twister instead of the default rng.
    private Random rng = new Random();

    private Behemoth.Util.Guid guids = new Behemoth.Util.Guid();
    private IDictionary<String, Entity> entities = new Dictionary<String, Entity>();

    Field3<TerrainTile> space = new Field3<TerrainTile>();

    private Properties<String, Object> globals = new Properties<String, Object>();

    private IDictionary<string, TerrainData> terrainData = new Dictionary<string, TerrainData>();

    private IDictionary<string, EntityTemplate> templates =
      new Dictionary<string, EntityTemplate>();

  }

}