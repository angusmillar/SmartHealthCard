using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.QRCode.Exceptions
{
  public class QRCodeChunkFormatException : FormatException
  {
    public QRCodeChunkFormatException(string message) : base(message)
    {
    }
  }
}
