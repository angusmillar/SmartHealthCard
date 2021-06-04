using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsSignatureValidator
  {
    Result Validate(IAlgorithm Algorithm, string Token);

  }
}