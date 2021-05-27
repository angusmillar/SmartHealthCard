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
  public class SmartHealthCardDecoder
  {
    private readonly IJsonSerializer JsonSerializer;
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;
    private IJwksProvider? JwksProvider;
    private IJwsSignatureValidator? JwsSignatureValidator;
    private IJwsPayloadValidator? JwsPayloadValidator;
    private IJwsHeaderValidator? JwsHeaderValidator;

    public SmartHealthCardDecoder()
    {
      this.JsonSerializer = new JsonSerializer();      
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer(JsonSerializer);
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer(JsonSerializer);
    }

    public SmartHealthCardDecoder(IJwksProvider JwksProvider)
    {
      this.JwksProvider = JwksProvider;
      this.JsonSerializer = new JsonSerializer();
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer(JsonSerializer);
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer(JsonSerializer);
    }

    public SmartHealthCardDecoder(IJwsHeaderSerializer HeaderSerializer)
    {
      if (HeaderSerializer is null)
        throw new System.NullReferenceException(nameof(HeaderSerializer));

      this.HeaderSerializer = HeaderSerializer;

      this.JsonSerializer = new JsonSerializer();
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer(JsonSerializer);
    }

    public SmartHealthCardDecoder(IJwsPayloadSerializer PayloadSerializer)
    {
      if (PayloadSerializer is null)
        throw new System.NullReferenceException(nameof(PayloadSerializer));

      this.JsonSerializer = new JsonSerializer();
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer(JsonSerializer);
      this.PayloadSerializer = PayloadSerializer;
    }

    public SmartHealthCardDecoder(IJwsHeaderSerializer HeaderSerializer, IJwsPayloadSerializer PayloadSerializer)
    {
      if (HeaderSerializer is null)
        throw new System.NullReferenceException(nameof(HeaderSerializer));
      if (PayloadSerializer is null)
        throw new System.NullReferenceException(nameof(PayloadSerializer));

      this.JsonSerializer = new JsonSerializer();
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = PayloadSerializer;
    }

    /// <summary>
    /// Takes a Smart Health Card JWS token and decodes it to a SmartHealthCardModel object model 
    /// (This does not validate the token Siganture)
    /// </summary>
    /// <param name="Token"></param>
    /// <returns>SmartHealthCardModel</returns>
    public SmartHealthCardModel Decode(string Token)
    {
      JwsDecoder JwsDecoder = new JwsDecoder(this.HeaderSerializer, this.PayloadSerializer);
      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token);
      return SmartHealthCardModel;
    }

    /// <summary>
    /// Takes a Smart Health Card JWS token and decodes it to a SmartHealthCard Json string 
    /// (This does not validate the token Siganture)
    /// </summary>
    /// <param name="Token"></param>
    /// <returns></returns>
    public string DecodeToJson(string Token)
    {
      JwsDecoder JwsDecoder = new JwsDecoder(this.HeaderSerializer, this.PayloadSerializer);
      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token);
      JsonSerializer JsonSerializer = new JsonSerializer();
      return JsonSerializer.ToJson(SmartHealthCardModel, Minified: false);      
    }

    /// <summary>
    /// Takes a Smart Health Card JWS token, Verifies the token signature and contents before decoding it to a SmartHealthCardModel object model 
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="JsonWebKeySet"></param>
    /// <returns></returns>
    public SmartHealthCardModel VerifyAndDecode(string Token)
    {     
      InstantiationForVerifying();
      JwsDecoder JwsDecoder = new JwsDecoder(
        this.JsonSerializer, 
        this.HeaderSerializer, 
        this.PayloadSerializer, 
        this.JwksProvider, 
        this.JwsSignatureValidator, 
        this.JwsHeaderValidator, 
        this.JwsPayloadValidator);

      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: true);
      return SmartHealthCardModel;
    }
    /// <summary>
    /// Takes a Smart Health Card JWS token and a Certificate,  Verifies the token signature and contents before decoding it to a SmartHealthCard Json string
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="Certificate"></param>
    /// <returns></returns>
    public string VerifyAndDecodeToJson(string Token)
    {      
      InstantiationForVerifying();
      JwsDecoder JwsDecoder = new JwsDecoder(
        this.JsonSerializer, 
        this.HeaderSerializer, 
        this.PayloadSerializer, 
        this.JwksProvider, 
        this.JwsSignatureValidator, 
        this.JwsHeaderValidator, 
        this.JwsPayloadValidator);

      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: true);
      JsonSerializer JsonSerializer = new JsonSerializer();
      return JsonSerializer.ToJson(SmartHealthCardModel, Minified: false);
    }

    private void InstantiationForVerifying()
    {
      if (this.JwksProvider is null)
        this.JwksProvider = new JwksProvider(new JsonSerializer());

      this.JwsSignatureValidator = new JwsSignatureValidator();
      this.JwsPayloadValidator = new SmartHealthCardPayloadValidator();
      this.JwsHeaderValidator = new SmartHealthCardHeaderValidator();
    }

  }
}
