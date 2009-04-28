using System;
using System.Collections.Generic;

using Behemoth.Util;

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
    public Color Foreground { get { return foreground; } }
    public Color Background { get { return background; } }

    public TerrainData(
      string name, TerrainFamily family, int icon,
      Color foreground, Color background)
    {
      this.name = name;
      this.family = family;
      this.icon = icon;
      this.foreground = foreground;
      this.background = background;
    }


    public TerrainData(
      string name, TerrainFamily family, char icon,
      Color foreground, Color background) :  this(name, family, (int)icon, foreground, background)
    {
    }


    /// <summary>
    /// Build a TerrainData from raw value list. Valid data formats are (name
    /// : string, family enum name : string, icon : int) and (name : string,
    /// family enum name : string, icon : int, foreground color :
    /// Behemoth.Util.Color, background color : Behemoth.Util.Color).
    /// </summary>
    public static TerrainData FromDataRow(Object[] dataRow)
    {
      var validator = Alg.ArrayP(
        Alg.TypeP<Object>(typeof(string)),
        Alg.TypeP<Object>(typeof(string)),
        Alg.Either(Alg.TypeP<Object>(typeof(char)), Alg.TypeP<Object>(typeof(int))),
        Alg.TypeP<Object>(typeof(Color)),
        Alg.TypeP<Object>(typeof(Color)));

      var err = validator(dataRow);
      if (err != null)
      {
        throw new ArgumentException("Bad data: "+err, "dataRow");
      }

      // XXX: I could probably librarize calling a function based on the
      // function, a datarow and a datarow interpretation spec, with the
      // signature of the function to call based on the spec...

      int icon = dataRow[2] is char ? (char)dataRow[2] : (int)dataRow[2];

      return new TerrainData(
        (string)dataRow[0],
        (TerrainFamily)Enum.Parse(typeof(TerrainFamily), (string)dataRow[1]),
        icon,
        (Color)dataRow[3],
        (Color)dataRow[4]);
    }


    private string name;
    private TerrainFamily family;
    private int icon;
    private Color foreground;
    private Color background;
  }


  /// <summary>
  /// Find a terrain data value corresponding to an icon index from a list of
  /// terrain data values. Throws KeyNotFoundException if no value in the list
  /// has the icon as it's Icon value.
  /// </summary>
  public static class TerrainUtil
  {
    public static TerrainData FindData(IEnumerable<TerrainData> data, int icon)
    {
      foreach (var terrain in data)
      {
        if (terrain.Icon == icon)
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