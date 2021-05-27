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
