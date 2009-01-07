using System;
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
  public static class TiledImport
  {
    public static void LoadMapData(
      byte[] data, out int width, out int height, out int[] tiles)
    {
      // XXX: Currently imports only the first layer, there may be several.
      // XXX: Currently doesn't import any objects.

      var doc = MemUtil.ReadXml(data);

      var layer = doc.Element("map").Element("layer");

      width = Int32.Parse(layer.Attribute("width").Value);
      height = Int32.Parse(layer.Attribute("height").Value);

      var dataElt = layer.Element("data");

      // XXX: value should be "base64", but is not checked here.
      if (dataElt.Attribute("encoding") == null)
      {
        throw new ArgumentException("Can't handle data that isn't binary encoded.", "data");
      }

      byte[] result = System.Convert.FromBase64String(dataElt.Value);

      // XXX: value should be "gzip", but is not checked here.
      if (dataElt.Attribute("compression") != null)
      {
        result = MemUtil.Ungzip(result);
      }

      Debug.Assert(result.Length == width * height * 4, "Bad tile data dimensions.");

      tiles = MemUtil.ToInt32Array(result);
    }
  }
}