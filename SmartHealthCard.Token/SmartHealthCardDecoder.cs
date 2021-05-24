using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.JwsToken;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Serializers.Shc;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token
{
  public class SmartHealthCardDecoder
  {
    private IAlgorithm? Algorithm;
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;
    private IJwsSignatureValidator? JwsSignatureValidator;
    private IJwsPayloadValidator? JwsPayloadValidator;
    private IJwsHeaderValidator? JwsHeaderValidator;

    public SmartHealthCardDecoder()
    {
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer();
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer();
    }

    public SmartHealthCardDecoder(IJwsHeaderSerializer HeaderSerializer)
    {
      if (HeaderSerializer is null)
        throw new System.NullReferenceException(nameof(HeaderSerializer));

      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer();
    }

    public SmartHealthCardDecoder(IJwsPayloadSerializer PayloadSerializer)
    {
      if (PayloadSerializer is null)
        throw new System.NullReferenceException(nameof(PayloadSerializer));

      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer();
      this.PayloadSerializer = PayloadSerializer;
    }

    public SmartHealthCardDecoder(IJwsHeaderSerializer HeaderSerializer, IJwsPayloadSerializer PayloadSerializer)
    {
      if (HeaderSerializer is null)
        throw new System.NullReferenceException(nameof(HeaderSerializer));
      if (PayloadSerializer is null)
        throw new System.NullReferenceException(nameof(PayloadSerializer));

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
      JwsDecoder JwsDecoder = new JwsDecoder(this.HeaderSerializer, this.PayloadSerializer, this.JwsSignatureValidator, this.JwsHeaderValidator, this.JwsPayloadValidator);
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
      return SmartHealthCardModelJsonSerializer.ToJson(SmartHealthCardModel, Minified: false);
    }

    /// <summary>
    /// Takes a Smart Health Card JWS token and a Certificate,  Verifies the token signature and contents before decoding it to a SmartHealthCardModel object model 
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="Certificate"></param>
    /// <returns></returns>
    public SmartHealthCardModel VerifyAndDecode(string Token, X509Certificate2 Certificate)
    {
      this.Algorithm = new ES256Algorithm(Certificate);
      InstantiationForVerifying(this.Algorithm);
      JwsDecoder JwsDecoder = new JwsDecoder(this.HeaderSerializer, this.PayloadSerializer, this.JwsSignatureValidator, this.JwsHeaderValidator, this.JwsPayloadValidator);
      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: true);
      return SmartHealthCardModel;
    }

    /// <summary>
    /// Takes a Smart Health Card JWS token and a JsonWebKeySet (JWKS),  Verifies the token signature and contents before decoding it to a SmartHealthCardModel object model 
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="JsonWebKeySet"></param>
    /// <returns></returns>
    public SmartHealthCardModel VerifyAndDecode(string Token, string JsonWebKeySet)
    {
      JsonSerializer JsonSerializer = new JsonSerializer();
      JsonWebKeySet JsonWebKeySetModel = JsonSerializer.FromJson<JsonWebKeySet>(JsonWebKeySet);
      this.Algorithm = ES256Algorithm.FromJWKS(JsonWebKeySetModel.Keys[0].Kid, JsonWebKeySetModel);
      InstantiationForVerifying(this.Algorithm);
      JwsDecoder JwsDecoder = new JwsDecoder(this.HeaderSerializer, this.PayloadSerializer, this.JwsSignatureValidator, this.JwsHeaderValidator, this.JwsPayloadValidator);
      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: true);
      return SmartHealthCardModel;
    }
    /// <summary>
    /// Takes a Smart Health Card JWS token and a Certificate,  Verifies the token signature and contents before decoding it to a SmartHealthCard Json string
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="Certificate"></param>
    /// <returns></returns>
    public string VerifyAndDecodeToJson(string Token, X509Certificate2 Certificate)
    {
      this.Algorithm = new ES256Algorithm(Certificate);
      InstantiationForVerifying(this.Algorithm);
      JwsDecoder JwsDecoder = new JwsDecoder(this.HeaderSerializer, this.PayloadSerializer, this.JwsSignatureValidator, this.JwsHeaderValidator, this.JwsPayloadValidator);
      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: true);
      return SmartHealthCardModelJsonSerializer.ToJson(SmartHealthCardModel, Minified: false);
    }

    private void InstantiationForVerifying(IAlgorithm Algorithm)
    {
      this.JwsSignatureValidator = new JwsSignatureValidator(Algorithm);
      this.JwsPayloadValidator = new SmartHealthCardPayloadValidator();
      this.JwsHeaderValidator = new SmartHealthCardHeaderValidator();
    }

  }
}
