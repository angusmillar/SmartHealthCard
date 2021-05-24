using SmartHealthCard.QRCode.Model;

namespace SmartHealthCard.QRCode.Chunker
{
  public interface ISmartHealthCardJwsChunker
  {
    Chunk[] Chunk(string JWSToken);
  }
}