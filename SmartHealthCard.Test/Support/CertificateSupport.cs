using SmartHealthCard.Token.Certificates;
using SmartHealthCard.Token.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Test.Support
{
  public static class CertificateSupport
  {
    public static X509Certificate2 GetCertificate(string Thumbprint)
    {
      //Loads the Certificate from the Windows Certificate store 
      return X509CertificateSupport.GetFirstMatchingCertificate(
          Thumbprint.ToUpper(),
          X509FindType.FindByThumbprint,
          StoreName.My,
          StoreLocation.LocalMachine,
          true
          );
    }

    public static X509Certificate2 GetCertificateFromPemFiles()
    {
      string CertificatePEM = Utf8EncodingSupport.GetString(ResourceData.TestECC256Cert);
      string PrivateKeyPEM = Utf8EncodingSupport.GetString(ResourceData.TestECC256Private_key);
      return X509Certificate2.CreateFromPem(CertificatePEM, PrivateKeyPEM);
    }

    

  }
}
