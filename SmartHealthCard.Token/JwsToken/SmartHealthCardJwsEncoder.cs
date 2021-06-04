using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Support;
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

    public async Task<Result<string>> EncodeAsync<HeaderType, PayloadType>(HeaderType Header, PayloadType Payload)
    {
      Result<byte[]> HeaderSerializeResult = await HeaderSerializer.SerializeAsync(Header);
      Result<byte[]> PayloadSerializeResult = await PayloadSerializer.SerializeAsync(Payload);
      var CombineResult = Result.Combine(HeaderSerializeResult, PayloadSerializeResult);
      if (CombineResult.Failure)
        return Result<string>.Fail(CombineResult.ErrorMessage);

      var HeaderSegment = Base64UrlEncoder.Encode(HeaderSerializeResult.Value);
      var PayloadSegment = Base64UrlEncoder.Encode(PayloadSerializeResult.Value);

      var BytesToSign = Utf8EncodingSupport.GetBytes($"{HeaderSegment}.{PayloadSegment}");
      Result<byte[]> SignatureResult = Algorithm.Sign(BytesToSign);
      if (SignatureResult.Failure)
        return Result<string>.Fail(SignatureResult.ErrorMessage);

      var SignatureSegment = Base64UrlEncoder.Encode(SignatureResult.Value);

      return Result<string>.Ok($"{HeaderSegment}.{PayloadSegment}.{ SignatureSegment}");
    }
  }
}
