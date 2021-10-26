using System.Threading.Tasks;
using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsDecoder
  {
    Task<Result<HeaderType>> DecodeHeaderAsync<HeaderType>(string Token);
    Task<Result<PayloadType>> DecodePayloadAsync<HeaderType, PayloadType>(string Token, bool Verify = false);
  }
}