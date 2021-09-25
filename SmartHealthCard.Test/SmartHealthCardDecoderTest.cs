using Hl7.Fhir.Model;
using SmartHealthCard.Test.Model;
using SmartHealthCard.Test.Serializers;
using SmartHealthCard.Test.Support;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Providers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardDecoderTest
  {
    [Fact]
    public async void Decode_Token_Verify_with_JWKS()
    {
      //### Prepare ######################################################
      
      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();

      List<X509Certificate2> CertificateList = new List<X509Certificate2>() { Certificate };

      //The base of the URL where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");
      string SmartHealthCardJwsToken = await SmartHealthCardJwsSupport.GetJWSCovidDetectedExampleOneAsync(Certificate, Issuer);
      
      //This testing JwksSupport class provides us with a mocked IJwksProvider that will inject the JWKS file
      //rather than make the HTTP call to go get it from a public endpoint.
      IJwksProvider MockedIJwksProvider = JwksSupport.GetMockedIJwksProvider(Certificate, Issuer);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MockedIJwksProvider);

      //### Act #######################################################
      SmartHealthCardModel SmartHealthCardModel = await Decoder.DecodeAsync(SmartHealthCardJwsToken, Verify: true);

      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));
      Assert.NotNull(SmartHealthCardModel);      
    }

    [Fact]
    public async void Decode_Token_Verify_with_Certificate()
    {
      //### Prepare ######################################################
      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();

      //The base of the URL where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");
      string SmartHealthCardJwsToken = await SmartHealthCardJwsSupport.GetJWSCovidDetectedExampleOneAsync(Certificate, Issuer);

      //This testing JwksSupport class provides us with a mocked IJwksProvider that will inject the JWKS file
      //rather than make the HTTP call to go get it from a public endpoint.
      IJwksProvider MockedIJwksProvider = JwksSupport.GetMockedIJwksProvider(Certificate, Issuer);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MockedIJwksProvider);

      //### Act #######################################################

      //Verify and Decode
      SmartHealthCardModel SmartHealthCardModel = await Decoder.DecodeAsync(SmartHealthCardJwsToken, Verify: true);

      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));
      Assert.NotNull(SmartHealthCardModel);
    }

    [Fact]
    public async void Decode_Token_Verify_InvalidTokenSignature_Certificate()
    {
      //### Prepare ######################################################
      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();

      //The base of the URL where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      string SmartHealthCardJwsTokenCovidDetected = await SmartHealthCardJwsSupport.GetJWSCovidDetectedExampleOneAsync(Certificate, Issuer);
      string SmartHealthCardJwsTokenCovidNotDetected = await SmartHealthCardJwsSupport.GetJWSCovidNotDetectedExampleOneAsync(Certificate, Issuer);

      //Here we have taken the Payload of a NotDetected Covid test result and substituted it into
      //a token build with a Detected Covid Test result, so the signature is now be invalid for it's payload.
      string[] JWSSplitCovidNotDetected = SmartHealthCardJwsTokenCovidNotDetected.Split('.');
      string[] JWSSplitCovidDetected = SmartHealthCardJwsTokenCovidDetected.Split('.');
      string FraudulentSmartHealthCardJwsToken = string.Join('.', JWSSplitCovidDetected[0], JWSSplitCovidNotDetected[1], JWSSplitCovidDetected[2]);

      //This testing JwksSupport class provides us with a mocked IJwksProvider that will inject the JWKS file
      //rather than make the HTTP call to go get it from a public endpoint.
      IJwksProvider MockedIJwksProvider = JwksSupport.GetMockedIJwksProvider(Certificate, Issuer);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MockedIJwksProvider);

      //### Act #######################################################

      //Verify and Decode
      SmartHealthCardSignatureInvalidException Exec = await Assert.ThrowsAsync<SmartHealthCardSignatureInvalidException>(() => Decoder.DecodeAsync(FraudulentSmartHealthCardJwsToken, Verify: true));

      //### Assert #######################################################
      Assert.Equal("The JWS signing signature is invalid.", Exec.Message);

    }

    [Fact]
    public async void Decode_Token_Verify_JWKS_Is_Inaccessible()
    {
      //### Prepare ######################################################
      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();

      //The base of the URL where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      string SmartHealthCardJwsTokenCovidDetected = await SmartHealthCardJwsSupport.GetJWSCovidDetectedExampleOneAsync(Certificate, Issuer);
      string SmartHealthCardJwsTokenCovidNotDetected = await SmartHealthCardJwsSupport.GetJWSCovidNotDetectedExampleOneAsync(Certificate, Issuer);

      //Here we have taken the Payload of a NotDetected Covid test result and substituted it into
      //a token build with a Detected Covid Test result, so the signature is now be invalid for it's payload.
      string[] JWSSplitCovidNotDetected = SmartHealthCardJwsTokenCovidNotDetected.Split('.');
      string[] JWSSplitCovidDetected = SmartHealthCardJwsTokenCovidDetected.Split('.');
      string FraudulentSmartHealthCardJwsToken = string.Join('.', JWSSplitCovidDetected[0], JWSSplitCovidNotDetected[1], JWSSplitCovidDetected[2]);

      //This testing JwksSupport class provides us with a mocked IJwksProvider that will inject the JWKS file
      //rather than make the HTTP call to go get it from a public endpoint.
      IJwksProvider MockedIJwksProvider = JwksSupport.GetMockedIJwksProvider(Certificate, Issuer);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MockedIJwksProvider);

      //### Act #######################################################

      //Verify and Decode
      SmartHealthCardSignatureInvalidException Exec = await Assert.ThrowsAsync<SmartHealthCardSignatureInvalidException>(() => Decoder.DecodeAsync(FraudulentSmartHealthCardJwsToken, Verify: true));

      //### Assert #######################################################
      Assert.Equal("The JWS signing signature is invalid.", Exec.Message);

    }


  }
}
