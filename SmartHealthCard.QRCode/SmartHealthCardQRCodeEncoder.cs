using Net.Codecrete.QrCodeGenerator;
using SmartHealthCard.QRCode.Chunker;
using SmartHealthCard.QRCode.Encoder;
using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SmartHealthCard.QRCode
{
  public class SmartHealthCardQRCodeEncoder
  {
    private readonly INumericalModeEncoder NumericalModeEncoder;
    private readonly ISmartHealthCardJwsChunker SmartHealthCardJwsChunker;
    private readonly IQRCodeEncoder QRCodeEncoder;
    private readonly QRCodeEncoderSettings? QRCodeEncoderSettings;

    public SmartHealthCardQRCodeEncoder()
    {     
      this.NumericalModeEncoder = new NumericalModeEncoder();
      this.SmartHealthCardJwsChunker = new SmartHealthCardJwsChunker(this.NumericalModeEncoder);
      this.QRCodeEncoder = new QRCodeEncoder();
    }

    //All Non-nullable field have been instantiated with in this constructor, warring disabled. 
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SmartHealthCardQRCodeEncoder(QRCodeEncoderSettings? QRCodeEncoderSettings = null,
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
                                        INumericalModeEncoder? NumericalModeEncoder = null,
                                        ISmartHealthCardJwsChunker? SmartHealthCardJwsChunker = null,
                                        IQRCodeEncoder? QRCodeEncoder = null)
    {
      this.QRCodeEncoderSettings = QRCodeEncoderSettings;

      if (NumericalModeEncoder is null)
        this.NumericalModeEncoder = new NumericalModeEncoder();

      if (SmartHealthCardJwsChunker is null)
      {
        if (this.NumericalModeEncoder is object)
          this.SmartHealthCardJwsChunker = new SmartHealthCardJwsChunker(this.NumericalModeEncoder);
      }

      if (QRCodeEncoder is null)
        this.QRCodeEncoder = new QRCodeEncoder();
    }

    public List<Bitmap> GetQRCodeList(string SmartHealthCardJWSToken, QRCodeEncoderSettings? QRCodeEncoderSettings = null)
    {
      if (QRCodeEncoderSettings is null)
        QRCodeEncoderSettings = new QRCodeEncoderSettings();

      Chunk[] ChunkArray = SmartHealthCardJwsChunker.Chunk(SmartHealthCardJWSToken);
      return QRCodeEncoder.GetQRCodeList(ChunkArray, QRCodeEncoderSettings);
    }

    public List<string> GetQRCodeRawDataList(string SmartHealthCardJWSToken)
    {
      Chunk[] ChunkArray = SmartHealthCardJwsChunker.Chunk(SmartHealthCardJWSToken);
      return QRCodeEncoder.GetQRCodeRawDataList(ChunkArray);
    }
  }
}
