using Hl7.Fhir.Model;
using SmartHealthCard.Test.Model;
using SmartHealthCard.Test.Serializers;
using SmartHealthCard.Test.Support;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Model.Shc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardEncoderTest
  {
    [Fact]
    public void Create_Token_Decode_Token()
    {
      //### Prepare ######################################################
      
      //Get the ECC certificate from the Windows Certificate Store by Thumbprint      
      X509Certificate2 Certificate = CertificateSupport.GetCertificate(Thumbprint: "72c78a3460fb27b9ef2ccfae2538675b75363fee");

      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get Fhir bundle
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19FhirBundleExample1();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);
      
      //The base of the Url where a validator will retive the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      //When the Smart Health Card became valid, the from date.
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);      
      
      //The Uri for the type of VerifiableCredentials
      Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      List<Uri> VerifiableCredentialTypeList = new List<Uri>() { VerifiableCredentialType };

      //Create the SmartHealthCardModel
      SmartHealthCardModel SmartHealthCardToEncode = new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the SmartHealthCard Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new SmartHealthCardEncoder();

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder();

      //### Act ##########################################################

      //Get the Smart Health Card Jws Token 
      string SmartHealthCardJwsToken = SmartHealthCardEncoder.GetToken(Certificate, SmartHealthCardToEncode);

      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));


      SmartHealthCardModel DecodedSmartHealthCardModle = Decoder.DecodeToSmartHealthCardModel(Certificate, SmartHealthCardJwsToken, Verify: true);

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
