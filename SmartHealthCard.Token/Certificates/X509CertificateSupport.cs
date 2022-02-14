using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Algorithms;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SmartHealthCard.Token.Exceptions;

namespace SmartHealthCard.Token.Certificates
{
  public static class X509CertificateSupport
  {
    public static X509Certificate2 GetFirstMatchingCertificate(string FindValue,
          X509FindType FindType, StoreName StoreName,
          StoreLocation StoreLocation, bool Valid)
    {
      X509Store certStore = new(StoreName, StoreLocation);
      certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      X509Certificate2Collection foundCerts = certStore.Certificates.Find(FindType, FindValue, Valid);
      certStore.Close();
      if (foundCerts.Count == 0)
      {
        throw new Exception($"Unable to locate a certificate for the find value of {FindValue} of type {FindType} in the store location of {StoreLocation} and store name of {StoreName} with a Valid status of {Valid}.");
      }
      return foundCerts[0];
    }
  }
}

