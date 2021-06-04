using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsPayloadValidator
  {
    Result Validate<T>(T Obj);
  }
}