using Hl7.Fhir.Model;
using SmartHealthCard.Test.Model;
using SmartHealthCard.Test.Serializers;
using SmartHealthCard.Test.Support;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Model.Shc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardDecoderTest
  {
    [Fact]
    public void Decode_Token_Verify_with_JWKS()
    {
      //### Prepare ######################################################
      //Get the ECC certificate from the Windows Certificate Store by Thumbprint      
      X509Certificate2 Certificate = CertificateSupport.GetCertificate(Thumbprint: "72c78a3460fb27b9ef2ccfae2538675b75363fee");
      List<X509Certificate2> CertificateList = new List<X509Certificate2>() { Certificate };
      string SmartHealthCardJwsToken = SmartHealthCardJwsSupport.GetJWSCovidExampleOne(Certificate);
      string JWKSFile = SmartHealthCardJWKS.Get(CertificateList.ToArray(), Minified: false);
      
      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder();

      //### Act #######################################################
      SmartHealthCardModel SmartHealthCardModel =Decoder.VerifyAndDecode(SmartHealthCardJwsToken, JWKSFile);


      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));
      Assert.NotNull(SmartHealthCardModel);

      
    }

    public void Decode_Token_Verify_with_Certificate()
    {
      //### Prepare ######################################################
      //Get the ECC certificate from the Windows Certificate Store by Thumbprint      
      X509Certificate2 Certificate = CertificateSupport.GetCertificate(Thumbprint: "72c78a3460fb27b9ef2ccfae2538675b75363fee");
     
      //Get Smart Health Card Token
      string SmartHealthCardJwsToken = SmartHealthCardJwsSupport.GetJWSCovidExampleOne(Certificate);     

      //Instantiate the SmartHealthCard Decoder
      SmartHealthCardDecoder Decoder = new SmartHealthCardDecoder();

      //### Act #######################################################

      //Verify and Decode
      SmartHealthCardModel SmartHealthCardModel = Decoder.VerifyAndDecode(SmartHealthCardJwsToken, Certificate);

      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));
      Assert.NotNull(SmartHealthCardModel);


    }


  }
}
