# SMART Health Card JWS token and QR code generation libaray #

An open-source *MIT License* library for encoding/decoding/validating FHIR SMART Health Card JWS tokens and generating their QR Codes

>See the official SMART Health Card specification page : [SMART Health Cards Framework](https://smarthealth.cards/)

>This implementation passes all test found here: [Smart Health Card verifier site](https://demo-portals.smarthealth.cards/VerifierPortal.html)

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
using SmartHealthCard.Token.Model.Jwks;

namespace SHC.Demo
{
  class Program
  {
    static void Main(string[] args)
    {
      //Get the Certifiacte containing a private Elliptic Curve key using the P-256 curve
      //from the Windows Certificate Store by Thumbprint
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

      //This libaray does not validate that the FHIR Bundle provided is valid FHIR, it only parses it as valid JSON.      
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
  }
}
    
```

## Example of Decoding and Validating a SMART Health Card JWS token  
```C#
using SmartHealthCard.Token.Certificates;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token;
using SmartHealthCard.QRCode;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using SmartHealthCard.Token.Model.Jwks;

namespace SHC.Demo
{
  class Program
  {    
    static void Main(string[] args)
    {
      string SmartHealthCardJwsToken = "[A SMART Health Card JWS token]";
      //Get the Certifiacte containing a private Elliptic Curve key using the P-256 curve
      //from the Windows Certificate Store by Thumbprint
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
      //Optionaly for development, you can provide an implementation of the IJwksProvider interface
      //which allows you to pass a Json Web Key Set (JKWS) that contain the public key used to verifiy you 
      //token's signatures.

      //If you don't do this the default implementation will use the Issuer (iss) value from Smart Health Card
      //token payload to make a HTTP call to obtain the JWKS file, which in a prodcution system it the behavour you want.

      //Yet in development this means you must have a public endpoint to proviode the JWKS.

      //By providing this simple interface implementation (see MyJwksProvider class below) you can successfuly
      //validate signatures in development with out the need for a public endpoint.
      //Of cource you would not do this is production.

      //Here is how you pass that interface implementation to the SmartHealthCardDecoder contructior.
      //SmartHealthCard.Token.Providers.IJwksProvider MyJwksProvider = new MyJwksProvider(Certificate);
      //SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MyJwksProvider);


      try
      {
        //Decode and verifiy, returing an object model of the Smart Health Card, throws exceptions if not valid
        SmartHealthCardModel DecodedSmartHealthCardModel = Decoder.Decode(SmartHealthCardJwsToken, Verify: true);

        //Or decode without verifing, not recommended for production systems
        //SmartHealthCardModel DecodedSmartHealthCard = Decoder.Decode(SmartHealthCardJwsToken);

        //Or decode and verifiy, returing the Smart Health Card as a JSON string, throws exceptions if not valid
        //string DecodedSmartHealthCardJson = Decoder.DecodeToJson(SmartHealthCardJwsToken, Verify: true);

      }
      catch (Exception Exec)
      {
        Console.WriteLine("The SMART Health Card JWS token was invalid, please see mesage below:");
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

    public JsonWebKeySet GetJwks(Uri WellKnownJwksUri)
    {
      //In prodcution the default implmentation of this IJwksProvider interface would
      //retrieve the JWKS file from the provided 'WellKnownJwksUri' Url that is found in
      //the SMART Health Card Token payload. 
      //Yet for development we can just ignore the 'WellKnownJwksUri' url and return our
      //own JWKS which we have generated from our certificate as seen below.
      //This allows you to test before you have a publicly exposed endpoint for you JWKS. 
      SmartHealthCardJwks SmartHealthCardJwks = new SmartHealthCardJwks();
      SmartHealthCard.Token.Model.Jwks.JsonWebKeySet Jwks = SmartHealthCardJwks.GetJsonWebKeySet(new List<X509Certificate2>() { Certificate });
      return Jwks;
    }
  }
}

```

## How to create a ECC Private/Public keys using OpenSSL ##
>Great example from Scott Brady : [Creating Elliptical Curve Keys using OpenSSL](https://www.scottbrady91.com/OpenSSL/Creating-Elliptical-Curve-Keys-using-OpenSSL)


## Repo owner ##

Angus Millar: angusbmillar@gmail.com
