# SMART Health Card JWS token and QR code generation library #

### An open-source *MIT License* .NET 5.0 library for encoding/decoding/validating FHIR SMART Health Card JWS tokens and generating their QR Codes

&nbsp;

## SMART Health Cards Framework
>See the official SMART Health Card specification page : [SMART Health Cards Framework](https://smarthealth.cards/)

&nbsp;

## Smart Health Card Development
>A fantasic site for testing your development: [Smart Health Card verifier site](https://demo-portals.smarthealth.cards/VerifierPortal.html)

&nbsp;

## How to create a ECC Private/Public keys using OpenSSL ##
>Great example from Scott Brady : [Creating Elliptical Curve Keys using OpenSSL](https://www.scottbrady91.com/OpenSSL/Creating-Elliptical-Curve-Keys-using-OpenSSL)


&nbsp;

## Nuget Packages in this repository

## [SmartHealthCard.Token](https://www.nuget.org/packages/SmartHealthCard.Token/0.1.0-alpha.1): Encode, Decode & Verifiy SMART Health Card JWS tokens  
```
Install-Package SmartHealthCard.QRCode -Version 0.1.0-alpha.1
```


## [SmartHealthCard.QRCode](https://www.nuget.org/packages/SmartHealthCard.QRCode/0.1.0-alpha.1): Encode SMART Health Card JWS token QR Code images
```
Install-Package SmartHealthCard.QRCode -Version 0.1.0-alpha.1
```

&nbsp;



## Example of Encoding a SMART Health Card JWS token and generating its QR Code images 
---
```C#
using SmartHealthCard.QRCode;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Certificates;
using SmartHealthCard.Token.Model.Shc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SHC.EncoderDemo
{
  class Program
  {
    static void Main(string[] args)
    {
      //Run the Encoder demo
      EncoderDemoRunner().Wait();
    }

    static async Task EncoderDemoRunner()
    {
      //Get the Certificate containing a private Elliptic Curve key using the P-256 curve
      //from the Windows Certificate Store by Thumb-print
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

      //This library does not validate that the FHIR Bundle provided is valid FHIR, it only parses it as valid JSON.      
      //I strongly suggest you use the FIRELY .NET SDK as found here: https://docs.fire.ly/projects/Firely-NET-SDK/index.html       
      //See the FHIR SMART Health Card FHIR profile site here: http://build.fhir.org/ig/dvci/vaccine-credential-ig/branches/main/index.html   

      //Set a FHIR Bundle as a JSON string. 
      string FhirBundleJson = "[A Smart Health Card FHIR Bundle in JSON format]";

      //Set the base of the URL where any validator will retrieve the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new Uri("https://sonichealthcare.com/something");

      //Set when the Smart Health Card becomes valid, (e.g the from date).
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //Set the appropriate VerifiableCredentialsType enum list, for more info see: see: https://smarthealth.cards/vocabulary/
      var VerifiableCredentialTypeList = new List<VerifiableCredentialType>() { VerifiableCredentialType.Covid19 };

      //Instantiate and populate the Smart Health Card Model with the properties we just setup
      SmartHealthCardModel SmartHealthCard = new SmartHealthCardModel(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the Smart Health Card Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new SmartHealthCardEncoder();

      //Get the Smart Health Card JWS Token 
      string SmartHealthCardJwsToken = await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCard);

      //Instantiate the Smart Health Card QR Code Factory
      SmartHealthCardQRCodeEncoder SmartHealthCardQRCodeEncoder = new SmartHealthCardQRCodeEncoder();

      //Get list of SMART Health Card QR Codes images
      //Note: If the SMART Health Card JWS payload is large then it will be split up into multiple QR Code images.
      //SMART Health Card QR Code scanners can scan each image in any order to obtain the whole SMART Health Card  
      List<Bitmap> QRCodeImageList = SmartHealthCardQRCodeEncoder.GetQRCodeList(SmartHealthCardJwsToken);

      //Write to file the SMART Health Card QR Codes images      
      for (int i = 0; i < QRCodeImageList.Count; i++)
      {
        QRCodeImageList[i].Save(@$"C:\Temp\SMARTHealthCard\QRCode-{i}.png", System.Drawing.Imaging.ImageFormat.Png);
      }
    }
  }
}
```

## Example of Decoding and Validating a SMART Health Card JWS token  
---
```C#
using SmartHealthCard.Token;
using SmartHealthCard.Token.Certificates;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Model.Shc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SHC.DecoderDemo
{
  class Program
  {
    static void Main(string[] args)
    {
      //Run the Decoder demo
      DecoderDemoRunner().Wait();
    }
    static async Task DecoderDemoRunner()
    {
      string SmartHealthCardJwsToken = "[A SMART Health Card JWS token]";
      //Get the ECC certificate from the Windows Certificate Store by Thumb-print
      string CertificateThumbprint = "72c78a3460fb27b9ef2ccfae2538675b75363fee";
      X509Certificate2 Certificate = X509CertificateSupport.GetFirstMatchingCertificate(
            CertificateThumbprint.ToUpper(),
            X509FindType.FindByThumbprint,
            StoreName.My,
            StoreLocation.LocalMachine,
            true
            );

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder();

      //Useful while in development!! 
      //Optionally for development, you can provide an implementation of the IJwksProvider interface
      //which allows you to pass a JSON Web Key Set (JKWS) that contain the public key used to verify you 
      //token's signatures.

      //If you don't do this the default implementation will use the Issuer (iss) value from Smart Health Card
      //token payload to make a HTTP call to obtain the JWKS file, which in a production system it the behavior you want.

      //Yet in development this means you must have a public endpoint to provide the JWKS.

      //By providing this simple interface implementation (see MyJwksProvider class below) you can successfully
      //validate signatures in development with out the need for a public endpoint.
      //Of course you would not do this is production.

      //Here is how you pass that interface implementation to the SmartHealthCardDecoder constructor.
      //SmartHealthCard.Token.Providers.IJwksProvider MyJwksProvider = new MyJwksProvider(Certificate);
      //SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MyJwksProvider);

      try
      {
        //Decode and verify, returning an object model of the Smart Health Card, throws exceptions if not valid
        SmartHealthCardModel DecodedSmartHealthCardModel = await Decoder.DecodeAsync(SmartHealthCardJwsToken, Verify: true);

        //Or decode without verifying, not recommended for production systems
        //SmartHealthCardModel DecodedSmartHealthCard = await Decoder.DecodeAsync(SmartHealthCardJwsToken);

        //Or decode and verify, returning the Smart Health Card as a JSON string, throws exceptions if not valid
        //string DecodedSmartHealthCardJson = await Decoder.DecodeToJsonAsync(SmartHealthCardJwsToken, Verify: true);
      }
      catch (Exception Exec)
      {
        Console.WriteLine("The SMART Health Card JWS token was invalid, please see message below:");
        Console.WriteLine(Exec.Message);
      }
    }
  }

  //Example implementation of the IJwksProvider interface
  public class MyJwksProvider : SmartHealthCard.Token.Providers.IJwksProvider
  {
    private readonly X509Certificate2 Certificate;
    public MyJwksProvider(X509Certificate2 Certificate)
    {
      this.Certificate = Certificate;
    }

    public Task<JsonWebKeySet> GetJwksAsync(Uri WellKnownJwksUri)
    {
      //In production the default implementation of this IJwksProvider interface would
      //retrieve the JWKS file from the provided 'WellKnownJwksUri' URL that is found in
      //the SMART Health Card Token payload. 
      //Yet for development we can just ignore the 'WellKnownJwksUri' URL and return our
      //own JWKS which we have generated from our certificate as seen below.
      //This allows you to test before you have a publicly exposed endpoint for you JWKS. 
      SmartHealthCardJwks SmartHealthCardJwks = new SmartHealthCardJwks();
      SmartHealthCard.Token.Model.Jwks.JsonWebKeySet Jwks = SmartHealthCardJwks.GetJsonWebKeySet(new List<X509Certificate2>() { Certificate });
      return Task.FromResult(Jwks);
    }
  }
}
```


## Repo owner ##

Angus Millar: angusbmillar@gmail.com
