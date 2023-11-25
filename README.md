# SMART Health Card Token & QR Code generation
Create and validate SMART Health Card JWS tokens and their QR codes

An open-source *MIT Licensed* C# .NET 8 library for encoding/decoding/validating FHIR SMART Health Card JWS tokens and encoding/decoding their QR codes

&nbsp;

## Nuget Packages
SMART Health Card JWS token encoding, decoding & verifying: [SmartHealthCard.Token](https://www.nuget.org/packages/SmartHealthCard.Token)   
```
Install-Package SmartHealthCard.Token -Version 8.0.0
```

SMART Health Card QR Code image encoding, decoding to JWS: [SmartHealthCard.QRCode](https://www.nuget.org/packages/SmartHealthCard.QRCode)
```
Install-Package SmartHealthCard.QRCode -Version 8.0.0
```

&nbsp;


## Examples

SMART Health cards are somewhat of an onion skined affair consisting of many layers. Their innner most layer consists of HL7 [FHIR](https://www.hl7.org/fhir/overview.html) resources in a JSON format. These represent the standadised chunks of health information that the SMART Health Cards hold and can digitally verify. To create these resources it is highly advised that you use the [The offical .NET FHIR API libaray](https://github.com/FirelyTeam/firely-net-sdk). 

This SMART Health card libaray will handle every other layer required to produce at a SMART Health Card QR Code image file, as specified in the SMART Health Cards Framework's [Technical Specifications](https://spec.smarthealth.cards/). Furthermore, from scanned QR Code data, this libaray will allow you to extract the FHIR resource payloads and confirm their validity against the cards digital signature. Below are two examples covering both scenarios.

&nbsp;

**Encoding a SMART Health Card JWS token and generating its QR Code image files**

```C#
using SkiaSharp;
using SmartHealthCard.QRCode;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Certificates;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Shc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SHC.EncoderDemo
{
  class Program
  {    
    static void Main()
    {
      //Run the Encoder demo
      EncoderDemoRunner().Wait();
    }
   
    static async Task EncoderDemoRunner()
    {
      //Get the Certificate containing a private Elliptic Curve key using the P-256 curve
      //from the Windows Certificate Store by Thumb-print
      string CertificateThumbprint = "89faeeea715ab86bd0ade30830cc313ff76cca79".ToUpper();
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
      Uri Issuer = new("https://sonichealthcare.com/something");

      //Set when the Smart Health Card becomes valid, (e.g the from date).
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //Set the appropriate VerifiableCredentialsType enum list, for more info see: see: https://smarthealth.cards/vocabulary/
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new()
      {        
        VerifiableCredentialType.HealthCard,
        VerifiableCredentialType.Covid19
      };

      //Instantiate and populate the Smart Health Card Model with the properties we just setup
      SmartHealthCardModel SmartHealthCard = new(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the Smart Health Card Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new();

      string SmartHealthCardJwsToken = string.Empty;
      try
      {
        //Get the Smart Health Card JWS Token 
        SmartHealthCardJwsToken = await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCard);
      }
      catch (SmartHealthCardEncoderException EncoderException)
      {
        Console.WriteLine("The SMART Health Card Encoder has found an error, please see message below:");
        Console.WriteLine(EncoderException.Message);
      }
      catch (Exception Exception)
      {
        Console.WriteLine("Oops, there is an unexpected development exception");
        Console.WriteLine(Exception.Message);
      }

      //Instantiate the Smart Health Card QR Code Factory
      SmartHealthCardQRCodeEncoder SmartHealthCardQRCodeEncoder = new();

      //Get list of SMART Health Card QR Codes images
      //Note: If the SMART Health Card JWS payload is large then it will be split up into multiple QR Code images.
      //SMART Health Card QR Code scanners can scan each image in any order to obtain the whole SMART Health Card  
      List<SKBitmap> QRCodeImageList = SmartHealthCardQRCodeEncoder.GetQRCodeList(SmartHealthCardJwsToken);

      //Write the SMART Health Card QR Codes images to file      
      for (int i = 0; i < QRCodeImageList.Count; i++)
      {
        using SKData SKData = QRCodeImageList[i].Encode(SKEncodedImageFormat.Png, 90);        
        using FileStream stream = File.OpenWrite(@$"C:\Temp\SMARTHealthCard\QRCode-{i}.png");
        SKData.SaveTo(stream);        
      }
    }
  }
}
```
&nbsp;
&nbsp;
&nbsp;

**Decode and validate a SMART Health Card's QR Code**

```C#
using SmartHealthCard.Token;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Support;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SHC.DecoderDemo
{
  class Program
  {
    static void Main()
    {
      //Run the Decoder demo
      DecoderDemoRunner().Wait();
    }
    static async Task DecoderDemoRunner()
    {
      //Below is to be a single QR Code's raw data
      string QRCodeRawData = "shc:/567629595326546034602....etc";
      
      //We must add it to a string list as you may have many if the payload was large and spread accross many QR Code images.
      List<string> QRCodeRawDataList = new() { QRCodeRawData };

      //Next we use the SmartHealthCardQRCodeDecoder to convert the set of QR Code data into its equivalent JWS token
      var SmartHealthCardQRCodeDecoder = new SmartHealthCard.QRCode.SmartHealthCardQRCodeDecoder();
      string SmartHealthCardJwsToken = SmartHealthCardQRCodeDecoder.GetToken(QRCodeRawDataList);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new();      

      try
      {
        //Decode and verify the JWS, returning an object model of the Smart Health Card, throws exceptions if not valid
        SmartHealthCardModel DecodedSmartHealthCardModel = await Decoder.DecodeAsync(SmartHealthCardJwsToken, Verify: true);

        //Or decode without verifying, not recommended for production systems
        //SmartHealthCardModel DecodedSmartHealthCard = await Decoder.DecodeAsync(SmartHealthCardJwsToken);

        //Or decode and verify, returning the Smart Health Card as a JSON string, throws exceptions if not valid
        //string DecodedSmartHealthCardJson = await Decoder.DecodeToJsonAsync(SmartHealthCardJwsToken, Verify: true);
      }
      catch (SmartHealthCardSignatureInvalidException SignatureInvalidException)
      {
        //The decoder successfully validated the JWS signature and found it to be invalid
        Console.WriteLine("The SMART Health Card's signing signature is invalid");
        Console.WriteLine(SignatureInvalidException.Message);
      }
      catch (SmartHealthCardJwksRequestException JwksRequestException)
      {
        //The decoder was unable to retrieved JWKS file that contains the token's public signing key.
        //This is likely due to an Internet connectivity issue, the exception message will say more.
        Console.WriteLine("The SMART Health Card's public key can not be retrieved.");
        Console.WriteLine(JwksRequestException.Message);
      }
      catch (SmartHealthCardDecoderException DecoderException)
      {
        //The decoder ran into an error while attempting to decode the JWS token and its SMART Health card payload.
        //It is likely that the SMART Health card token is incorrectly structured 
        Console.WriteLine("The SMART Health Card Decoder has encountered an error, please see message below::");
        Console.WriteLine(DecoderException.Message);
      }
      catch (Exception Exception)
      {
        //Any unexpected errors that the decoder did not protect against. 
        Console.WriteLine("Oops, there is an unexpected development exception.");
        Console.WriteLine(Exception.Message);
      }
    }
  }
}
```


## While working in development

Optionally while working in development, you can provide an implementation of the ```IJwksProvider``` interface to the constructor of the ```SmartHealthCardDecoder```. This allows you to provide a JSON Web Key Set (JKWS) containing the public key required to verify the token's signature.

If not provided, the default implementation will use the Issuer's (iss) URL found within the scanned Smart Health Card token to make a HTTP call to obtain its matching JWKS file (public Key). This is required to validate the token's signature and is the desired behavior for a production system.

However, this default behavior mean a public endpoint is required to allow the JWKS (public key) to be sourced.

By providing the below implementation of the ```IJwksProvider``` interface you can successfully validate signatures in development with out the need for a public endpoint. Though you would not do this in a production system.

**Example implementation of the ```IJwksProvider``` interface**
```C#
  public class MyJwksProvider : SmartHealthCard.Token.Providers.IJwksProvider
  {
    private readonly X509Certificate2 Certificate;
    public MyJwksProvider(X509Certificate2 Certificate)
    {
      this.Certificate = Certificate;
    }

    public Task<Result<JsonWebKeySet>> GetJwksAsync(Uri WellKnownJwksUri, CancellationToken? CancellationToken = null)
    {
      //In production the default implementation of this IJwksProvider interface would
      //retrieve the JWKS file from the provided 'WellKnownJwksUri' URL that is found in
      //the SMART Health Card Token payload. 
      //Yet for development we can just ignore the 'WellKnownJwksUri' URL and return our
      //own JWKS which we have generated from our certificate as seen below.
      //This allows you to test before you have a publicly exposed endpoint for you JWKS. 
      //Alternatively you could not do this and use a service such as : https://ngrok.com/
      SmartHealthCardJwks SmartHealthCardJwks = new();
      JsonWebKeySet Jwks = SmartHealthCardJwks.GetJsonWebKeySet(new List<X509Certificate2>() { Certificate });
      return Task.FromResult(Result<JsonWebKeySet>.Ok(Jwks));
    }   
  }

```

**Example of passing the above ```IJwksProvider``` interface implementation to the ```SmartHealthCardDecoder```**

```C#    
  //Get the ECC certificate from the Windows Certificate Store by Thumb-print
  string CertificateThumbprint = "72c78a3460fb27b9ef2ccfae2538675b75363fee";
  X509Certificate2 Certificate = X509CertificateSupport.GetFirstMatchingCertificate(
        CertificateThumbprint.ToUpper(),
        X509FindType.FindByThumbprint,
        StoreName.My,
        StoreLocation.LocalMachine,
        true
        );
  
  SmartHealthCard.Token.Providers.IJwksProvider MyJwksProvider = new MyJwksProvider(Certificate);
  
  SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder(MyJwksProvider);

```




&nbsp;

## SMART Health Card developer resources
&nbsp;

**The SMART Health Card framework standard**

>The official SMART Health Card specification page : [SMART Health Cards Framework](https://smarthealth.cards/)

&nbsp;

**Smart Health Card Development**

>A fantastic site for testing your development: [Smart Health Card verifier site](https://demo-portals.smarthealth.cards/VerifierPortal.html)

&nbsp;

**How to create a ECC Private/Public keys using OpenSSL**

>Great example from Scott Brady : [Creating Elliptical Curve Keys using OpenSSL](https://www.scottbrady91.com/OpenSSL/Creating-Elliptical-Curve-Keys-using-OpenSSL)

&nbsp;



## Breaking change from .NET 5 to .NET 6


**From .NET 6 onwards the ```SmartHealthCardQRCodeEncoder``` retruns a ```SKData``` datatype instead of the older ```Bitmap``` datatype when encoding QR Code images**



As .NET's ```System.Drawing.Common``` NuGet package is now attributed as a Windows-specific library, and it can no longer be used in multi-platform libraries. This library's ```SmartHealthCard.QRCode``` project has switched to the SkiaSharp library and its SKData datatype for the QR Code images rather than the older Bitmap datatype. You can read more about this change from Microsoft here:  [System.Drawing.Common only supported on Windows](https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only) 

The ```SHC.EncoderDemo``` example seen above shows the use of this new ```SKData``` datatype along with the creation of ```.png``` QR Code image files.  

&nbsp;

## Repo owner ##

Angus Millar: angusbmillar@gmail.com
