namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsEncoder
  {
    string Encode<HeaderType, PayloadType>(HeaderType Header, PayloadType Payload);
  }
}