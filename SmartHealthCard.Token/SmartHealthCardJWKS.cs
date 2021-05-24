using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Model.Jwks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token
{
  public static class SmartHealthCardJWKS
  {
    public static string Get(X509Certificate2[] CertificateList, bool Minified = true)
    {
      List<JsonWebKey> JsonWebKeySetModelList = new List<JsonWebKey>();
      foreach (X509Certificate2 Certificate in CertificateList)
      {        
        ES256Algorithm Algorithm = new ES256Algorithm(Certificate);        
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
      JsonWebKeySet JsonWebKeySet = new JsonWebKeySet(JsonWebKeySetModelList);

      Newtonsoft.Json.Formatting Formatting = Newtonsoft.Json.Formatting.None;
      if (!Minified)
        Formatting = Newtonsoft.Json.Formatting.Indented;

      return Newtonsoft.Json.JsonConvert.SerializeObject(JsonWebKeySet, Formatting);
    }
  }
}
