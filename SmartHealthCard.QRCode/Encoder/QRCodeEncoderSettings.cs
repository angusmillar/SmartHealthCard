using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.QRCode.Encoder
{
  public class QRCodeEncoderSettings
  {
    public int Scale { get; set; } = 2;
    public int Border { get; set; } = 5;
    public Color Foreground { get; set; } = Color.Black;
    public Color Background { get; set; } = Color.White;

  }
}
