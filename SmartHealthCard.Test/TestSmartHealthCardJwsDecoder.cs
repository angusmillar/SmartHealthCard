using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHealthCard.Token.JwsToken;
using System.Security.Cryptography.X509Certificates;
using SmartHealthCard.Test.Support;
using SmartHealthCard.Token.Providers;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Exceptions;

namespace SmartHealthCard.Test
{
  public class TestSmartHealthCardJwsDecoderOne
  {

    [Fact]
    public async void Test_JwksProviderReturningRetries()
    {
      //### Prepare ######################################################
      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();

      //The base of the URL where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");
      string SmartHealthCardJwsToken = await SmartHealthCardJwsSupport.GetJWSCovidExampleOneAsync(Certificate, Issuer);

      //This MockedRetryIJwksProvider only retunes a retry, it does not retuns a JWKS
      //IJwksProvider MockedRetryIJwksProvider = JwksSupport.GetMockedRetryIJwksProvider(Certificate,Issuer);
      IJwksProvider MockedRetryIJwksProvider = JwksSupport.GetMockedRetryIJwksProvider(Issuer);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MockedRetryIJwksProvider);

      //### Act #######################################################

      //Verify and Decode      
      Func<Task> Act = () => Decoder.DecodeAsync(SmartHealthCardJwsToken, Verify: true);

      //### Assert #######################################################

      var exception = await Assert.ThrowsAsync<SmartHealthCardDecoderException>(Act);
      Assert.StartsWith("Unable to obtain the JsonWebKeySet (JWKS) from :", exception.Message);
      Assert.Contains("Atempt 1 after", exception.Message);
    }
  }
  public class TestSmartHealthCardJwsDecoderTwo
  {
    [Fact]
    public async void Test_JwksProviderReturningRetriesFollowedBySuccess()
    {
      //### Prepare ######################################################
      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();

      //The base of the URL where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");
      string SmartHealthCardJwsToken = await SmartHealthCardJwsSupport.GetJWSCovidExampleOneAsync(Certificate, Issuer);

      //This MockedRetryIJwksProvider only retunes a retry, it does not retuns a JWKS
      //IJwksProvider MockedRetryIJwksProvider = JwksSupport.GetMockedRetryIJwksProvider(Certificate,Issuer);
      IJwksProvider MockedRetryIJwksProvider = JwksSupport.GetMockedRetryFollowedBySuccessIJwksProvider(Certificate, Issuer);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MockedRetryIJwksProvider);

      //### Act #######################################################

      //Verify and Decode      
      SmartHealthCardModel SmartHealthCardModel = await Decoder.DecodeAsync(SmartHealthCardJwsToken, Verify: true);

      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));
      Assert.NotNull(SmartHealthCardModel);
    }
  }
}
