using System.IO;
using System.IO.Compression;
using System.Text;

namespace SmartHealthCard.Token.Compression
{
  public static class DeflateCompression
  {
    public static byte[] Compress(string input)
    {
      using (MemoryStream MemoryStream = new MemoryStream())
      {
        using (DeflateStream DeflateStream = new DeflateStream(MemoryStream, CompressionMode.Compress))
        {
          using (StreamWriter StreamWriter = new StreamWriter(DeflateStream, Encoding.UTF8))
          {
            StreamWriter.Write(input);
          }
        }
        return MemoryStream.ToArray();
      }
    }

    public static string Uncompress(byte[] input)
    {
      using (MemoryStream MemoryStream = new MemoryStream(input))
      {
        using (DeflateStream DeflateStream = new DeflateStream(MemoryStream, CompressionMode.Decompress))
        {
          using (StreamReader StreamReader = new StreamReader(DeflateStream, Encoding.UTF8))
          {
            return StreamReader.ReadToEnd();
          }
        }
      }
    }
  }
}
