using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.QRCode.Model
{
  public class Chunk
  {
    public Chunk(string TextSegment, string NumericSegment)
    {
      this.ByteSegment = TextSegment;
      this.NumericSegment = NumericSegment;
    }

    public string ByteSegment { get; set; }
    public string NumericSegment { get; set; }    
  }
}
