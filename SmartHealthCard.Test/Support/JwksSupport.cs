using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Providers;
using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Test.Support
{
  public static class JwksSupport
  {
    public static IJwksProvider GetMockedIJwksProvider(X509Certificate2 Certificate, Uri Issuer)
    {
      List<X509Certificate2> CertificateList = new List<X509Certificate2>() { Certificate };
      SmartHealthCardJwks SmartHealthCardJwks = new SmartHealthCardJwks();
      JsonWebKeySet JsonWebKeySet = SmartHealthCardJwks.GetJsonWebKeySet(CertificateList);
      Uri WellKnownJwksUri = new Uri($"{Issuer.OriginalString}/.well-known/jwks.json");
      var JWKSProviderMock = new Mock<IJwksProvider>();

      JWKSProviderMock.Setup(x => x.GetJwksAsync(WellKnownJwksUri, null)).ReturnsAsync(Result<JsonWebKeySet>.Ok(JsonWebKeySet));
      return JWKSProviderMock.Object;
    }
    
    /// <summary>
    /// Returns a retry Result up to five times 
    /// </summary>
    /// <param name="Issuer"></param>
    /// <returns></returns>
    public static IJwksProvider GetMockedRetryIJwksProvider(Uri Issuer)
    {                  
      Uri WellKnownJwksUri = new Uri($"{Issuer.OriginalString}/.well-known/jwks.json");
      var JWKSProviderMock = new Mock<IJwksProvider>();

      JWKSProviderMock.SetupSequence(x => x.GetJwksAsync(WellKnownJwksUri, null))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the zero atempt"))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the first atempt"))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the second atempt"))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the third atempt"))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the forth atempt"));        
      return JWKSProviderMock.Object;
    }

    /// <summary>
    /// Returns a retry Result four times anmd then success 
    /// </summary>
    /// <param name="Certificate"></param>
    /// <param name="Issuer"></param>
    /// <returns></returns>
    public static IJwksProvider GetMockedRetryFollowedBySuccessIJwksProvider(X509Certificate2 Certificate, Uri Issuer)
    {
      List<X509Certificate2> CertificateList = new List<X509Certificate2>() { Certificate };
      SmartHealthCardJwks SmartHealthCardJwks = new SmartHealthCardJwks();
      JsonWebKeySet JsonWebKeySet = SmartHealthCardJwks.GetJsonWebKeySet(CertificateList);
      Uri WellKnownJwksUri = new Uri($"{Issuer.OriginalString}/.well-known/jwks.json");
      var JWKSProviderMock = new Mock<IJwksProvider>();

      JWKSProviderMock.SetupSequence(x => x.GetJwksAsync(WellKnownJwksUri, null))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the zero atempt"))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the first atempt"))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the second atempt"))
        .ReturnsAsync(Result<JsonWebKeySet>.Retry("This was the third atempt"))        
        .ReturnsAsync(Result<JsonWebKeySet>.Ok(JsonWebKeySet));
      return JWKSProviderMock.Object;
    }


  }
}
