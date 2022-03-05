using Net.Codecrete.QrCodeGenerator;
using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Text;

namespace SmartHealthCard.QRCode.Encoder
{
  public class QRCodeEncoder : IQRCodeEncoder
  {
    [SupportedOSPlatform("Windows")]
    public List<Bitmap> GetQRCodeList(IEnumerable<Chunk> ChunkList, QRCodeEncoderSettings QRCodeEncoderSettings)
    {
      List<Bitmap> BitmapList = new();
      foreach (Chunk Chunk in ChunkList)
      {
        List<QrSegment> SegmentList = new()
        {
          QrSegment.MakeBytes(Encoding.ASCII.GetBytes(Chunk.ByteSegment)),
          QrSegment.MakeNumeric(Chunk.NumericSegment)
        };
        
        QrCode QrCode = QrCode.EncodeSegments(SegmentList, QrCode.Ecc.Low, 22, 22);

        BitmapList.Add(QrCode.ToBitmap(
          QRCodeEncoderSettings.Scale, 
          QRCodeEncoderSettings.Border, 
          QRCodeEncoderSettings.Foreground, 
          QRCodeEncoderSettings.Background));
      }
      return BitmapList;
    }

    public List<string> GetQRCodeRawDataList(IEnumerable<Chunk> ChunkList)
    {
      List<string> QRCodeData = new();
      foreach (Chunk Chunk in ChunkList)
      {
        string Data = $"{Chunk.ByteSegment}{Chunk.NumericSegment}";
        QRCodeData.Add(Data);        
      }
      return QRCodeData;
    }
  }
}
