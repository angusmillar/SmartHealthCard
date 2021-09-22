using SmartHealthCard.Token;
using SmartHealthCard.Token.Certificates;
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
    static void Main(string[] args)
    {
      //Run the Decoder demo
      DecoderDemoRunner().Wait();
    }
    static async Task DecoderDemoRunner()
    {
      //Get the ECC certificate from the Windows Certificate Store by Thumb-print
      string CertificateThumbprint = "72c78a3460fb27b9ef2ccfae2538675b75363fee";
      X509Certificate2 Certificate = X509CertificateSupport.GetFirstMatchingCertificate(
            CertificateThumbprint.ToUpper(),
            X509FindType.FindByThumbprint,
            StoreName.My,
            StoreLocation.LocalMachine,
            true
            );

      //Below is a single QR Code's raw data
      string QRCodeRawData = "shc:/567629595326546034602....etc";
      
      //We must add it to a string list as you may have many if the payload was large and spread accross many QR Code images.
      List<string> QRCodeRawDataList = new List<string>() { QRCodeRawData };

      //Next we use the SmartHealthCardQRCodeDecoder to convert the set of QR Code data into its equivalent JWS token
      var SmartHealthCardQRCodeDecoder = new SmartHealthCard.QRCode.SmartHealthCardQRCodeDecoder();
      string SmartHealthCardJwsToken = SmartHealthCardQRCodeDecoder.GetToken(QRCodeRawDataList);

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder();      

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
        Console.WriteLine("The SMART Health Card is invalid");
        Console.WriteLine(SignatureInvalidException.Message);
      }
      catch (SmartHealthCardDecoderException DecoderException)
      {
        Console.WriteLine("The SMART Health Card Decoder has found an error, please see message below::");
        Console.WriteLine(DecoderException.Message);
      }
      catch (Exception Exception)
      {
        Console.WriteLine("Oops, there is an unexpected development exception");
        Console.WriteLine(Exception.Message);
      }
    }
  }

  //While in development!! 
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

  //Where below is an example implementation of the IJwksProvider interface
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
      SmartHealthCardJwks SmartHealthCardJwks = new SmartHealthCardJwks();
      JsonWebKeySet Jwks = SmartHealthCardJwks.GetJsonWebKeySet(new List<X509Certificate2>() { Certificate });
      return Task.FromResult(Result<JsonWebKeySet>.Ok(Jwks));
    }   
  }
}
