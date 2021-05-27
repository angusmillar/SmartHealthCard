using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Compression
{
  public static class DeflateCompression
  {
    public static async Task<byte[]> CompressAsync(string input)
    {
      using (MemoryStream MemoryStream = new MemoryStream())
      {
        using (DeflateStream DeflateStream = new DeflateStream(MemoryStream, CompressionMode.Compress))
        {
          using (StreamWriter StreamWriter = new StreamWriter(DeflateStream, Encoding.UTF8))
          {
            await StreamWriter.WriteAsync(input);
          }
        }
        return MemoryStream.ToArray();
      }
    }

    public static async Task<string> UncompressAsync(byte[] input)
    {
      using (MemoryStream MemoryStream = new MemoryStream(input))
      {
        using (DeflateStream DeflateStream = new DeflateStream(MemoryStream, CompressionMode.Decompress))
        {
          using (StreamReader StreamReader = new StreamReader(DeflateStream, Encoding.UTF8))
          {
            return await StreamReader.ReadToEndAsync();
          }
        }
      }
    }
  }
}
