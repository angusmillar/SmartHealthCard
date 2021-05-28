using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHealthCard.Token;
using System.Security.Cryptography.X509Certificates;
using SmartHealthCard.Test.Support;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Model.Jwks;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardJwksTest
  {

    [Fact]
    public void Jwks_Json_get()
    {
      //### Prepare ######################################################

      X509Certificate2 Certificate = CertificateSupport.GetCertificate(Thumbprint: CertificateSupport.TestingThumbprint);
      List<X509Certificate2> CertificateList = new List<X509Certificate2>() { Certificate, Certificate, Certificate };
      SmartHealthCardJwks SmartHealthCardJwks = new SmartHealthCardJwks();
      JsonSerializer JsonSerializer = new JsonSerializer();
      
      //### Act ##########################################################


      string JwksJson = SmartHealthCardJwks.Get(CertificateList, Minified: false);
      
      //Parse the JSON back to the model
      JsonWebKeySet JsonWebKeySet = JsonSerializer.FromJson<JsonWebKeySet>(JwksJson);


      //### Assert #######################################################
      Assert.NotNull(JwksJson);
      Assert.Equal(3, JsonWebKeySet.Keys.Count);
      Assert.Equal("EC", JsonWebKeySet.Keys[0].Kty);
      Assert.NotNull(JsonWebKeySet.Keys[0].Kid);
      Assert.Equal("sig", JsonWebKeySet.Keys[0].Use);
      Assert.Equal("ES256", JsonWebKeySet.Keys[0].Alg);
      Assert.Equal("P-256", JsonWebKeySet.Keys[0].Crv);
      Assert.NotNull(JsonWebKeySet.Keys[0].X);
      Assert.NotNull(JsonWebKeySet.Keys[0].Y);     
    }

  
  }
}
