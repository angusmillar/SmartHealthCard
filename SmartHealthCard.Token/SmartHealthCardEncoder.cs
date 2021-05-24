using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Serializers.Shc;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token
{
  public class SmartHealthCardEncoder
  {
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;

    public SmartHealthCardEncoder()
    {
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer();
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer();
    }

    public SmartHealthCardEncoder(IJwsHeaderSerializer HeaderSerializer, IJwsPayloadSerializer PayloadSerializer)
    {
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = PayloadSerializer;
    }
    public SmartHealthCardEncoder(IJwsHeaderSerializer HeaderSerializer)
    {
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer();
    }

    public SmartHealthCardEncoder(IJwsPayloadSerializer PayloadSerializer)
    {
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer();
      this.PayloadSerializer = PayloadSerializer;
    }

    public string GetToken(X509Certificate2 Certificate, SmartHealthCardModel SmartHealthCardModel)
    {
      //Create the Elliptic Curve Signing Algorithm
      IAlgorithm Algorithm = new ES256Algorithm(Certificate);
            
      //Create the Smart Health Card JWS Header Model
      var Header = new SmartHealthCareJWSHeaderModel(Algorithm.Name, "DEF", Algorithm.GetKid());

      //Encode the JWS Token passing in the Header and Payload byte arrays from our two custom serializers 
      JwsToken.JwsEncoder JwsEncoder = new JwsToken.JwsEncoder(HeaderSerializer, PayloadSerializer, Algorithm);
      return JwsEncoder.Encode(Header, SmartHealthCardModel);      

    }    
  }
}
