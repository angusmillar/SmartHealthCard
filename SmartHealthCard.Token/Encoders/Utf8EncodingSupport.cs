using System.Text;

namespace SmartHealthCard.Token.Encoders
{
  public static class Utf8EncodingSupport
  {
    private static readonly UTF8Encoding UTF8Encoding = new(encoderShouldEmitUTF8Identifier: false);

    public static byte[] GetBytes(string input) => UTF8Encoding.GetBytes(input);

    public static string GetString(byte[] bytes) => UTF8Encoding.GetString(bytes);

    public static byte[] GetBytes(string input1, byte separator, string input2)
    {
      byte[] output = new byte[UTF8Encoding.GetByteCount(input1) + UTF8Encoding.GetByteCount(input2) + 1];
      int bytesWritten = UTF8Encoding.GetBytes(input1, 0, input1.Length, output, 0);
      output[bytesWritten++] = separator;
      UTF8Encoding.GetBytes(input2, 0, input2.Length, output, bytesWritten);
      return output;
    }

  }
}
