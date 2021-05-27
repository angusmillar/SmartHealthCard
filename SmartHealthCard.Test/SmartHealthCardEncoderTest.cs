using Hl7.Fhir.Model;
using Moq;
using SmartHealthCard.Test.Model;
using SmartHealthCard.Test.Serializers;
using SmartHealthCard.Test.Support;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Providers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardEncoderTest
  {
    [Fact]
    public async void Create_Token_Decode_Token()
    {
      //### Prepare ######################################################
      
      //Get the ECC certificate from the Windows Certificate Store by Thumb-print      
      X509Certificate2 Certificate = CertificateSupport.GetCertificate(Thumbprint: "72c78a3460fb27b9ef2ccfae2538675b75363fee");

      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get FHIR bundle
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19FhirBundleExample1();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);
      
      //The base of the URL where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");      

      //When the Smart Health Card became valid, the from date.
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);      
      
      //The Uri for the type of VerifiableCredentials
      //Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new List<VerifiableCredentialType>() { VerifiableCredentialType.Covid19 };

      //Create the SmartHealthCardModel
      SmartHealthCardModel SmartHealthCard = new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the SmartHealthCard Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new SmartHealthCardEncoder();

      //This testing JwksSupport class provides us with a mocked IJwksProvider that will inject the JWKS file
      //rather than make the HTTP call to go get it from a public endpoint.
      IJwksProvider MockedIJwksProvider = JwksSupport.GetMockedIJwksProvider(Certificate, Issuer);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MockedIJwksProvider);

      //### Act ##########################################################

      //Get the Smart Health Card JWS Token 
      
      //string SmartHealthCardJwsToken = Assert.Throws<SmartHealthCardException>(() => SmartHealthCardEncoder.GetToken(Certificate, SmartHealthCardToEncode));
      string SmartHealthCardJwsToken = await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCard);

      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));


      SmartHealthCardModel DecodedSmartHealthCardModle = await Decoder .DecodeAsync(SmartHealthCardJwsToken, Verify: true);

      //Check the IssuanceDate as the same to seconds precision
      DateTimeOffset ActualIssuanceDate = DecodedSmartHealthCardModle.GetIssuanceDate();
      Assert.True((IssuanceDateTimeOffset - ActualIssuanceDate).TotalSeconds < 1);      
      Assert.Equal(Issuer, DecodedSmartHealthCardModle.Issuer);
      Assert.NotNull(DecodedSmartHealthCardModle.VerifiableCredential);
      Assert.Equal(VerifiableCredentialTypeList, DecodedSmartHealthCardModle.VerifiableCredential.VerifiableCredentialTypeList);
      Assert.NotNull(DecodedSmartHealthCardModle.VerifiableCredential.CredentialSubject);
      Assert.Equal(FhirVersion, DecodedSmartHealthCardModle.VerifiableCredential.CredentialSubject.FhirVersion);      
    }

    
  }
}
