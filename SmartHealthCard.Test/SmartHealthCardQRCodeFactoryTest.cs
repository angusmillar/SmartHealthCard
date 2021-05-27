using Hl7.Fhir.Model;
using SmartHealthCard.Test.Model;
using SmartHealthCard.Test.Serializers;
using SmartHealthCard.Test.Support;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.QRCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Drawing;
using System.Drawing.Imaging;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardQRCodeFactoryTest
  {
    [Fact]
    public async void Create_QRCode()
    {
      //### Prepare ######################################################

      //Get the ECC certificate from the Windows Certificate Store by Thumb-print      
      X509Certificate2 Certificate = CertificateSupport.GetCertificate(Thumbprint: "72c78a3460fb27b9ef2ccfae2538675b75363fee");

      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get FHIR bundle
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19FhirBundleExample1();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);

      //The base of the URL where a validator will retie the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      //When the Smart Health Card became valid, the from date.
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //The Uri for the type of VerifiableCredentials
     // Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new List<VerifiableCredentialType>() { VerifiableCredentialType.Covid19 };

      //Create the SmartHealthCardModel
      SmartHealthCardModel SmartHealthCardToEncode = new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the SmartHealthCard Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new SmartHealthCardEncoder();
      X509Certificate2[] CertArray = new X509Certificate2[] { Certificate };
      
      //### Act ##########################################################

      //Get the Smart Health Card retrieve Token 
      string SmartHealthCardJwsToken = await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCardToEncode);

      //Create list of QR Codes
      SmartHealthCardQRCodeEncoder SmartHealthCardQRCodeFactory = new SmartHealthCardQRCodeEncoder();
      List<Bitmap> QRCodeImageList = SmartHealthCardQRCodeFactory.GetQRCodeList(SmartHealthCardJwsToken);

      
      //Write out QR Code to file
      //for (int i = 0; i < QRCodeImageList.Length; i++)
      //{
      //  QRCodeImageList[i].Save(@$"C:\Temp\SMARTHealthCard\QRCode-{i}.png", ImageFormat.Png);
      //}
     
      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));
      Assert.NotNull(QRCodeImageList);
      Assert.Single(QRCodeImageList);

    }

  }
}
