using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Behemoth.Alg
{
  public struct Color
  {
    public byte R;
    public byte G;
    public byte B;
    public byte A;


    public Color(byte r, byte g, byte b)
    {
      R = r;
      G = g;
      B = b;
      A = 0xff;
    }


    public Color(byte r, byte g, byte b, byte a)
    {
      R = r;
      G = g;
      B = b;
      A = a;
    }


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

  }
}