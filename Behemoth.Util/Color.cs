using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Behemoth.Util
{
  public struct Color
  {
    public byte R { get { return r; } }
    public byte G { get { return g; } }
    public byte B { get { return b; } }
    public byte A { get { return a; } }


    private byte r;
    private byte g;
    private byte b;
    private byte a;


    public Color(byte r, byte g, byte b) : this(r, g, b, 0xff) {}


    public Color(byte r, byte g, byte b, byte a)
    {
      this.r = r;
      this.g = g;
      this.b = b;
      this.a = a;
    }


    public Color(double r, double g, double b) : this(r, g, b, 1.0) {}


    public Color(double r, double g, double b, double a) :
      this(
        (byte)(r * 255),
        (byte)(g * 255),
        (byte)(b * 255),
        (byte)(a * 255))
    {}


    public Color(String desc)
    {
      this = ParseColor(desc);
    }


    public override bool Equals(Object obj)
    {
      return obj is Color && this == (Color)obj;
    }


    public override int GetHashCode()
    {
      return Num.HashPoint(R, G, B, A);
    }


    public static bool operator==(Color lhs, Color rhs)
    {
      return lhs.R == rhs.R && lhs.G == rhs.G && lhs.B == rhs.B && lhs.A == rhs.A;
    }


    public static bool operator!=(Color lhs, Color rhs)
    {
      return !(lhs == rhs);
    }


    /// <summary>
    /// Parses string color descriptions to color structs.<br/>
    /// "#123456"   => R: 0x12, G: 0x34, B: 0x56, A: 0xff<br/>
    /// "#12345678" => R: 0x12, G: 0x34, B: 0x56, A: 0x78<br/>
    /// "#123"      => R: 0x11, G: 0x22, B: 0x33, A: 0xff<br/>
    /// "#1234"     => R: 0x11, G: 0x22, B: 0x33, A: 0x44<br/>
    /// </summary>
    public static Color ParseColor(String desc)
    {
      Match match;
      if ((match = longHexRGB.Match(desc)).Success)
      {
        return new Color(
          Byte.Parse(match.Groups["r"].Value, NumberStyles.HexNumber),
          Byte.Parse(match.Groups["g"].Value, NumberStyles.HexNumber),
          Byte.Parse(match.Groups["b"].Value, NumberStyles.HexNumber));
      }
      else if ((match = longHexRGBA.Match(desc)).Success)
      {
        return new Color(
          Byte.Parse(match.Groups["r"].Value, NumberStyles.HexNumber),
          Byte.Parse(match.Groups["g"].Value, NumberStyles.HexNumber),
          Byte.Parse(match.Groups["b"].Value, NumberStyles.HexNumber),
          Byte.Parse(match.Groups["a"].Value, NumberStyles.HexNumber));
      }
      else if ((match = shortHexRGBA.Match(desc)).Success)
      {
        byte r = Byte.Parse(match.Groups["r"].Value, NumberStyles.HexNumber);
        byte g = Byte.Parse(match.Groups["g"].Value, NumberStyles.HexNumber);
        byte b = Byte.Parse(match.Groups["b"].Value, NumberStyles.HexNumber);
        byte a = Byte.Parse(match.Groups["a"].Value, NumberStyles.HexNumber);
        return new Color(
          (byte)(r + r * 0x10), (byte)(g + g * 0x10),
          (byte)(b + b * 0x10), (byte)(a + a * 0x10));
      }
      else if ((match = shortHexRGB.Match(desc)).Success)
      {
        byte r = Byte.Parse(match.Groups["r"].Value, NumberStyles.HexNumber);
        byte g = Byte.Parse(match.Groups["g"].Value, NumberStyles.HexNumber);
        byte b = Byte.Parse(match.Groups["b"].Value, NumberStyles.HexNumber);
        return new Color(
          (byte)(r + r * 0x10), (byte)(g + g * 0x10),
          (byte)(b + b * 0x10), 0xff);
      }

      else
      {
        throw new ArgumentException(
          "Couldn't parse color description: '"+desc+"'.", "desc");
      }
    }

    private static Regex longHexRGB = new Regex("^#(?<r>[0-9a-fA-F]{2})(?<g>[0-9a-fA-F]{2})(?<b>[0-9a-fA-F]{2})$");
    private static Regex longHexRGBA = new Regex("^#(?<r>[0-9a-fA-F]{2})(?<g>[0-9a-fA-F]{2})(?<b>[0-9a-fA-F]{2})(?<a>[0-9a-fA-F]{2})$");
    private static Regex shortHexRGB = new Regex("^#(?<r>[0-9a-fA-F])(?<g>[0-9a-fA-F])(?<b>[0-9a-fA-F])$");
    private static Regex shortHexRGBA = new Regex("^#(?<r>[0-9a-fA-F])(?<g>[0-9a-fA-F])(?<b>[0-9a-fA-F])(?<a>[0-9a-fA-F])$");

    // Named HTML colors.
    public static readonly Color Aliceblue = ParseColor("#F0F8FF");
    public static readonly Color Antiquewhite = ParseColor("#FAEBD7");
    public static readonly Color Aqua = ParseColor("#00FFFF");
    public static readonly Color Aquamarine = ParseColor("#7FFFD4");
    public static readonly Color Azure = ParseColor("#F0FFFF");
    public static readonly Color Beige = ParseColor("#F5F5DC");
    public static readonly Color Bisque = ParseColor("#FFE4C4");
    public static readonly Color Black = ParseColor("#000000");
    public static readonly Color Blanchedalmond = ParseColor("#FFEBCD");
    public static readonly Color Blue = ParseColor("#0000FF");
    public static readonly Color Blueviolet = ParseColor("#8A2BE2");
    public static readonly Color Brown = ParseColor("#A52A2A");
    public static readonly Color Burlywood = ParseColor("#DEB887");
    public static readonly Color Cadetblue = ParseColor("#5F9EA0");
    public static readonly Color Chartreuse = ParseColor("#7FFF00");
    public static readonly Color Chocolate = ParseColor("#D2691E");
    public static readonly Color Coral = ParseColor("#FF7F50");
    public static readonly Color Cornflowerblue = ParseColor("#6495ED");
    public static readonly Color Cornsilk = ParseColor("#FFF8DC");
    public static readonly Color Crimson = ParseColor("#DC143C");
    public static readonly Color Cyan = ParseColor("#00FFFF");
    public static readonly Color Darkblue = ParseColor("#00008B");
    public static readonly Color Darkcyan = ParseColor("#008B8B");
    public static readonly Color Darkgoldenrod = ParseColor("#B8860B");
    public static readonly Color Darkgray = ParseColor("#A9A9A9");
    public static readonly Color Darkgrey = ParseColor("#A9A9A9");
    public static readonly Color Darkgreen = ParseColor("#006400");
    public static readonly Color Darkkhaki = ParseColor("#BDB76B");
    public static readonly Color Darkmagenta = ParseColor("#8B008B");
    public static readonly Color Darkolivegreen = ParseColor("#556B2F");
    public static readonly Color Darkorange = ParseColor("#FF8C00");
    public static readonly Color Darkorchid = ParseColor("#9932CC");
    public static readonly Color Darkred = ParseColor("#8B0000");
    public static readonly Color Darksalmon = ParseColor("#E9967A");
    public static readonly Color Darkseagreen = ParseColor("#8FBC8F");
    public static readonly Color Darkslateblue = ParseColor("#483D8B");
    public static readonly Color Darkslategray = ParseColor("#2F4F4F");
    public static readonly Color Darkslategrey = ParseColor("#2F4F4F");
    public static readonly Color Darkturquoise = ParseColor("#00CED1");
    public static readonly Color Darkviolet = ParseColor("#9400D3");
    public static readonly Color Deeppink = ParseColor("#FF1493");
    public static readonly Color Deepskyblue = ParseColor("#00BFFF");
    public static readonly Color Dimgray = ParseColor("#696969");
    public static readonly Color Dimgrey = ParseColor("#696969");
    public static readonly Color Dodgerblue = ParseColor("#1E90FF");
    public static readonly Color Firebrick = ParseColor("#B22222");
    public static readonly Color Floralwhite = ParseColor("#FFFAF0");
    public static readonly Color Forestgreen = ParseColor("#228B22");
    public static readonly Color Fuchsia = ParseColor("#FF00FF");
    public static readonly Color Gainsboro = ParseColor("#DCDCDC");
    public static readonly Color Ghostwhite = ParseColor("#F8F8FF");
    public static readonly Color Gold = ParseColor("#FFD700");
    public static readonly Color Goldenrod = ParseColor("#DAA520");
    public static readonly Color Gray = ParseColor("#808080");
    public static readonly Color Grey = ParseColor("#808080");
    public static readonly Color Green = ParseColor("#008000");
    public static readonly Color Greenyellow = ParseColor("#ADFF2F");
    public static readonly Color Honeydew = ParseColor("#F0FFF0");
    public static readonly Color Hotpink = ParseColor("#FF69B4");
    public static readonly Color Indianred = ParseColor("#CD5C5C");
    public static readonly Color Indigo = ParseColor("#4B0082");
    public static readonly Color Ivory = ParseColor("#FFFFF0");
    public static readonly Color Khaki = ParseColor("#F0E68C");
    public static readonly Color Lavender = ParseColor("#E6E6FA");
    public static readonly Color Lavenderblush = ParseColor("#FFF0F5");
    public static readonly Color Lawngreen = ParseColor("#7CFC00");
    public static readonly Color Lemonchiffon = ParseColor("#FFFACD");
    public static readonly Color Lightblue = ParseColor("#ADD8E6");
    public static readonly Color Lightcoral = ParseColor("#F08080");
    public static readonly Color Lightcyan = ParseColor("#E0FFFF");
    public static readonly Color Lightgoldenrodyellow = ParseColor("#FAFAD2");
    public static readonly Color Lightgray = ParseColor("#D3D3D3");
    public static readonly Color Lightgrey = ParseColor("#D3D3D3");
    public static readonly Color Lightgreen = ParseColor("#90EE90");
    public static readonly Color Lightpink = ParseColor("#FFB6C1");
    public static readonly Color Lightsalmon = ParseColor("#FFA07A");
    public static readonly Color Lightseagreen = ParseColor("#20B2AA");
    public static readonly Color Lightskyblue = ParseColor("#87CEFA");
    public static readonly Color Lightslategray = ParseColor("#778899");
    public static readonly Color Lightslategrey = ParseColor("#778899");
    public static readonly Color Lightsteelblue = ParseColor("#B0C4DE");
    public static readonly Color Lightyellow = ParseColor("#FFFFE0");
    public static readonly Color Lime = ParseColor("#00FF00");
    public static readonly Color Limegreen = ParseColor("#32CD32");
    public static readonly Color Linen = ParseColor("#FAF0E6");
    public static readonly Color Magenta = ParseColor("#FF00FF");
    public static readonly Color Maroon = ParseColor("#800000");
    public static readonly Color Mediumaquamarine = ParseColor("#66CDAA");
    public static readonly Color Mediumblue = ParseColor("#0000CD");
    public static readonly Color Mediumorchid = ParseColor("#BA55D3");
    public static readonly Color Mediumpurple = ParseColor("#9370D8");
    public static readonly Color Mediumseagreen = ParseColor("#3CB371");
    public static readonly Color Mediumslateblue = ParseColor("#7B68EE");
    public static readonly Color Mediumspringgreen = ParseColor("#00FA9A");
    public static readonly Color Mediumturquoise = ParseColor("#48D1CC");
    public static readonly Color Mediumvioletred = ParseColor("#C71585");
    public static readonly Color Midnightblue = ParseColor("#191970");
    public static readonly Color Mintcream = ParseColor("#F5FFFA");
    public static readonly Color Mistyrose = ParseColor("#FFE4E1");
    public static readonly Color Moccasin = ParseColor("#FFE4B5");
    public static readonly Color Navajowhite = ParseColor("#FFDEAD");
    public static readonly Color Navy = ParseColor("#000080");
    public static readonly Color Oldlace = ParseColor("#FDF5E6");
    public static readonly Color Olive = ParseColor("#808000");
    public static readonly Color Olivedrab = ParseColor("#6B8E23");
    public static readonly Color Orange = ParseColor("#FFA500");
    public static readonly Color Orangered = ParseColor("#FF4500");
    public static readonly Color Orchid = ParseColor("#DA70D6");
    public static readonly Color Palegoldenrod = ParseColor("#EEE8AA");
    public static readonly Color Palegreen = ParseColor("#98FB98");
    public static readonly Color Paleturquoise = ParseColor("#AFEEEE");
    public static readonly Color Palevioletred = ParseColor("#D87093");
    public static readonly Color Papayawhip = ParseColor("#FFEFD5");
    public static readonly Color Peachpuff = ParseColor("#FFDAB9");
    public static readonly Color Peru = ParseColor("#CD853F");
    public static readonly Color Pink = ParseColor("#FFC0CB");
    public static readonly Color Plum = ParseColor("#DDA0DD");
    public static readonly Color Powderblue = ParseColor("#B0E0E6");
    public static readonly Color Purple = ParseColor("#800080");
    public static readonly Color Red = ParseColor("#FF0000");
    public static readonly Color Rosybrown = ParseColor("#BC8F8F");
    public static readonly Color Royalblue = ParseColor("#4169E1");
    public static readonly Color Saddlebrown = ParseColor("#8B4513");
    public static readonly Color Salmon = ParseColor("#FA8072");
    public static readonly Color Sandybrown = ParseColor("#F4A460");
    public static readonly Color Seagreen = ParseColor("#2E8B57");
    public static readonly Color Seashell = ParseColor("#FFF5EE");
    public static readonly Color Sienna = ParseColor("#A0522D");
    public static readonly Color Silver = ParseColor("#C0C0C0");
    public static readonly Color Skyblue = ParseColor("#87CEEB");
    public static readonly Color Slateblue = ParseColor("#6A5ACD");
    public static readonly Color Slategray = ParseColor("#708090");
    public static readonly Color Slategrey = ParseColor("#708090");
    public static readonly Color Snow = ParseColor("#FFFAFA");
    public static readonly Color Springgreen = ParseColor("#00FF7F");
    public static readonly Color Steelblue = ParseColor("#4682B4");
    public static readonly Color Tan = ParseColor("#D2B48C");
    public static readonly Color Teal = ParseColor("#008080");
    public static readonly Color Thistle = ParseColor("#D8BFD8");
    public static readonly Color Tomato = ParseColor("#FF6347");
    public static readonly Color Turquoise = ParseColor("#40E0D0");
    public static readonly Color Violet = ParseColor("#EE82EE");
    public static readonly Color Wheat = ParseColor("#F5DEB3");
    public static readonly Color White = ParseColor("#FFFFFF");
    public static readonly Color Whitesmoke = ParseColor("#F5F5F5");
    public static readonly Color Yellow = ParseColor("#FFFF00");
    public static readonly Color Yellowgreen = ParseColor("#9ACD32");
  }
}