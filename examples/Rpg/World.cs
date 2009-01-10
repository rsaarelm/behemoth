using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Behemoth.Alg;

namespace Rpg
{
  [Serializable]
  public class World
  {
    public World()
    {
      terrainCache = new EasyCache<int, TerrainData>(
        (icon) => TerrainUtil.FindData(terrainData, icon),
        (index) => {});
    }


    /// <summary>
    /// Make a new entity using an entity template.
    /// </summary>
    public Entity Spawn(EntityTemplate template)
    {
      string id = guids.Next(template.Name);
      Entity result = template.Make(id);

      return result;
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

    public Field3<TerrainTile> Space { get { return space; } }


    public Random Rng { get { return rng; } }


    /// <summary>
    /// Freeform global flags and variables.
    /// </summary>
    public Properties<String, Object> Globals { get { return globals; } }


    public World AddTerrain(TerrainData data)
    {
      terrainData.Add(data);
      return this;
    }


    public TerrainData GetTerrain(int icon)
    {
      return terrainCache[icon];
    }


    private void Register(Entity entity)
    {
      CoreComponent core;
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


    // XXX: Use something like Mersenne Twister instead of the default rng.
    private Random rng = new Random();

    private Behemoth.Alg.Guid guids = new Behemoth.Alg.Guid();
    private IDictionary<String, Entity> entities = new Dictionary<String, Entity>();

    Field3<TerrainTile> space = new Field3<TerrainTile>();

    private Properties<String, Object> globals = new Properties<String, Object>();

    private IList<TerrainData> terrainData = new List<TerrainData>();

    private Cache<int, TerrainData> terrainCache;
  }

}