using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.JwsToken;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Serializers.Shc;
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

    public string DecodeToJson(X509Certificate2 Certificate, string Token, bool Verify = true)
    {
      this.Algorithm = new ES256Algorithm(Certificate);
      SetupIfVerifying(Verify);
      JwsDecoder JwsDecoder = new JwsDecoder(this.HeaderSerializer, this.PayloadSerializer, this.JwsSignatureValidator, this.JwsHeaderValidator, this.JwsPayloadValidator);
      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token, Verify);
      return SmartHealthCardModelJsonSerializer.ToJson(SmartHealthCardModel, Minified: false);
    }
    public SmartHealthCardModel DecodeToSmartHealthCardModel(X509Certificate2 Certificate, string Token, bool Verify = true)
    {
      this.Algorithm = new ES256Algorithm(Certificate);
      SetupIfVerifying(Verify);
      JwsDecoder JwsDecoder = new JwsDecoder(this.HeaderSerializer, this.PayloadSerializer, this.JwsSignatureValidator, this.JwsHeaderValidator, this.JwsPayloadValidator);
      SmartHealthCardModel SmartHealthCardModel = JwsDecoder.DecodePayload<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token, Verify);
      return SmartHealthCardModel;
    }

    private void SetupIfVerifying(bool Verify)
    {
      if (Verify)
      {
        if (this.Algorithm is null)
          throw new System.NullReferenceException(nameof(this.Algorithm));

        this.JwsSignatureValidator = new JwsSignatureValidator(this.Algorithm);
        this.JwsPayloadValidator = new SmartHealthCardPayloadValidator();
        this.JwsHeaderValidator = new SmartHealthCardHeaderValidator();
      }
    }

  }
}
