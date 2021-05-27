using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Serializers.Shc;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token
{
  public class SmartHealthCardEncoder
  {
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;
    private readonly IJsonSerializer JsonSerializer;

    public SmartHealthCardEncoder()
    {
      this.JsonSerializer = new JsonSerializer();
      HeaderSerializer = new SmartHealthCardJwsHeaderSerializer(JsonSerializer);
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer(JsonSerializer);
    }

    public SmartHealthCardEncoder(IJwsHeaderSerializer HeaderSerializer, IJwsPayloadSerializer PayloadSerializer)
    {
      this.JsonSerializer = new JsonSerializer();
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = PayloadSerializer;
    }
    public SmartHealthCardEncoder(IJwsHeaderSerializer HeaderSerializer)
    {
      this.JsonSerializer = new JsonSerializer();
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer(JsonSerializer);
    }

    public SmartHealthCardEncoder(IJwsPayloadSerializer PayloadSerializer)
    {
      this.JsonSerializer = new JsonSerializer();
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer(JsonSerializer);
      this.PayloadSerializer = PayloadSerializer;
    }

    /// <summary>
    /// Given a signing certificate and Smart Health Card Model it, will return a Smart Health Card JWS Token
    /// </summary>
    /// <param name="Certificate"></param>
    /// <param name="SmartHealthCard"></param>
    /// <returns></returns>
    public string GetToken(X509Certificate2 Certificate, SmartHealthCardModel SmartHealthCard)
    {
      //Create the Elliptic Curve Signing Algorithm
      IAlgorithm Algorithm = new ES256Algorithm(Certificate, JsonSerializer);
            
      //Create the Smart Health Card JWS Header Model
      var Header = new SmartHealthCareJWSHeaderModel(Algorithm.Name, "DEF", Algorithm.GetKid());

      //Encode the JWS Token passing in the Header and Payload byte arrays from our two custom serializers 
      JwsToken.JwsEncoder JwsEncoder = new JwsToken.JwsEncoder(HeaderSerializer, PayloadSerializer, Algorithm);
      return JwsEncoder.Encode(Header, SmartHealthCard);      

    }
    /// <summary>
    /// Given a signing certificate and list of Smart Health Card Models, it will return a .smart-health-card JSON file
    /// </summary>
    /// <param name="Certificate"></param>
    /// <param name="SmartHealthCardList"></param>
    /// <returns></returns>
    public string GetSmartHealthCardFile(X509Certificate2 Certificate, List<SmartHealthCardModel> SmartHealthCardList)
    {
      //Create the Elliptic Curve Signing Algorithm
      IAlgorithm Algorithm = new ES256Algorithm(Certificate, JsonSerializer);

      //Create the Smart Health Card JWS Header Model
      var Header = new SmartHealthCareJWSHeaderModel(Algorithm.Name, "DEF", Algorithm.GetKid());

      //Encode the JWS Token passing in the Header and Payload byte arrays from our two custom serializers 
      JwsToken.JwsEncoder JwsEncoder = new JwsToken.JwsEncoder(HeaderSerializer, PayloadSerializer, Algorithm);

      //Smart Health Card File object model
      SmartHealthCardFile SmartHealthCardFile = new SmartHealthCardFile();
      foreach (SmartHealthCardModel SmartHealthCard in SmartHealthCardList)
      {
        SmartHealthCardFile.VerifiableCredentialList.Add(JwsEncoder.Encode(Header, SmartHealthCard));
      }
      return JsonSerializer.ToJson(SmartHealthCardFile);      
    }
  }
}
