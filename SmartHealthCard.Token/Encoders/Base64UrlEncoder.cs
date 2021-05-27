using System;

namespace SmartHealthCard.Token.Encoders
{
  public static class Base64UrlEncoder 
  {   
    public static string Encode(byte[] input)
    {
      if (input is null)
        throw new ArgumentNullException(nameof(input));
      if (input.Length == 0)
        throw new ArgumentOutOfRangeException(nameof(input));

      var output = Convert.ToBase64String(input);
      output = output.FirstSegment('='); // Remove any trailing '='
      output = output.Replace('+', '-'); // 62nd char 
      output = output.Replace('/', '_'); // 63rd char 
      return output;
    }
    
    public static byte[] Decode(string input)
    {
      if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException(nameof(input));

      //URL decoding process
      var output = input;
      output = output.Replace('-', '+'); // 62nd char 
      output = output.Replace('_', '/'); // 63rd char 
      switch (output.Length % 4) // Pad the appropriate number of '='s
      {
        case 0:
          break; // None
        case 2:
          output += "==";
          break; // Two 
        case 3:
          output += "=";
          break; // One 
        default:
          throw new FormatException("Illegal base64url string as its characters can not be cleanly divided by 4.");
      }
      //Base 64 decoding process
      var converted = Convert.FromBase64String(output);
      return converted;
    }

    private static string FirstSegment(this string Input, char Separator)
    {
      var IndexOfSeparator = Input.IndexOf(Separator);
      return IndexOfSeparator != -1 ? Input.Substring(0, IndexOfSeparator) : Input;
    }
  }
}
