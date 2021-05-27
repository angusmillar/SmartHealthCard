using SmartHealthCard.Token.Algorithms;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsSignatureValidator
  {
    void Validate(IAlgorithm Algorithm, string Token);

  }
}