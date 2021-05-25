using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Serializers.Jws;

namespace SmartHealthCard.Token.JwsToken
{
  public class JwsEncoder
  {
    private readonly IAlgorithm Algorithm;
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;    
    public JwsEncoder(IJwsHeaderSerializer HeaderSerializer, IJwsPayloadSerializer PayloadSerializer, IAlgorithm Algorithm)
    {
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = PayloadSerializer;
      this.Algorithm = Algorithm;    
    }

    public string Encode<HeaderType, PayloadType>(HeaderType Header, PayloadType Payload)
    {     
      var HeaderSegment = Base64UrlEncoder.Encode(HeaderSerializer.Serialize(Header));
      var PayloadSegment = Base64UrlEncoder.Encode(PayloadSerializer.Serialize(Payload));

      var BytesToSign = Utf8EncodingSupport.GetBytes($"{HeaderSegment}.{PayloadSegment}");
      var Signature = Algorithm.Sign(BytesToSign);
      var SignatureSegment = Base64UrlEncoder.Encode(Signature);

      return $"{HeaderSegment}.{PayloadSegment}.{ SignatureSegment}";
    }
  }
}
