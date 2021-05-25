# SMART Health Card JWS token and QR code generation libaray #

This is an open-source library for encoding/decoding/validating FHIR SMART Health Card JWS tokens and generating their QR Codes

>See the official SMART Health Card specification page : [SMART Health Cards Framework](https://smarthealth.cards/)


## Example of Encoding a SMART Health Card JWS token and generating its QR Code images 
```C#
using SmartHealthCard.Token.Certificates;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token;
using SmartHealthCard.QRCode;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;

namespace SHC.Demo
{
  class Program
  {
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
     
      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get a FHIR Bundle, see FHIR profile site: http://build.fhir.org/ig/dvci/vaccine-credential-ig/branches/main/index.html      
      string FhirBundleJson = "[Smart Health Card FHIR Bundle in Json format]";

      //The base of the Url where a validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      //When the Smart Health Card became valid, (e.g The from date).
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

      //Get the Smart Health Card Jws Token 
      string SmartHealthCardJwsToken = SmartHealthCardEncoder.GetToken(Certificate, SmartHealthCardToEncode);

      //Create list of QR Codes
      SmartHealthCardQRCodeFactory SmartHealthCardQRCodeFactory = new SmartHealthCardQRCodeFactory();
      Bitmap[] QRCodeImageList = SmartHealthCardQRCodeFactory.CreateQRCode(SmartHealthCardJwsToken);

      //Write out QR Code image file.
      //Note that there can be many QR Codes if the FHIR Bundle is large
      for (int i = 0; i < QRCodeImageList.Length; i++)
      {
        QRCodeImageList[i].Save(@$"C:\Temp\SMARTHealthCard\QRCode-{i}.png", System.Drawing.Imaging.ImageFormat.Png);
      }
    }
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
