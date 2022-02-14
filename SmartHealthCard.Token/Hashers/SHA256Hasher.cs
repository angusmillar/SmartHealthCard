using SmartHealthCard.Token.Encoders;

namespace SmartHealthCard.Token.Hashers
{
  public static class SHA256Hasher
  {
    public static byte[] GetSHA256Hash(this string input)
    {
      using var sha256Hash = System.Security.Cryptography.SHA256.Create();
      return sha256Hash.ComputeHash(Utf8EncodingSupport.GetBytes(input));
    }
  }
}
