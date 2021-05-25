using Net.Codecrete.QrCodeGenerator;
using SmartHealthCard.QRCode.Chunker;
using SmartHealthCard.QRCode.Encoder;
using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SmartHealthCard.QRCode
{
  public class SmartHealthCardQRCodeFactory
  {
    private readonly INumericalModeEncoder NumericalModeEncoder;
    private readonly ISmartHealthCardJwsChunker SmartHealthCardJwsChunker;
    public SmartHealthCardQRCodeFactory()
    {
      this.NumericalModeEncoder = new NumericalModeEncoder();
      this.SmartHealthCardJwsChunker = new SmartHealthCardJwsChunker(this.NumericalModeEncoder);
    }

    public SmartHealthCardQRCodeFactory(INumericalModeEncoder NumericalModeEncoder)
    {
      this.NumericalModeEncoder = NumericalModeEncoder;
      this.SmartHealthCardJwsChunker = new SmartHealthCardJwsChunker(this.NumericalModeEncoder);
    }

    public SmartHealthCardQRCodeFactory(ISmartHealthCardJwsChunker SmartHealthCardJwsChunker)
    {
      this.NumericalModeEncoder = new NumericalModeEncoder();
      this.SmartHealthCardJwsChunker = SmartHealthCardJwsChunker;
    }

    public SmartHealthCardQRCodeFactory(INumericalModeEncoder NumericalModeEncoder, ISmartHealthCardJwsChunker SmartHealthCardJwsChunker)      
    {
      this.NumericalModeEncoder = NumericalModeEncoder;
      this.SmartHealthCardJwsChunker = SmartHealthCardJwsChunker;
    }

    public Bitmap[] CreateQRCode(string SmartHealthCardJWSToken)
    {
      List<Bitmap> BitmapList = new List<Bitmap>();      
      Chunk[] ChunkArray = this.SmartHealthCardJwsChunker.Chunk(SmartHealthCardJWSToken);
      foreach (Chunk Chunk in ChunkArray)
      {
        List<QrSegment> SegmentList = new List<QrSegment>()
        {
          QrSegment.MakeBytes(Encoding.ASCII.GetBytes(Chunk.ByteSegment)),
          QrSegment.MakeNumeric(Chunk.NumericSegment)
        };
        QrCode QrCode = QrCode.EncodeSegments(SegmentList, QrCode.Ecc.Low, 22, 22);
        BitmapList.Add(QrCode.ToBitmap(2, 5, Color.Black, Color.White));
      }
      return BitmapList.ToArray();
    }

    public string[] CreateQRCodeRawData(string SmartHealthCardJWSToken)
    {
      List<string> QRCodeData = new List<string>();
      Chunk[] ChunkArray = this.SmartHealthCardJwsChunker.Chunk(SmartHealthCardJWSToken);
      foreach (Chunk Chunk in ChunkArray)
      {
        string Data = $"{Chunk.ByteSegment}{Chunk.NumericSegment}";       
        QRCodeData.Add(Data);
      }
      return QRCodeData.ToArray();
    }
  }
}
