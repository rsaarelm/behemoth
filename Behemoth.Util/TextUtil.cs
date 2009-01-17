using System;
using System.Collections.Generic;

namespace Behemoth.Util
{
  public static class TextUtil
  {
    public static IEnumerable<string> SplitAtNewlines(string text)
    {
      return text.Split(Alg.A('\n'));
    }


    /// <summary>
    /// Splits a single long line into bits of at most maxLength size. Tries
    /// to split at whitespace.
    /// </summary>
    public static IEnumerable<string> SplitLongLine(string text, int maxLength)
    {
      if (maxLength < 1)
      {
        throw new ArgumentException(
          "Must have positive nonzero line length.", "maxLength");
      }
      int currentLineStart = 0;
      int lastCutPos = -1;
      Func<char, bool> isWhiteSpace = (c) => Char.IsWhiteSpace(c);

      while (currentLineStart < text.Length)
      {
        if (text.Length - currentLineStart < maxLength)
        {
          yield return text.Substring(currentLineStart);
          break;
        }

        int endPos = currentLineStart + maxLength;

        int betterPos;
        if (FindCharBackwards(text, endPos, isWhiteSpace, out betterPos)
            && betterPos > currentLineStart)
        {
          endPos = betterPos;
        }

        var len = endPos - currentLineStart + 1;

        yield return text.Substring(currentLineStart, len);
        currentLineStart += len;
      }
    }


    /// <summary>
    /// Scans the string backwards from startPos, returns the first index
    /// i for which pred(str[i]) is true. Returns false if none is found.
    /// </summary>
    public static bool FindCharBackwards(
      string str, int startPos, Func<char, bool> pred, out int index)
    {
      index = -1;
      for (int i = startPos; i >= 0; i--)
      {
        if (pred(str[i]))
        {
          index = i;
          return true;
        }
      }

      return false;
    }



    /// <summary>
    /// Scans the string forwards from startPos, returns the first index i for
    /// which pred(str[i]) is true. Returns false if none is found.
    /// </summary>
    public static bool FindCharForwards(
      string str, int startPos, Func<char, bool> pred, out int index)
    {
      index = -1;
      for (int i = startPos; i < str.Length; i++)
      {
        if (pred(str[i]))
        {
          index = i;
          return true;
        }
      }

      return false;
    }

  }
}