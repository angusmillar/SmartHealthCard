using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;

namespace SmartHealthCard.QRCode.Encoder
{
  public interface INumericalModeDecoder
  {
    string Decode(IEnumerable<Chunk> ChunkList);
  }
}