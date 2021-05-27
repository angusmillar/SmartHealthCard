using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Serializers.Jws;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.JwsToken
{
  public class SmartHealthCardJwsEncoder : IJwsEncoder
  {
    private readonly IAlgorithm Algorithm;
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;
    public SmartHealthCardJwsEncoder(IJwsHeaderSerializer HeaderSerializer, IJwsPayloadSerializer PayloadSerializer, IAlgorithm Algorithm)
    {
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = PayloadSerializer;
      this.Algorithm = Algorithm;
    }

    public async Task<string> EncodeAsync<HeaderType, PayloadType>(HeaderType Header, PayloadType Payload)
    {
      var HeaderSegment = Base64UrlEncoder.Encode(await HeaderSerializer .SerializeAsync(Header));
      var PayloadSegment = Base64UrlEncoder.Encode(await PayloadSerializer.SerializeAsync(Payload));

      var BytesToSign = Utf8EncodingSupport.GetBytes($"{HeaderSegment}.{PayloadSegment}");
      var Signature = Algorithm.Sign(BytesToSign);
      var SignatureSegment = Base64UrlEncoder.Encode(Signature);

      return $"{HeaderSegment}.{PayloadSegment}.{ SignatureSegment}";
    }
  }
}
