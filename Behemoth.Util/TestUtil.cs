using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Behemoth.Util
{
  public static class TestUtil
  {
    public static T RoundtripSerialize<T>(T val)
    {
      Stream stream = new MemoryStream();
      IFormatter formatter = new BinaryFormatter();

      formatter.Serialize(stream, val);
      stream.Seek(0, SeekOrigin.Begin);

      return (T)formatter.Deserialize(stream);
    }
  }
}