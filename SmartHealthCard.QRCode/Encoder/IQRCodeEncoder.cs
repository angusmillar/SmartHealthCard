using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;
using System.Drawing;

namespace SmartHealthCard.QRCode.Encoder
{
  public interface IQRCodeEncoder
  {
    List<Bitmap> GetQRCodeList(IEnumerable<Chunk> ChunkList, QRCodeEncoderSettings QRCodeEncoderSettings);
    List<string> GetQRCodeRawDataList(IEnumerable<Chunk> ChunkList);
  }
}