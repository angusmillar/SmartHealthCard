using SmartHealthCard.QRCode.Encoder;
using SmartHealthCard.QRCode.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHealthCard.QRCode.Chunker
{
  public class SmartHealthCardJwsChunker : ISmartHealthCardJwsChunker
  {
    private readonly INumericalModeEncoder NumericalModeEncoder;
    private const string Prefix = "shc:";

    public SmartHealthCardJwsChunker(INumericalModeEncoder NumericalModeEncoder)
    {
      this.NumericalModeEncoder = NumericalModeEncoder;
    }

    public Chunk[] Chunk(string JWSToken)
    {

      int MaxQRCodeLength = 1195;
      List<Chunk> ChunkList = new();

      if (JWSToken.Length <= MaxQRCodeLength)
      {
        //If the total is under 1195 there is no need to chunk it
        ChunkList.Add(new Chunk($"{Prefix}/", this.NumericalModeEncoder.Encode(JWSToken)));
        return ChunkList.ToArray();
      }
      else
      {
        // here we keep dividing the total until we get an even distribution where all chunks numeric portion are under 1195 
        IEnumerable<int>? ChunkSizes = Array.Empty<int>();
        bool FindingFoundChuckSize = true;
        int Divider = 2;
        while (FindingFoundChuckSize)
        {
          ChunkSizes = DistributeInteger(JWSToken.Length, Divider);
          if (ChunkSizes.Any(x => x > 1191))
          {
            Divider++;
          }
          else
          {
            FindingFoundChuckSize = false;
          }
        }
        //Now we have the length for each chunk so we build the text for each QR Code
        int[] ChunkSizeArray = ChunkSizes.ToArray();
        int StartPosition = 0;
        for (int i = 0; i < ChunkSizeArray.Length; i++)
        {
          int CurrentChunkLength = ChunkSizeArray[i];
          string ThisCunk = JWSToken.Substring(StartPosition, ChunkSizeArray[i]);
          string NumericChunkData = NumericalModeEncoder.Encode(JWSToken.Substring(StartPosition, ChunkSizeArray[i]));

          //Syntax: shc:/[This Chunk index]/[Total Chunks]/[This chunk's data]
          //shc:/2/3/56762909524320603460292437404460 ..etc
          ChunkList.Add(new Chunk($"{Prefix}/{i+1}/{ChunkSizeArray.Length}/", NumericChunkData));
          StartPosition += ChunkSizeArray[i];
        }
        return ChunkList.ToArray();
      }
    }

    private static IEnumerable<int> DistributeInteger(int total, int divider)
    {
      if (divider == 0)
      {
        yield return 0;
      }
      else
      {
        int rest = total % divider;
        double result = total / (double)divider;

        for (int i = 0; i < divider; i++)
        {
          if (rest-- > 0)
            yield return (int)Math.Ceiling(result);
          else
            yield return (int)Math.Floor(result);
        }
      }
    }


  }
}
