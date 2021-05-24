namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsSignatureValidator
  {
    void Validate(string Token);

  }
}