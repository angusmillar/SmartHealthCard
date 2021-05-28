using SmartHealthCard.QRCode.Encoder;
using SmartHealthCard.QRCode.Model;
using System.Collections.Generic;

namespace SmartHealthCard.QRCode
{
  /// <summary>
  /// A decoder to decode from the raw QR Code data to a SMART Health Card JWS token
  /// Sorry but this does not accept a QR image directly, it requires a scanner to 
  /// have gotten the raw QR Code data from the QR Code image
  /// </summary>
  public class SmartHealthCardQRCodeDecoder
  {
    private readonly INumericalModeDecoder NumericalModeDecoder;    
    private readonly IQRCodeDecoder QRCodeDecoder;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SmartHealthCardQRCodeDecoder()      
    {
      this.NumericalModeDecoder = new NumericalModeDecoder();
      this.QRCodeDecoder = new QRCodeDecoder();
    }

    /// <summary>
    /// Provide any implementation of the following interfaces to override their default implementation 
    /// </summary>
    /// <param name="NumericalModeDecoder">Provides an implementation of the Numerical decoder used for SMART Health Card QR Codes, see: https://smarthealth.cards/#encoding-chunks-as-qr-codes </param>
    /// <param name="QRCodeDecoder">Provides an implementation of the chuck decoder used for SMART Health Card QR Codes, see: https://smarthealth.cards/#encoding-chunks-as-qr-codes</param>
    public SmartHealthCardQRCodeDecoder(     
      INumericalModeDecoder? NumericalModeDecoder = null,
      IQRCodeDecoder? QRCodeDecoder = null)
    {      
      this.NumericalModeDecoder = NumericalModeDecoder ?? new NumericalModeDecoder();      
      this.QRCodeDecoder = QRCodeDecoder ?? new QRCodeDecoder();
    }

    /// <summary>
    /// Provided SMART Health Card QR Code raw data and it will return a SMART Health Card JWS Token   
    /// </summary>
    /// <param name="QRCodeRawDataList"></param>
    /// <returns></returns>
    public string GetToken(List<string> QRCodeRawDataList)
    {      
      IEnumerable<Chunk> ChunkList = QRCodeDecoder.GetQRCodeChunkList(QRCodeRawDataList);
      return NumericalModeDecoder.Decode(ChunkList);      
    }
  }
}
