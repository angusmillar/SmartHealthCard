using Microsoft.Extensions.Options;
using SmartHealthCard.JwksEndpoint.CertificateSupport;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.JwksEndpoint.JwksSupport
{
  public class JwksJsonProvider : IJwksJsonProvider
  {
    private readonly IOptions<List<CertificateThumbprint>> CertificateThumprintList;
    private readonly List<X509Certificate2> CertificateList;
    private string? JwksJson;

    public JwksJsonProvider(IOptions<List<CertificateThumbprint>> CertificateThumprintList)
    {
      this.CertificateThumprintList = CertificateThumprintList;
      CertificateList = new List<X509Certificate2>();
    }

    public string GetJwksJson()
    {
      if (!string.IsNullOrWhiteSpace(JwksJson))
      {
        return JwksJson;
      }
      else
      {
        if (CertificateThumprintList.Value.Count == 0)
        {
          throw new CertificateLoadException($"Zero Thumb-prints were found in the services appsettings.json file's CertificateThumbprintList. There must be at least one Thumb-print listed.");
        }
        foreach (CertificateThumbprint CertificateThumbprint in CertificateThumprintList.Value)
        {
          if (CertificateThumbprint.Thumbprint is null)
            throw new ArgumentNullException(nameof(CertificateThumbprint.Thumbprint));

          CertificateList.Add(GetFirstMatchingCertificate(StoreName.My, StoreLocation.LocalMachine, CertificateThumbprint.Thumbprint));
          SmartHealthCardJwks SmartHealthCardJwks = new SmartHealthCardJwks();
          try
          {
            JwksJson = SmartHealthCardJwks.Get(this.CertificateList, Minified: false);
          }
          catch(SmartHealthCardJwksException SmartHealthCardJwksException)
          {
            throw new CertificateLoadException(SmartHealthCardJwksException.Message);
          }          
        }
        if (JwksJson is null)
        {
          throw new ApplicationException($"Unexpected error {typeof(SmartHealthCardJwks).FullName} returned an empty string rather than a JSON Web Key Set (JWKS)");
        }
        else
        {
          return JwksJson;
        }
      }
    }

    private X509Certificate2 GetFirstMatchingCertificate(StoreName StoreName, StoreLocation StoreLocation, string Thumbprint)
    {
      bool IsValidCertificate = true;
      X509Store certStore = new X509Store(StoreName, StoreLocation);
      certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      X509Certificate2Collection foundCerts = certStore.Certificates.Find(X509FindType.FindByThumbprint, Thumbprint.ToUpper(), IsValidCertificate);
      certStore.Close();
      if (foundCerts.Count == 0)
      {
        throw new CertificateLoadException($"Unable to locate a certificate for the find value of {Thumbprint.ToUpper()} of type {X509FindType.FindByThumbprint} in the store location of {StoreLocation} and store name of {StoreName} with a Valid status of {IsValidCertificate}.");
      }
      return foundCerts[0];
    }
  }
}
