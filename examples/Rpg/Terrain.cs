using System;
using System.Collections.Generic;

using Behemoth.Alg;

namespace Rpg
{
  [Serializable]
  public struct TerrainTile
  {
    public TerrainTile(TerrainData type)
    {
      Type = type;
    }

    // More stuff here if needed.
    public TerrainData Type;
  }


  public enum TerrainFamily
  {
    NoTerrain = 0,
    Wall,
    TransparentWall,
    IllusionWall,
    WallGap,
    FenceWall,
    Pillar,
    Ground,
    Water,
    Lava,
    Void
  }


  [Serializable]
  public struct TerrainData
  {
    public string Name { get { return name; } }
    public TerrainFamily Family { get { return family; } }

    public int Icon { get { return icon; } }
    // Icon for wall tiles which have another wall tile in front of them.
    public int BackIcon { get { return backIcon; } }

    public TerrainData(string name, TerrainFamily family, int icon)
    {
      if (TerrainUtil.IsWallType(family))
      {
        throw new ArgumentException("Wall-type families must be specified with a backicon.", "family");
      }
      this.name = name;
      this.family = family;
      this.icon = icon;
      this.backIcon = icon;
    }


    public TerrainData(string name, TerrainFamily family, int icon, int backIcon)
    {
      this.name = name;
      this.family = family;
      this.icon = icon;
      this.backIcon = backIcon;
    }


    /// <summary>
    /// Build a TerrainData from raw value list. Valid data formats are (name
    /// : string, family enum name : string, icon : int) and (name : string,
    /// family enum name : string, icon : int, backIcon : int).
    /// </summary>
    public static TerrainData FromDataRow(Object[] dataRow)
    {
      var validator = Alg.Either(
        Alg.ArrayP(
          Alg.TypeP<Object>(typeof(string)),
          Alg.TypeP<Object>(typeof(string)),
          Alg.TypeP<Object>(typeof(int))),
        Alg.ArrayP(
          Alg.TypeP<Object>(typeof(string)),
          Alg.TypeP<Object>(typeof(string)),
          Alg.TypeP<Object>(typeof(int)),
          Alg.TypeP<Object>(typeof(int))));

      var err = validator(dataRow);
      if (err != null)
      {
        throw new ArgumentException("Bad data: "+err, "dataRow");
      }

      if (dataRow.Length == 3)
      {
        return new TerrainData(
          (string)dataRow[0],
          (TerrainFamily)Enum.Parse(typeof(TerrainFamily), (string)dataRow[1]),
          (int)dataRow[2]);
      }
      else if (dataRow.Length == 4)
      {
        return new TerrainData(
          (string)dataRow[0],
          (TerrainFamily)Enum.Parse(typeof(TerrainFamily), (string)dataRow[1]),
          (int)dataRow[2], (int)dataRow[3]);
      }
      else
      {
        throw new ArgumentException("Bad number of arguments in data row", "dataRow");
      }
    }


    private string name;
    private TerrainFamily family;
    private int icon;
    private int backIcon;
  }


  /// <summary>
  /// Find a terrain data value corresponding to an icon index from a list of
  /// terrain data values. Throws KeyNotFoundException if no value in the list
  /// has the icon as either it's Icon or BackIcon value.
  /// </summary>
  public static class TerrainUtil
  {
    public static TerrainData FindData(IEnumerable<TerrainData> data, int icon)
    {
      foreach (var terrain in data)
      {
        if (terrain.Icon == icon || terrain.BackIcon == icon)
        {
          return terrain;
        }
      }
      throw new KeyNotFoundException("No TerrainData has icon "+icon+".");
    }


    public static bool IsWallType(TerrainFamily family)
    {
      return
        family == TerrainFamily.NoTerrain ||
        family == TerrainFamily.Wall ||
        family == TerrainFamily.TransparentWall ||
        family == TerrainFamily.IllusionWall ||
        family == TerrainFamily.WallGap;
    }


    public static bool IsWall(TerrainTile tile)
    {
      return IsWallType(tile.Type.Family);
    }


    /// <summary>
    /// Return whether a terrain blocks movement along the ground.
    /// </summary>
    public static bool BlocksMoving(TerrainTile tile)
    {
      var data = tile.Type;
      return
        data.Family == TerrainFamily.NoTerrain ||
        data.Family == TerrainFamily.Wall ||
        data.Family == TerrainFamily.TransparentWall ||
        data.Family == TerrainFamily.FenceWall ||
        data.Family == TerrainFamily.Pillar ||
        data.Family == TerrainFamily.Water ||
        data.Family == TerrainFamily.Lava ||
        data.Family == TerrainFamily.Void;
    }


    /// <summary>
    /// Return whether a terrain blocks flying movement.
    /// </summary>
    public static bool BlocksFlying(TerrainTile tile)
    {
      var data = tile.Type;
      return
        data.Family == TerrainFamily.NoTerrain ||
        data.Family == TerrainFamily.Wall ||
        data.Family == TerrainFamily.TransparentWall ||
        data.Family == TerrainFamily.FenceWall ||
        data.Family == TerrainFamily.Pillar;
    }


    /// <summary>
    /// Return whether a terrain blocks visual contact.
    /// </summary>
    public static bool BlocksSight(TerrainTile tile)
    {
      // There's currently no non-wall sight-blocking terrain.
      var data = tile.Type;
      return
        data.Family == TerrainFamily.NoTerrain ||
        data.Family == TerrainFamily.Wall ||
        data.Family == TerrainFamily.IllusionWall;
    }


    /// <summary>
    /// Return whether a terrain blocks small flying projectiles.
    /// </summary>
    public static bool BlocksShot(TerrainTile tile)
    {
      var data = tile.Type;
      return
        data.Family == TerrainFamily.NoTerrain ||
        data.Family == TerrainFamily.Wall ||
        data.Family == TerrainFamily.TransparentWall ||
        data.Family == TerrainFamily.Pillar;
    }
  }
}