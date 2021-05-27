using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Serializers.Json;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token
{
  public class SmartHealthCardJwks
  {
    private readonly IJsonSerializer JsonSerializer;
    public SmartHealthCardJwks()
    {
      this.JsonSerializer = new JsonSerializer();
    }

    public SmartHealthCardJwks(IJsonSerializer JsonSerializer)
    {
      this.JsonSerializer = JsonSerializer;
    }

    public JsonWebKeySet GetJsonWebKeySet(IEnumerable<X509Certificate2> CertificateList, bool Minified = true)
    {
      List<JsonWebKey> JsonWebKeySetModelList = new List<JsonWebKey>();
      foreach (X509Certificate2 Certificate in CertificateList)
      {
        ES256Algorithm Algorithm = new ES256Algorithm(Certificate, JsonSerializer);

        JsonWebKey JsonWebKeySetModel = new JsonWebKey(
          Kty: Algorithm.KeyTypeName,
          Kid: Algorithm.GetKid(),
          Use: "sig",
          Alg: Algorithm.Name,
          Crv: Algorithm.CurveName,
          X: Algorithm.GetPointCoordinateX(),
          Y: Algorithm.GetPointCoordinateY());

        JsonWebKeySetModelList.Add(JsonWebKeySetModel);
      }
      return  new JsonWebKeySet(JsonWebKeySetModelList);
    }

    public string Get(IEnumerable<X509Certificate2> CertificateList, bool Minified = true)
    {
      JsonWebKeySet JsonWebKeySet = GetJsonWebKeySet(CertificateList, Minified);
      IJsonSerializer JsonSerializer = new JsonSerializer();
      return JsonSerializer.ToJson(JsonWebKeySet, Minified);
    }
  }
}
