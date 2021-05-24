namespace SmartHealthCard.Token.Algorithms
{
  public interface IAlgorithm
  {
    string Name { get; }
    string KeyTypeName { get; }
    string CurveName { get; }
    string GetKid();
    byte[] Sign(byte[] bytesToSign);
    bool Verify(byte[] bytesToSign, byte[] signature);
  }
}