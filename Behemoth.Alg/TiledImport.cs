using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Behemoth.Alg
{
  /// <summary>
  /// Utilities for importing maps made using the <a
  /// href="http://mapeditor.org/">Tiled</a> editor.
  /// </summary>
  /// <remarks>
  /// The Tiled map must use binary encoded layer data and all layers are
  /// expected to be of the same size.
  /// </remarks>
  /// <params name="tilesets">
  /// A mapping of tileset names to the first tile indices of these sets.
  /// </params>
  /// <params name="layers">
  /// A list of layer name and layer tile data pairs.
  /// </params>
  public static class TiledImport
  {
    public static void LoadMapData(
      byte[] data, out int width, out int height,
      out IDictionary<String, int> tilesets,
      out IList<Tuple2<String, int[]>> layers)
    {
      // XXX: Currently doesn't import any objects.

      var map = MemUtil.ReadXml(data).Element("map");

      width = -1;
      height = -1;

      tilesets = new Dictionary<String, int>();
      layers = new List<Tuple2<String, int[]>>();

      foreach (var tileset in map.Elements("tileset"))
      {
        tilesets[tileset.Attribute("name").Value] =
          MemUtil.IntAttribute(tileset, "firstgid");
      }


      foreach (var layer in map.Elements("layer"))
      {
        // XXX: Assuming that each layer has the same size.
        width = MemUtil.IntAttribute(layer, "width");
        height = MemUtil.IntAttribute(layer, "height");

        String name = layer.Attribute("name").Value;
        int[] tiles = GetLayerData(layer.Element("data"));

        Debug.Assert(tiles.Length == width * height, "Bad tile data dimensions.");

        layers.Add(new Tuple2<String, int[]>(name, tiles));
      }
    }


    static int[] GetLayerData(XElement dataElt)
    {
      // XXX: value should be "base64", but is not checked here.
      if (dataElt.Attribute("encoding") == null)
      {
        throw new ArgumentException("Can't handle data that isn't binary encoded.", "data");
      }

      byte[] bytes = System.Convert.FromBase64String(dataElt.Value);

      // XXX: value should be "gzip", but is not checked here.
      if (dataElt.Attribute("compression") != null)
      {
        bytes = MemUtil.Ungzip(bytes);
      }

      return MemUtil.ToInt32Array(bytes);
    }
  }
}