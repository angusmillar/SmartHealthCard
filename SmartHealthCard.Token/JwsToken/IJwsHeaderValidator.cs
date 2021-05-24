namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsHeaderValidator
  {
    void Validate<T>(T Obj);

  }
}