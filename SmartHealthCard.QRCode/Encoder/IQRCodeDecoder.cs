using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;

namespace SmartHealthCard.QRCode.Encoder
{
  public interface IQRCodeDecoder
  {
    IEnumerable<Chunk> GetQRCodeChunkList(IEnumerable<string> QRCodeRawDataList);
  }
}