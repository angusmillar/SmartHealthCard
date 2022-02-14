using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartHealthCard.QRCode.Model;

namespace SmartHealthCard.QRCode.Encoder
{
  public class NumericalModeDecoder : INumericalModeDecoder
  {
    public string Decode(IEnumerable<Chunk> ChunkList)
    {
      StringBuilder StringBuilder = new();
      foreach (Chunk Chunk in ChunkList)
      {
        string Numeric = Chunk.NumericSegment;
        foreach (string Number in Spliter(Numeric, 2))
        {
          if (int.TryParse(Number, out int IntNumber))
          {
            StringBuilder.Append(Convert.ToChar(IntNumber + 45));
          }
        }
      }
      return StringBuilder.ToString();
    }

    private static IEnumerable<string> Spliter(string str, int chunkSize)
    {
      return Enumerable.Range(0, str.Length / chunkSize)
          .Select(i => str.Substring(i * chunkSize, chunkSize));
    }
  }
}
