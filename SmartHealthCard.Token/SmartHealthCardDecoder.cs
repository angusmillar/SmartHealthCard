using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.JwsToken;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Providers;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Serializers.Shc;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token
{
  /// <summary>
  /// A SMART Health Card decoder. 
  /// Take a SMART Health Card JWS token and decode's its payload to either a JSON string or an object model
  /// Optionaly verifiy the token's signing signature.
  /// For prodcution systems it is higly advised that you do verifiy the signature. 
  /// </summary>
  public class SmartHealthCardDecoder
  {
    private readonly IJsonSerializer JsonSerializer;
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;

    private IJwksProvider? JwksProvider;
    private IJwsSignatureValidator? JwsSignatureValidator;
    private IJwsHeaderValidator? JwsHeaderValidator;
    private IJwsPayloadValidator? JwsPayloadValidator;

    /// <summary>
    /// Default Consttuctor
    /// </summary>
    public SmartHealthCardDecoder()
    {
      this.JsonSerializer = new JsonSerializer();
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer(JsonSerializer);
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer(JsonSerializer);
    }

    /// <summary>
    /// Provide any implementation of the IJwksProvider interface to overide the default implementation
    /// This allows you to inject a JWKS file to be used when validating the JWS signature instead of the 
    /// default implementation that will attempt to source the JWKS from the token's Issuer URL (iss) + /.well-known/jwks.json 
    /// </summary>
    /// <param name="JwksProvider">Provides the provider for sourcing the JWKS file for token signature verifying</param>
    public SmartHealthCardDecoder(IJwksProvider? JwksProvider)
      :this()
    {
      this.JwksProvider = JwksProvider;
    }

    /// <summary>
    /// Provide any implementation of the follwowing interfaces to overide their default implementation
    /// </summary>
    /// <param name="JsonSerializer">Provides an implementation of a basic JSON serialization</param>
    /// <param name="HeaderSerializer">Provides an implementation that performs the serialization of the data that is packed into the JWS Header</param>
    /// <param name="PayloadSerializer">Provides an implementation that performs the the data that is packed into the JWS Payload</param>
    public SmartHealthCardDecoder(IJsonSerializer? JsonSerializer, IJwsHeaderSerializer? HeaderSerializer, IJwsPayloadSerializer? PayloadSerializer)
    {
      this.JsonSerializer = JsonSerializer ?? new JsonSerializer();
      this.HeaderSerializer = HeaderSerializer ?? new SmartHealthCardJwsHeaderSerializer(this.JsonSerializer);
      this.PayloadSerializer = PayloadSerializer ?? new SmartHealthCardJwsPayloadSerializer(this.JsonSerializer);
    }

    /// <summary>
    /// Provide any implementation of the follwowing interfaces to overide their default implementation 
    /// </summary>
    /// <param name="JsonSerializer">Provides an implementation of a basic JSON serialization</param>
    /// <param name="HeaderSerializer">Provides an implementation that performs the serialization of the data that is packed into the JWS Header</param>
    /// <param name="PayloadSerializer">Provides an implementation that performs the the data that is packed into the JWS Payload</param>
    /// <param name="JwksProvider">Provides an implementation that sources the JWKS file for token signature verifying</param>
    /// <param name="JwsSignatureValidator">Provides an implementation of the JWS signature verifying</param>
    /// <param name="JwsHeaderValidator">Provides an implementation that perfomes the serialization of the data that is packed into the JWS Payload</param>
    /// <param name="JwsPayloadValidator">Provides an implementation that perfomes the serialization of the data that is packed into the JWS Payload</param>
    public SmartHealthCardDecoder(IJsonSerializer? JsonSerializer, IJwsHeaderSerializer? HeaderSerializer, IJwsPayloadSerializer? PayloadSerializer,
      IJwksProvider? JwksProvider, IJwsSignatureValidator? JwsSignatureValidator, IJwsHeaderValidator? JwsHeaderValidator, IJwsPayloadValidator? JwsPayloadValidator)
    {
      this.JsonSerializer = JsonSerializer ?? new JsonSerializer();
      this.HeaderSerializer = HeaderSerializer ?? new SmartHealthCardJwsHeaderSerializer(this.JsonSerializer);
      this.PayloadSerializer = PayloadSerializer ?? new SmartHealthCardJwsPayloadSerializer(this.JsonSerializer);

      this.JwksProvider = JwksProvider;
      this.JwsSignatureValidator = JwsSignatureValidator;
      this.JwsHeaderValidator = JwsHeaderValidator;
      this.JwsPayloadValidator = JwsPayloadValidator;
    }

    /// <summary>
    /// Decode a SMART Health Card JWS Token to its JSON form of the SMART Health Card verifiable credentials
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="Verify"></param>
    /// <returns></returns>
    public string DecodeToJson(string Token, bool Verify = true)
    {
      SmartHealthCardModel SmartHealthCardModel = Decode(Token, Verify);
      return JsonSerializer.ToJson(SmartHealthCardModel, Minified: false);
    }

    /// <summary>
    /// Decode a SMART Health Card JWS Token to a object model form of the SMART Health Card verifiable credentials
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="Verify"></param>
    /// <returns></returns>
    public SmartHealthCardModel Decode(string Token, bool Verify = true)
    {
      if (Verify)
      {
        IJwsDecoder JwsDecoder = new SmartHealthCardJwsDecoder(
          this.JsonSerializer,
          this.HeaderSerializer,
          this.PayloadSerializer,
          this.JwksProvider,
          this.JwsSignatureValidator,
          this.JwsHeaderValidator,
          this.JwsPayloadValidator);

        return JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: Verify);
      }
      else
      {
        IJwsDecoder JwsDecoder = new SmartHealthCardJwsDecoder(
          this.HeaderSerializer,
          this.PayloadSerializer);

        return JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: Verify);
      }
    }
  }
}
