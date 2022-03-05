using SkiaSharp;
using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;

namespace SmartHealthCard.QRCode.Encoder
{
  public interface IQRCodeEncoder
  {
    List<SKBitmap> GetQRCodeList(IEnumerable<Chunk> ChunkList, QRCodeEncoderSettings QRCodeEncoderSettings);
    List<string> GetQRCodeRawDataList(IEnumerable<Chunk> ChunkList);
  }
}