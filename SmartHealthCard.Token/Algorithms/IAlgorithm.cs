using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Token.Algorithms
{
  public interface IAlgorithm
  {
    string Name { get; }
    string KeyTypeName { get; }
    string CurveName { get; }
    Result<string> GetKid();   
    Result<byte[]> Sign(byte[] bytesToSign);
    Result<bool> Verify(byte[] bytesToSign, byte[] signature);
  }
}