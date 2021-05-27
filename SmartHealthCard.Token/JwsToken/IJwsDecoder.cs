namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsDecoder
  {
    HeaderType DecodeHeader<HeaderType>(string Token);
    PayloadType DecodePayload<HeaderType, PayloadType>(string Token, bool Verity = false);
  }
}