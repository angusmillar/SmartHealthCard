using Microsoft.Extensions.Options;
using SmartHealthCard.JwksApi.CertificateSupport;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Exceptions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SmartHealthCard.JwksApi.JwksSupport
{
  public class JwksJsonProvider : IJwksJsonProvider
  {
    private readonly IOptions<List<CertificateThumbprint>> CertificateThumprintList;       
    private string? JwksJson;

    public JwksJsonProvider(IOptions<List<CertificateThumbprint>> CertificateThumprintList)
    {
      this.CertificateThumprintList = CertificateThumprintList;      
    }

    public string GetJwksJson(bool FromPEMFile = false)
    {
      if (!string.IsNullOrWhiteSpace(JwksJson))
      {
        return JwksJson;
      }
      else if (FromPEMFile)
      {
        List<X509Certificate2> CertificateList = new();
        CertificateList.Add(GetSingleCertificateFromPemFiles());
        this.JwksJson = GetJwksString(CertificateList);               
      }
      else
      {
        if (CertificateThumprintList.Value.Count == 0)
        {
          throw new CertificateLoadException($"Zero Thumb-prints were found in the services appsettings.json file's CertificateThumbprintList. There must be at least one Thumb-print listed.");
        }
        List<X509Certificate2> CertificateList = new();
        foreach (CertificateThumbprint CertificateThumbprint in CertificateThumprintList.Value)
        {
          if (CertificateThumbprint.Thumbprint is null)
          {
            throw new ArgumentNullException("CertificateThumbprintList.Thumbprint", $"CertificateThumbprintList.Thumbprint value found to be null in appsettings.json");            
          }
          CertificateList.Add(GetFirstMatchingCertificateFromWindowsCertificateStore(StoreName.My, StoreLocation.LocalMachine, CertificateThumbprint.Thumbprint));
        }
        this.JwksJson = GetJwksString(CertificateList);
      }
      if (JwksJson is not null)
      {
        return JwksJson;
      }
      else
      {
        throw new ApplicationException($"Unexpected error {typeof(SmartHealthCardJwks).FullName} returned an empty string rather than a JSON Web Key Set (JWKS)");        
      }
    }

    private static string GetJwksString(List<X509Certificate2> CertificateList)
    {
      SmartHealthCardJwks SmartHealthCardJwks = new();
      try
      {
        return SmartHealthCardJwks.Get(CertificateList, Minified: false);
      }
      catch (SmartHealthCardJwksException SmartHealthCardJwksException)
      {
        throw new CertificateLoadException(SmartHealthCardJwksException.Message);
      }
    }

    private static X509Certificate2 GetFirstMatchingCertificateFromWindowsCertificateStore(StoreName StoreName, StoreLocation StoreLocation, string Thumbprint)
    {
      bool IsValidCertificate = true;
      X509Store certStore = new(StoreName, StoreLocation);
      certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      X509Certificate2Collection foundCerts = certStore.Certificates.Find(X509FindType.FindByThumbprint, Thumbprint.ToUpper(), IsValidCertificate);
      certStore.Close();
      if (foundCerts.Count == 0)
      {
        throw new CertificateLoadException($"Unable to locate a certificate for the find value of {Thumbprint.ToUpper()} of type {X509FindType.FindByThumbprint} in the store location of {StoreLocation} and store name of {StoreName} with a Valid status of {IsValidCertificate}.");
      }
      return foundCerts[0];
    }

    private static X509Certificate2 GetSingleCertificateFromPemFiles()
    {
      UTF8Encoding UTF8Encoding = new(encoderShouldEmitUTF8Identifier: false);
      string CertificatePEM = UTF8Encoding.GetString(Resource.TestECC256Cert);
      string PrivateKeyPEM = UTF8Encoding.GetString(Resource.TestECC256Private_key);
      return X509Certificate2.CreateFromPem(CertificatePEM, PrivateKeyPEM);
    }
  }
}
