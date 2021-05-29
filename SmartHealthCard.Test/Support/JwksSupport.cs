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
  }
}
