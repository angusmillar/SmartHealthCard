using System.Text;

namespace SmartHealthCard.QRCode.Encoder
{
  public class NumericalModeEncoder : INumericalModeEncoder
  {
    public string Encode(string JWSToken)
    {
      StringBuilder StringBuilder = new StringBuilder();
      foreach (char Char in JWSToken.ToCharArray())
      {
        int Integer = Char - 45;
        StringBuilder.Append(Integer.ToString("D2"));
      }
      return StringBuilder.ToString();
    }
  }
}
