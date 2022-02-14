using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Support;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token
{
  /// <summary>
  /// Provided with a list of Certificates with each containing a private Elliptic Curve key using the P-256 curve
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
    /// generate a JSON Web Key Set (JWKS) containing only the public key in an object model form 
    /// </summary>
    /// <param name="CertificateList">List of Certificates containing a private Elliptic Curve key using the P-256 curve</param>
    /// <param name="Minified">Weather to minify the returned JSON or not, default is true</param>
    /// <returns></returns>
    public JsonWebKeySet GetJsonWebKeySet(IEnumerable<X509Certificate2> CertificateList)
    {
      List<JsonWebKey> JsonWebKeySetModelList = new();
      foreach (X509Certificate2 Certificate in CertificateList)
      {
        ES256Algorithm Algorithm = new(Certificate, JsonSerializer);
        Result<string> KidResult = Algorithm.GetKid();        
        Result<string> XResult = Algorithm.GetPointCoordinateX();       
        Result<string> YResult = Algorithm.GetPointCoordinateY();        
        Result ResultCombine = Result.Combine(KidResult, XResult, YResult);
        if (ResultCombine.Failure)
          throw new SmartHealthCardJwksException(ResultCombine.ErrorMessage);

        JsonWebKey JsonWebKeySetModel = new(
          Kty: Algorithm.KeyTypeName,
          Kid: KidResult.Value,
          Use: "sig",
          Alg: Algorithm.Name,
          Crv: Algorithm.CurveName,
          X: XResult.Value,
          Y: YResult.Value);

        JsonWebKeySetModelList.Add(JsonWebKeySetModel);
      }
      return  new JsonWebKeySet(JsonWebKeySetModelList);
    }

    /// <summary>
    /// From the given X509Certificate2 certificate containing a Private Elliptic Curve key, generate a JSON Web Key Set (JWKS) in JSON form
    /// </summary>
    /// <param name="CertificateList">List of Certificates containing a private Elliptic Curve key using the P-256 curve</param>
    /// <param name="Minified">Weather to minify the returned JSON or not, default is true</param>
    /// <returns></returns>
    public string Get(IEnumerable<X509Certificate2> CertificateList, bool Minified = true)
    {
      JsonWebKeySet JsonWebKeySet = GetJsonWebKeySet(CertificateList);
      IJsonSerializer JsonSerializer = new JsonSerializer();
      Result<string> ToJsonResult = JsonSerializer.ToJson(JsonWebKeySet, Minified);
      if (ToJsonResult.Failure)
        throw new SmartHealthCardJwksException(ToJsonResult.ErrorMessage);

      return ToJsonResult.Value;
    }
  }
}
