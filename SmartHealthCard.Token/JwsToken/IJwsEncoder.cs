using System.Threading.Tasks;

namespace SmartHealthCard.Token.JwsToken
{
  public interface IJwsEncoder
  {
    Task<string> EncodeAsync<HeaderType, PayloadType>(HeaderType Header, PayloadType Payload);
  }
}