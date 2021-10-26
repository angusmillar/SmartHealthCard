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

    /// <summary>
    /// Test that an invalid IssuanceDate throws and exception with the correct message.
    /// That is an IssuanceDate that is in the future by more than 2 mins. This test case sets it to 3 min into the future.
    /// </summary>
    [Fact]
    public async void Decode_Token_Verify_IsTrue_IssuanceDate_Is_Invalid()
    {
      //### Prepare ######################################################

      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();

      List<X509Certificate2> CertificateList = new List<X509Certificate2>() { Certificate };

      //The base of the URL where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      //Gets a JWS token with an IssuanceDate that is 3 min into the future from now
      string SmartHealthCardJwsToken = await SmartHealthCardJwsSupport.GetJWSCovidIssuanceDateInvalidExampleAsync(Certificate, Issuer);

      //This testing JwksSupport class provides us with a mocked IJwksProvider that will inject the JWKS file
      //rather than make the HTTP call to go get it from a public endpoint.
      IJwksProvider MockedIJwksProvider = JwksSupport.GetMockedIJwksProvider(Certificate, Issuer);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MockedIJwksProvider);

      //### Act #######################################################     
     
      Func<System.Threading.Tasks.Task> Act = () => Decoder.DecodeAsync(SmartHealthCardJwsToken, Verify: true);

      //### Assert #######################################################

      var exception = await Assert.ThrowsAsync<SmartHealthCardDecoderException>(Act);
      Assert.StartsWith("The token's Issuance Date (nbf) timestamp is earlier than the current date and time. The token is not valid untill:", exception.Message);      
      
    }

    [Fact]
    public void Decode_Token_Verify_IsTrue_IssuanceDate_Is_Before_minimum_Allowed_Date()
    {
      //### Prepare ######################################################

      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get FHIR bundle
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19NotDetectedFhirBundleExample();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);

      //IssuanceDate can not be Before : 00:00:00 UTC on 1 January 1970
      DateTimeOffset IssuanceDateTimeOffset = new DateTimeOffset(1970, 01, 01, 00, 00, 00, new TimeSpan(0, 0, 0));
      IssuanceDateTimeOffset = IssuanceDateTimeOffset.AddSeconds(-1);

      //The Uri for the type of VerifiableCredentials
      // Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new List<VerifiableCredentialType>()
      {
        VerifiableCredentialType.VerifiableCredential,
        VerifiableCredentialType.HealthCard,
        VerifiableCredentialType.Covid19
      };

      //### Act #######################################################     

      //Create the SmartHealthCardModel     
      var ex = Assert.Throws<SmartHealthCardPayloadException>(() => new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset, 
                                                                                             new VerifiableCredential(VerifiableCredentialTypeList, 
                                                                                             new CredentialSubject(FhirVersion, FhirBundleJson))));
      //### Assert #######################################################

      Assert.Equal("The provided IssuanceDate is a date and time before the minimum allowed date and time of: 00:00:00 UTC on 1 January 1970", ex.Message);

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
