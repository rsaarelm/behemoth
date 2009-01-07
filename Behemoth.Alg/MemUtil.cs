using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;


namespace Behemoth.Alg
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

      // Make settings that don't try to read DTD.
      var settings = new XmlReaderSettings();
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
  }
}