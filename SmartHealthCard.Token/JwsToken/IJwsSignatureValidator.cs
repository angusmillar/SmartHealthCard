using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsSignatureValidator
  {
    Result<bool> Validate(IAlgorithm Algorithm, string Token);

  }
}