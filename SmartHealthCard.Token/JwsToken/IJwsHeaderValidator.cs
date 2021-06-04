using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsHeaderValidator
  {
    Result Validate<T>(T Obj);

  }
}