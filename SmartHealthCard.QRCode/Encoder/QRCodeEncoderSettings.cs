using SkiaSharp;

namespace SmartHealthCard.QRCode.Encoder
{
  /// <summary>
  /// The available setting for generating QR Code images 
  /// </summary>
  public class QRCodeEncoderSettings
  {
    /// <summary>
    /// The Scale is the number of pixels used for each single point within the QR Code image
    /// the default is set to 2
    /// </summary>
    public int Scale { get; set; } = 2;
    /// <summary>
    /// The Border is the number of pixels thick the board around the QR Code will be
    /// The default is set to 5
    /// </summary>
    public int Border { get; set; } = 5;
    /// <summary>
    /// The QR Code Foreground is the color
    /// The default is Black
    /// </summary>
    public SKColor Foreground { get; set; } = SKColor.FromHsv(0, 0, 0); //Black
    /// <summary>
    /// The QR Code Background is the color
    /// The default is White
    /// </summary>
    public SKColor Background { get; set; } = SKColor.FromHsv(0, 0, 100); //White

  }
}
