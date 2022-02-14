using Net.Codecrete.QrCodeGenerator;
using SmartHealthCard.QRCode.Exceptions;
using SmartHealthCard.QRCode.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.QRCode.Encoder
{
  public class QRCodeDecoder : IQRCodeDecoder
  {
    public IEnumerable<Chunk> GetQRCodeChunkList(IEnumerable<string> QRCodeRawDataList)
    {
      //shc:/2/3/56762909524320603460292437404460<snipped for brevity>
      //shc:/56762909524320603460292437404460<snipped for brevity>
      List<Chunk> ChunkList = new();
      foreach (string QRCodeRawData in QRCodeRawDataList)
      {
        string[] Split = QRCodeRawData.Split('/');
        Chunk? Chunk;
        if (Split.Length == 2)
        {
          Chunk = new Chunk($"{Split[0]}/", Split[1]);
        }
        else if (Split.Length == 4)
        {
          Chunk = new Chunk($"{Split[0]}/{Split[1]}/{Split[2]}/", Split[3]);
        }
        else
        {
          throw new QRCodeChunkFormatException($"The raw QR Code data was incorrectly formated, found {Split.Length} chunks where only 2 or 4 are allowed.");
        }
        ChunkList.Add(Chunk);
      }
      return ChunkList;
    }
  }
}
