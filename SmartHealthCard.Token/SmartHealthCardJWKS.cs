using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Serializers.Json;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token
{
  /// <summary>
  /// Provided with a list of Certifiactes wiht each containing a private Elliptic Curve key using the P-256 curve
  /// will generate a JSON Web Key Set (JWKS) file or object model 
  /// </summary>
  public class SmartHealthCardJwks
  {
    private readonly IJsonSerializer JsonSerializer;
    
    /// <summary>    
    /// Default Constructor    
    /// </summary>
    public SmartHealthCardJwks()
    {
      this.JsonSerializer = new JsonSerializer();
    }

    /// <summary>
    /// Provides an implementation of basic JSON serialization
    /// </summary>
    /// <param name="JsonSerializer">Provides an implementation of basic JSON serialization</param>
    public SmartHealthCardJwks(IJsonSerializer JsonSerializer)
    {
      this.JsonSerializer = JsonSerializer;
    }

    /// <summary>
    /// From the given X509Certificate2 certificate containing a Private Elliptic Curve key, 
    /// generate a Json Web Key Set (JWKS) containing only the public key in an object model form 
    /// </summary>
    /// <param name="CertificateList">List of Certifiactes containing a private Elliptic Curve key using the P-256 curve</param>
    /// <param name="Minified">Weather to minify the retunred JSON or not, default is true</param>
    /// <returns></returns>
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

    /// <summary>
    /// From the given X509Certificate2 certificate containing a Private Elliptic Curve key, generate a Json Web Key Set (JWKS) in JSON form
    /// </summary>
    /// <param name="CertificateList">List of Certifiactes containing a private Elliptic Curve key using the P-256 curve</param>
    /// <param name="Minified">Weather to minify the retunred JSON or not, default is true</param>
    /// <returns></returns>
    public string Get(IEnumerable<X509Certificate2> CertificateList, bool Minified = true)
    {
      JsonWebKeySet JsonWebKeySet = GetJsonWebKeySet(CertificateList, Minified);
      IJsonSerializer JsonSerializer = new JsonSerializer();
      return JsonSerializer.ToJson(JsonWebKeySet, Minified);
    }
  }
}
