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
    public static async Task<string> GetJWSCovidExampleOneAsync(X509Certificate2 Certificate, Uri Issuer)
    {
      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get Fhir bundle
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19FhirBundleExample1();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);

      //When the Smart Health Card became valid, the from date.
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //The Uri for the type of VerifiableCredentials
      //Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      //List<Uri> VerifiableCredentialTypeList = new List<Uri>() { VerifiableCredentialType };
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new List<VerifiableCredentialType>() { VerifiableCredentialType.Covid19 };

      //Create the SmartHealthCardModel
      SmartHealthCardModel SmartHealthCardToEncode = new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the SmartHealthCard Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new SmartHealthCardEncoder();

      //Get the Smart Health Card Jws Token 
      return await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCardToEncode);

    }
  }
}
