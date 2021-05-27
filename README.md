# SMART Health Card JWS token and QR code generation libaray #

This is an open-source library for encoding/decoding/validating FHIR SMART Health Card JWS tokens and generating their QR Codes

>See the official SMART Health Card specification page : [SMART Health Cards Framework](https://smarthealth.cards/)

>This implementation passes all test found here: [Smart Health Card verifier site](https://demo-portals.smarthealth.cards/VerifierPortal.html)

## Example of Encoding a SMART Health Card JWS token and generating its QR Code images 
```C#
    static void Main(string[] args)
    {
      //Get the ECC certificate from the Windows Certificate Store by Thumbprint
      string CertificateThumbprint = "72c78a3460fb27b9ef2ccfae2538675b75363fee";
      X509Certificate2 Certificate = X509CertificateSupport.GetFirstMatchingCertificate(
            CertificateThumbprint.ToUpper(),
            X509FindType.FindByThumbprint,
            StoreName.My,
            StoreLocation.LocalMachine,
            true
            );

      //Set the Version of FHIR in use
      string FhirVersion = "4.0.1";

      //This libaray does not validate that the FHIR Bundle provided is valid FHIR, it only checks that it is valid JSON.      
      //I strongly suggest you use the FIRELY .NET SDK as found here: https://docs.fire.ly/projects/Firely-NET-SDK/index.html       
      //See the FHIR SMART Health Card FHIR profile site here: http://build.fhir.org/ig/dvci/vaccine-credential-ig/branches/main/index.html   

      //Set a FHIR Bundle as a JSON string. 
      string FhirBundleJson = "[A Smart Health Card FHIR Bundle in Json format]";

      //Set the base of the Url where any validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      //Set when the Smart Health Card becomes valid, (e.g the from date).
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //Set the appropirate VerifiableCredentialsType enum list, for more info see: see: https://smarthealth.cards/vocabulary/
      var VerifiableCredentialTypeList = new List<VerifiableCredentialType>() { VerifiableCredentialType.Covid19 };

      //Instantiate and populate the Smart Health Card Model with the properties we just setup
      SmartHealthCardModel SmartHealthCard = new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the Smart Health Card Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new SmartHealthCardEncoder();

      //Get the Smart Health Card JWS Token 
      string SmartHealthCardJwsToken = SmartHealthCardEncoder.GetToken(Certificate, SmartHealthCard);

      //Instantiate the Smart Health Card QR Code Factory
      SmartHealthCardQRCodeEncoder SmartHealthCardQRCodeEncoder = new SmartHealthCardQRCodeEncoder();

      //Get list of SMART Health Card QR Codes images
      //Note: If the SMART Health Card JWS payload is large then it will be split up into mutiple QR Code images.
      //SMART Health Card QR Code scanners can scan each image in any order to obtain the whole SMART Health Card  
      List<Bitmap> QRCodeImageList = SmartHealthCardQRCodeEncoder.GetQRCodeList(SmartHealthCardJwsToken);

      //Write to file the SMART Health Card QR Codes images      
      for (int i = 0; i < QRCodeImageList.Count; i++)
      {
        QRCodeImageList[i].Save(@$"C:\Temp\SMARTHealthCard\QRCode-{i}.png", System.Drawing.Imaging.ImageFormat.Png);
      }
    }
```

## Example of Decoding and Validating a SMART Health Card JWS token  
```C#
using SmartHealthCard.Token;
using SmartHealthCard.Token.Certificates;
using SmartHealthCard.Token.Model.Shc;
using System;
using System.Security.Cryptography.X509Certificates;

namespace SHC.Demo
{
  class Program
  {
    static void Main(string[] args)
    {
      string SmartHealthCardJwsToken = "[A SMART Health Card JWS token]";
      //Get the ECC certificate from the Windows Certificate Store by Thumbprint
      string CertificateThumbprint = "72c78a3460fb27b9ef2ccfae2538675b75363fee";
      X509Certificate2 Certificate = X509CertificateSupport.GetFirstMatchingCertificate(
            CertificateThumbprint.ToUpper(),
            X509FindType.FindByThumbprint,
            StoreName.My,
            StoreLocation.LocalMachine,
            true
            );

      //Instantiate the SMART Health Card Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder();
      try
      {
        SmartHealthCardModel DecodedSmartHealthCardModel = Decoder.DecodeToSmartHealthCardModel(Certificate, SmartHealthCardJwsToken, Verify: true);
      }
      catch (Exception Exec)
      {
        Console.WriteLine("The SMART Health Card JWS token was invalid, please see mesage below:");
        Console.WriteLine(Exec.Message);
      }
    }
  }
}

```

## How to create a ECC Private/Public keys with OpenSSL ##
>Great example from Scott Brady : [Creating Elliptical Curve Keys using OpenSSL](https://www.scottbrady91.com/OpenSSL/Creating-Elliptical-Curve-Keys-using-OpenSSL)


## Repo owner ##

Angus Millar: angusbmillar@gmail.com
