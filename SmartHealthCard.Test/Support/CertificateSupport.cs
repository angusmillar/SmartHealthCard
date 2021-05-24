using SmartHealthCard.Token.Certificates;
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
  }
}
