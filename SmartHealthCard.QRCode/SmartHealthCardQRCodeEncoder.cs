using Net.Codecrete.QrCodeGenerator;
using SkiaSharp;
using SmartHealthCard.QRCode.Chunker;
using SmartHealthCard.QRCode.Encoder;
using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Text;

namespace SmartHealthCard.QRCode
{
  /// <summary>
  /// An encoder to generate SMART Health Card QR Code Images or there raw payloads
  /// </summary>
  public class SmartHealthCardQRCodeEncoder
  {
    private readonly INumericalModeEncoder NumericalModeEncoder;
    private readonly ISmartHealthCardJwsChunker SmartHealthCardJwsChunker;
    private readonly IQRCodeEncoder QRCodeEncoder;
    private readonly QRCodeEncoderSettings QRCodeEncoderSettings;


    /// <summary>
    /// Default Constructor
    /// </summary>
    public SmartHealthCardQRCodeEncoder()
      : this(new QRCodeEncoderSettings())
    {     
    }

    /// <summary>
    /// Optionally provide setting for the encoder which control things like the QR Code size and color
    /// </summary>
    /// <param name="QRCodeEncoderSettings"></param>
    public SmartHealthCardQRCodeEncoder(QRCodeEncoderSettings? QRCodeEncoderSettings = null)
    {
      this.QRCodeEncoderSettings = QRCodeEncoderSettings ?? new QRCodeEncoderSettings();
      this.NumericalModeEncoder = new NumericalModeEncoder();
      this.SmartHealthCardJwsChunker = new SmartHealthCardJwsChunker(this.NumericalModeEncoder);
      this.QRCodeEncoder = new QRCodeEncoder();
    }

    /// <summary>
    /// Provide any implementation of the following interfaces to override their default implementation 
    /// </summary>
    /// <param name="QRCodeEncoderSettings">Optionally provide setting for the encoder which control things like the QR Code size and color</param>
    /// <param name="NumericalModeEncoder">Provides an implementation of the Numerical encoding used for SMART Health Card QR Codes, see https://smarthealth.cards/#encoding-chunks-as-qr-codes </param>
    /// <param name="SmartHealthCardJwsChunker">Provides an implementation of the Chunks encoding used for SMART Health Card QR Codes, see https://smarthealth.cards/#encoding-chunks-as-qr-codes </param>
    /// <param name="QRCodeEncoder">>Provides an implementation of the QR Code image generation, the default uses: https://github.com/manuelbl/QrCodeGenerator  </param>
    public SmartHealthCardQRCodeEncoder(
      QRCodeEncoderSettings? QRCodeEncoderSettings = null,
      INumericalModeEncoder? NumericalModeEncoder = null,
      ISmartHealthCardJwsChunker? SmartHealthCardJwsChunker = null,
      IQRCodeEncoder? QRCodeEncoder = null)
    {
      this.QRCodeEncoderSettings = QRCodeEncoderSettings ?? new QRCodeEncoderSettings();
      this.NumericalModeEncoder = NumericalModeEncoder ?? new NumericalModeEncoder();
      this.SmartHealthCardJwsChunker = SmartHealthCardJwsChunker ?? new SmartHealthCardJwsChunker(this.NumericalModeEncoder);
      this.QRCodeEncoder = QRCodeEncoder ?? new QRCodeEncoder();
    }

    /// <summary>
    /// Provided a SMART Health Card JWS Token it will return a list of Bitmaps that represent the entire SMART Health Card in QR Codes 
    /// Note: SMART Health Card JWS Token with large payload may be broken up into many QR Codes where verifiers can scan each in any order
    /// to reconstruct the SMART Health Card JWS Token
    /// </summary>
    /// <param name="SmartHealthCardJWSToken"></param>
    /// <returns></returns>    
    public List<SKBitmap> GetQRCodeList(string SmartHealthCardJWSToken)
    {     
      Chunk[] ChunkArray = SmartHealthCardJwsChunker.Chunk(SmartHealthCardJWSToken);
      return QRCodeEncoder.GetQRCodeList(ChunkArray, this.QRCodeEncoderSettings);
    }

    /// <summary>
    /// Provided a SMART Health Card JWS Token it will return a string list of the raw data that can be encoded into a QR Codes 
    /// Note: SMART Health Card JWS Token with large payload may be broken up into many QR Codes where verifiers can scan each in any order
    /// to reconstruct the SMART Health Card JWS Token
    /// </summary>
    /// <param name="SmartHealthCardJWSToken"></param>
    /// <returns></returns>
    public List<string> GetQRCodeRawDataList(string SmartHealthCardJWSToken)
    {
      Chunk[] ChunkArray = SmartHealthCardJwsChunker.Chunk(SmartHealthCardJWSToken);
      return QRCodeEncoder.GetQRCodeRawDataList(ChunkArray);
    }
    
  }
}
