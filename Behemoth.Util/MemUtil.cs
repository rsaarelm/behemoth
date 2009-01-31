using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;


namespace Behemoth.Util
{
  /// <summary>
  /// Utils related to the innards of the runtime.
  /// </summary>
  public static class MemUtil
  {
    /// <summary>
    /// Look for a static method from a class and then from its parents. Call
    /// the method if found and return the result.
    /// </summary>
    /// <params name="type">
    /// The type of the class where the static method lookup starts.
    /// </params>
    /// <type name="methodName">
    /// The name of the method to look for.
    /// </type>
    /// <type name="args">
    /// Arguments passed to the static method call.
    /// </type>
    /// <returns>
    /// The result of the static method call, if the method was found. Null
    /// otherwise.
    /// </returns>
    public static Object CallInheritedStaticMethod(
      Type type, string methodName, Object[] args)
    {
      try
      {
        return type.InvokeMember(
          methodName, BindingFlags.Static | BindingFlags.InvokeMethod,
          null, null, args);
      }
      catch (MissingMethodException)
      {
        if (type.BaseType != null)
        {
          return CallInheritedStaticMethod(type.BaseType, methodName, args);
        }
        else
        {
          return null;
        }
      }
    }


    /// <summary>
    /// Read an XDocument object from a byte array representing an XML file.
    /// </summary>
    public static XDocument ReadXml(byte[] data)
    {
      var stream = new MemoryStream(data);

      // Make settings that don't try to read the DTD...
      var settings = new XmlReaderSettings();
      settings.XmlResolver = null;
      // ...nor complain about the presence of the DTD.
      settings.ProhibitDtd = false;

      var reader = XmlReader.Create(stream, settings);

      return XDocument.Load(reader);
    }


    public static byte[] ReadBytes(Stream stream)
    {
      var result = new MemoryStream();
      var chunkSize = 8192;
      byte[] buffer = new byte[chunkSize];

      while (true)
      {
        var bytesRead = stream.Read(buffer, 0, chunkSize);

        if (bytesRead == 0)
        {
          // EOF.
          break;
        }
        result.Write(buffer, 0, bytesRead);
      }

      return result.ToArray();
    }


    /// <summary>
    /// Decompress a GZipped data block.
    /// </summary>
    public static byte[] Ungzip(byte[] data)
    {
      var gzStream = new GZipStream(
        new MemoryStream(data), CompressionMode.Decompress);

      return ReadBytes(gzStream);
    }


    /// <summary>
    /// Map a byte array into an Int32 array so that each consecutive chunk of
    /// four bytes becomes one int.
    /// </summary>
    public static int[] ToInt32Array(byte[] byteArray)
    {
      int length = byteArray.Length / 4;
      int[] result = new int[length];

      for (int i = 0; i < length; i++)
      {
        result[i] = BitConverter.ToInt32(byteArray, i * 4);
      }

      return result;
    }


    /// <summary>
    /// Return an integer attribute from an XML node.
    /// </summary>
    public static int IntAttribute(XElement elt, string name)
    {
      return Int32.Parse(elt.Attribute(name).Value);
    }


    /// <summary>
    /// Pad an array with the default values of the generic type until it is
    /// at least minLength in size.
    /// </summary>
    public static T[] Pad<T>(T[] array, int minLength)
    {
      T[] result = new T[Math.Max(minLength, array.Length)];
      Array.Copy(array, 0, result, 0, array.Length);
      return result;
    }


    /// <summary>
    /// Get the dimensions of a 2d array.
    /// </summary>
    public static void GetArrayDims<T>(
      T[,] array, out int width, out int height)
    {
      width = array.GetLength(1);
      height = array.GetLength(0);
    }


    /// <summary>
    /// Create a two-dimensional array with values sampled from a function.
    /// The value are assigned with the following form: result[y, x] =
    /// sourceFunc(x, y).
    /// </summary>
    public static T[,] MakeArray<T>(
      int width, int height, Func<int, int, T> sourceFunc)
    {
      T[,] result = new T[height, width];
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          result[y, x] = sourceFunc(x, y);
        }
      }
      return result;
    }



    /// <summary>
    /// Create a two-dimensional array sampling the source function for cell
    /// centers from range [-1, 1] in x and y.
    /// </summary>
    public static T[,] MakeArray<T>(
      int width, int height, Func<double, double, T> sourceFunc)
    {
      T[,] result = new T[height, width];
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          // Point the sampling positions at points in [0, 1] corresponding to
          // cell centers.
          double sampleX = (2.0 + (double)x * 4.0) / (width * 4.0) - 1;
          double sampleY = (2.0 + (double)y * 4.0) / (height * 4.0) - 1;

          result[y, x] = sourceFunc(sampleX, sampleY);
        }
      }
      return result;
    }
  }
}