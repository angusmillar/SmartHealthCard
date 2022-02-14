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
using SmartHealthCard.Token.Encoders;
using System.IO;
using System.Security.Cryptography;
using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardJwksTest
  {

    [Fact]
    public void Jwks_Json_get()
    {
      //### Prepare ######################################################
      
      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();


      //X509Certificate2 Certificate = CertificateSupport.GetCertificate(Thumbprint: CertificateSupport.TestingThumbprint);
      List<X509Certificate2> CertificateList = new() { Certificate, Certificate, Certificate };
      SmartHealthCardJwks SmartHealthCardJwks = new();
      JsonSerializer JsonSerializer = new();

      //### Act ##########################################################
      

      string JwksJson = SmartHealthCardJwks.Get(CertificateList, Minified: false);
      
      //Parse the JSON back to the model
      Result<JsonWebKeySet> JsonWebKeySetResult = JsonSerializer.FromJson<JsonWebKeySet>(JwksJson);
      JsonWebKeySet JsonWebKeySet = JsonWebKeySetResult.Value;

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

    //[Fact]
    //public void invalid_Jwks_Json()
    //{
    //  //### Prepare ######################################################

     
    //  JsonSerializer JsonSerializer = new JsonSerializer();

    //  string SmartHealthCardCovidJson = Utf8EncodingSupport.GetString(ResourceData.InvalidJwks);

    //  //### Act ##########################################################
    //  string InvalidJwksJson = Utf8EncodingSupport.GetString(ResourceData.InvalidJwks);

      

    //  //Parse the JSON back to the model
    //  JsonWebKeySet JsonWebKeySet = JsonSerializer.FromJson<JsonWebKeySet>(InvalidJwksJson);


    //  //### Assert #######################################################
    //  //Assert.NotNull(JwksJson);
    //  Assert.Equal(3, JsonWebKeySet.Keys.Count);
    //  Assert.Equal("EC", JsonWebKeySet.Keys[0].Kty);
    //  Assert.NotNull(JsonWebKeySet.Keys[0].Kid);
    //  Assert.Equal("sig", JsonWebKeySet.Keys[0].Use);
    //  Assert.Equal("ES256", JsonWebKeySet.Keys[0].Alg);
    //  Assert.Equal("P-256", JsonWebKeySet.Keys[0].Crv);
    //  Assert.NotNull(JsonWebKeySet.Keys[0].X);
    //  Assert.NotNull(JsonWebKeySet.Keys[0].Y);
    //}

  }
}
