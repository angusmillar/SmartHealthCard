using SmartHealthCard.Token.Support;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsEncoder
  {
    Task<Result<string>> EncodeAsync<HeaderType, PayloadType>(HeaderType Header, PayloadType Payload);
  }
}