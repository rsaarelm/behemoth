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


    public float[] FloatArray
    {
      get
      {
        return new float[] {
          (float)R / 255.0f, (float)G / 255.0f,
          (float)B / 255.0f, (float)A / 255.0f};
      }
    }


    public Color WithAlpha(byte alpha)
    {
      return new Color(R, G, B, alpha);
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
    public static readonly Color AliceBlue = ParseColor("#F0F8FF");
    public static readonly Color AntiqueWhite = ParseColor("#FAEBD7");
    public static readonly Color Aqua = ParseColor("#00FFFF");
    public static readonly Color Aquamarine = ParseColor("#7FFFD4");
    public static readonly Color Azure = ParseColor("#F0FFFF");
    public static readonly Color Beige = ParseColor("#F5F5DC");
    public static readonly Color Bisque = ParseColor("#FFE4C4");
    public static readonly Color Black = ParseColor("#000000");
    public static readonly Color BlanchedAlmond = ParseColor("#FFEBCD");
    public static readonly Color Blue = ParseColor("#0000FF");
    public static readonly Color BlueViolet = ParseColor("#8A2BE2");
    public static readonly Color Brown = ParseColor("#A52A2A");
    public static readonly Color BurlyWood = ParseColor("#DEB887");
    public static readonly Color CadetBlue = ParseColor("#5F9EA0");
    public static readonly Color Chartreuse = ParseColor("#7FFF00");
    public static readonly Color Chocolate = ParseColor("#D2691E");
    public static readonly Color Coral = ParseColor("#FF7F50");
    public static readonly Color CornflowerBlue = ParseColor("#6495ED");
    public static readonly Color Cornsilk = ParseColor("#FFF8DC");
    public static readonly Color Crimson = ParseColor("#DC143C");
    public static readonly Color Cyan = ParseColor("#00FFFF");
    public static readonly Color DarkBlue = ParseColor("#00008B");
    public static readonly Color DarkCyan = ParseColor("#008B8B");
    public static readonly Color DarkGoldenRod = ParseColor("#B8860B");
    public static readonly Color DarkGray = ParseColor("#A9A9A9");
    public static readonly Color DarkGreen = ParseColor("#006400");
    public static readonly Color DarkKhaki = ParseColor("#BDB76B");
    public static readonly Color DarkMagenta = ParseColor("#8B008B");
    public static readonly Color DarkOliveGreen = ParseColor("#556B2F");
    public static readonly Color DarkOrange = ParseColor("#FF8C00");
    public static readonly Color DarkOrchid = ParseColor("#9932CC");
    public static readonly Color DarkRed = ParseColor("#8B0000");
    public static readonly Color DarkSalmon = ParseColor("#E9967A");
    public static readonly Color DarkSeaGreen = ParseColor("#8FBC8F");
    public static readonly Color DarkSlateBlue = ParseColor("#483D8B");
    public static readonly Color DarkSlateGray = ParseColor("#2F4F4F");
    public static readonly Color DarkTurquoise = ParseColor("#2F4F4F");
    public static readonly Color DarkViolet = ParseColor("#9400D3");
    public static readonly Color DeepPink = ParseColor("#FF1493");
    public static readonly Color DeepSkyBlue = ParseColor("#00BFFF");
    public static readonly Color DimGray = ParseColor("#696969");
    public static readonly Color DodgerBlue = ParseColor("#1E90FF");
    public static readonly Color FireBrick = ParseColor("#B22222");
    public static readonly Color FloralWhite = ParseColor("#FFFAF0");
    public static readonly Color ForestGreen = ParseColor("#228B22");
    public static readonly Color Fuchsia = ParseColor("#FF00FF");
    public static readonly Color Gainsboro = ParseColor("#DCDCDC");
    public static readonly Color GhostWhite = ParseColor("#F8F8FF");
    public static readonly Color Gold = ParseColor("#FFD700");
    public static readonly Color Goldenrod = ParseColor("#DAA520");
    public static readonly Color Gray = ParseColor("#808080");
    public static readonly Color Green = ParseColor("#008000");
    public static readonly Color GreenYellow = ParseColor("#ADFF2F");
    public static readonly Color Honeydew = ParseColor("#F0FFF0");
    public static readonly Color HotPink = ParseColor("#FF69B4");
    public static readonly Color IndianRed = ParseColor("#CD5C5C");
    public static readonly Color Indigo = ParseColor("#4B0082");
    public static readonly Color Ivory = ParseColor("#FFFFF0");
    public static readonly Color Khaki = ParseColor("#F0E68C");
    public static readonly Color Lavender = ParseColor("#E6E6FA");
    public static readonly Color LavenderBlush = ParseColor("#FFF0F5");
    public static readonly Color LawnGreen = ParseColor("#7CFC00");
    public static readonly Color LemonChiffon = ParseColor("#FFFACD");
    public static readonly Color LightBlue = ParseColor("#ADD8E6");
    public static readonly Color LightCoral = ParseColor("#F08080");
    public static readonly Color LightCyan = ParseColor("#E0FFFF");
    public static readonly Color LightGoldenrodYellow = ParseColor("#FAFAD2");
    public static readonly Color LightGreen = ParseColor("#90EE90");
    public static readonly Color LightGrey = ParseColor("#D3D3D3");
    public static readonly Color LightPink = ParseColor("#FFB6C1");
    public static readonly Color LightSalmon = ParseColor("#FFA07A");
    public static readonly Color LightSeaGreen = ParseColor("#20B2AA");
    public static readonly Color LightSkyBlue = ParseColor("#87CEFA");
    public static readonly Color LightSlateGray = ParseColor("#778899");
    public static readonly Color LightSteelBlue = ParseColor("#B0C4DE");
    public static readonly Color LightYellow = ParseColor("#FFFFE0");
    public static readonly Color Lime = ParseColor("#00FF00");
    public static readonly Color LimeGreen = ParseColor("#32CD32");
    public static readonly Color Linen = ParseColor("#FAF0E6");
    public static readonly Color Magenta = ParseColor("#FF00FF");
    public static readonly Color Maroon = ParseColor("#800000");
    public static readonly Color MediumAquamarine = ParseColor("#66CDAA");
    public static readonly Color MediumBlue = ParseColor("#0000CD");
    public static readonly Color MediumOrchid = ParseColor("#BA55D3");
    public static readonly Color MediumPurple = ParseColor("#9370D8");
    public static readonly Color MediumSeaGreen = ParseColor("#3CB371");
    public static readonly Color MediumSlateBlue = ParseColor("#7B68EE");
    public static readonly Color MediumSpringGreen = ParseColor("#00FA9A");
    public static readonly Color MediumTurquoise = ParseColor("#48D1CC");
    public static readonly Color MediumVioletRed = ParseColor("#C71585");
    public static readonly Color MidnightBlue = ParseColor("#191970");
    public static readonly Color MintCream = ParseColor("#F5FFFA");
    public static readonly Color MistyRose = ParseColor("#FFE4E1");
    public static readonly Color Moccasin = ParseColor("#FFE4B5");
    public static readonly Color NavajoWhite = ParseColor("#FFDEAD");
    public static readonly Color Navy = ParseColor("#000080");
    public static readonly Color OldLace = ParseColor("#FDF5E6");
    public static readonly Color Olive = ParseColor("#808000");
    public static readonly Color OliveDrab = ParseColor("#6B8E23");
    public static readonly Color Orange = ParseColor("#FFA500");
    public static readonly Color OrangeRed = ParseColor("#FF4500");
    public static readonly Color Orchid = ParseColor("#DA70D6");
    public static readonly Color PaleGoldenrod = ParseColor("#EEE8AA");
    public static readonly Color PaleGreen = ParseColor("#98FB98");
    public static readonly Color PaleTurquoise = ParseColor("#AFEEEE");
    public static readonly Color PaleVioletRed = ParseColor("#D87093");
    public static readonly Color PapayaWhip = ParseColor("#FFEFD5");
    public static readonly Color PeachPuff = ParseColor("#FFDAB9");
    public static readonly Color Peru = ParseColor("#CD853F");
    public static readonly Color Pink = ParseColor("#FFC0CB");
    public static readonly Color Plum = ParseColor("#DDA0DD");
    public static readonly Color PowderBlue = ParseColor("#B0E0E6");
    public static readonly Color Purple = ParseColor("#800080");
    public static readonly Color Red = ParseColor("#FF0000");
    public static readonly Color RosyBrown = ParseColor("#BC8F8F");
    public static readonly Color RoyalBlue = ParseColor("#4169E1");
    public static readonly Color SaddleBrown = ParseColor("#8B4513");
    public static readonly Color Salmon = ParseColor("#FA8072");
    public static readonly Color SandyBrown = ParseColor("#F4A460");
    public static readonly Color SeaGreen = ParseColor("#2E8B57");
    public static readonly Color Seashell = ParseColor("#FFF5EE");
    public static readonly Color Sienna = ParseColor("#A0522D");
    public static readonly Color Silver = ParseColor("#C0C0C0");
    public static readonly Color SkyBlue = ParseColor("#87CEEB");
    public static readonly Color SlateBlue = ParseColor("#6A5ACD");
    public static readonly Color SlateGray = ParseColor("#708090");
    public static readonly Color Snow = ParseColor("#FFFAFA");
    public static readonly Color SpringGreen = ParseColor("#00FF7F");
    public static readonly Color SteelBlue = ParseColor("#4682B4");
    public static readonly Color Tan = ParseColor("#D2B48C");
    public static readonly Color Teal = ParseColor("#008080");
    public static readonly Color Thistle = ParseColor("#D8BFD8");
    public static readonly Color Tomato = ParseColor("#FF6347");
    public static readonly Color Turquoise = ParseColor("#40E0D0");
    public static readonly Color Violet = ParseColor("#EE82EE");
    public static readonly Color Wheat = ParseColor("#F5DEB3");
    public static readonly Color White = ParseColor("#FFFFFF");
    public static readonly Color WhiteSmoke = ParseColor("#F5F5F5");
    public static readonly Color Yellow = ParseColor("#FFFF00");
    public static readonly Color YellowGreen = ParseColor("#9ACD32");
  }
}