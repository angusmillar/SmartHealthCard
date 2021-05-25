namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsPayloadValidator
  {
    void Validate<T>(T Obj);
  }
}