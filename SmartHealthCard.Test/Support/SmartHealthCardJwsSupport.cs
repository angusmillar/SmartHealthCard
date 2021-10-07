using Hl7.Fhir.Model;
using SmartHealthCard.Test.Model;
using SmartHealthCard.Test.Serializers;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Model.Shc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Test.Support
{
  public static class SmartHealthCardJwsSupport
  {
    public static async Task<string> GetJWSCovidNotDetectedExampleOneAsync(X509Certificate2 Certificate, Uri Issuer)
    {
      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get FHIR bundle
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19NotDetectedFhirBundleExample();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);

      //When the Smart Health Card became valid, the from date.
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //The Uri for the type of VerifiableCredentials
      // Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new List<VerifiableCredentialType>()
      {
        VerifiableCredentialType.VerifiableCredential,
        VerifiableCredentialType.HealthCard,
        VerifiableCredentialType.Covid19
      };

      //Create the SmartHealthCardModel
      SmartHealthCardModel SmartHealthCardToEncode = new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the SmartHealthCard Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new SmartHealthCardEncoder();

      //Get the Smart Health Card JWS Token 
      return await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCardToEncode);

    }

    public static async Task<string> GetJWSCovidDetectedExampleOneAsync(X509Certificate2 Certificate, Uri Issuer)
    {
      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get FHIR bundle
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19DetectedFhirBundleExample();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);

      //When the Smart Health Card became valid, the from date.
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //The Uri for the type of VerifiableCredentials
      // Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new List<VerifiableCredentialType>()
      {
        VerifiableCredentialType.VerifiableCredential,
        VerifiableCredentialType.HealthCard,
        VerifiableCredentialType.Covid19
      };

      //Create the SmartHealthCardModel
      SmartHealthCardModel SmartHealthCardToEncode = new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the SmartHealthCard Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new SmartHealthCardEncoder();

      //Get the Smart Health Card JWS Token 
      return await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCardToEncode);

    }
  }
}
