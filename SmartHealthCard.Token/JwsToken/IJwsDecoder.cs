using System.Threading.Tasks;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsDecoder
  {
    Task<HeaderType> DecodeHeaderAsync<HeaderType>(string Token);
    Task<PayloadType> DecodePayloadAsync<HeaderType, PayloadType>(string Token, bool Verity = false);
  }
}