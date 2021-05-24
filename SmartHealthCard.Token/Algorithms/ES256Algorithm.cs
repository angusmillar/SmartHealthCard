using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Hashers;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token.Algorithms
{
  public sealed class ES256Algorithm : IAlgorithm 
  {
    private readonly X509Certificate2 Certificate;
    private readonly ECDsa PublicKey;
    private readonly ECDsa PrivateKey;

    public ES256Algorithm(X509Certificate2 Certificate)
    {
      this.Certificate = Certificate;
      ECDsa? PrivateKey = GetPrivateKey(this.Certificate);
      if (PrivateKey == null)
      {
        throw new NullReferenceException("Unable to obtain the private key from the provided Certificate.");
      }
      else
      {
        this.PrivateKey = PrivateKey;
      }

      ECDsa? PublicKey = GetPublicKey(this.Certificate) ?? throw new Exception("Certificate's PublicKey cannot be null.");
      if (PublicKey == null)
      {
        throw new NullReferenceException("Unable to obtain a public key from the provided Certificate.");
      }
      else
      {
        this.PublicKey = PublicKey;                
      }

    }

    public string Name => "ES256";
    public string KeyTypeName => "EC";
    public string CurveName => "P-256";
   
    private HashAlgorithmName HashAlgorithmName => HashAlgorithmName.SHA256;

    public byte[] Sign(byte[] bytesToSign)
    {
      return this.PrivateKey.SignData(bytesToSign, this.HashAlgorithmName);
    }
    public bool Verify(byte[] bytesToSign, byte[] signature)
    {
      return this.PublicKey.VerifyData(bytesToSign, signature, this.HashAlgorithmName);
    }
    public string GetKid()
    {
      return Base64UrlEncoder.Encode(this.Certificate.Thumbprint.GetSHA256Hash());
    }
    public string GetPointCoordinateX()
    {
      ECParameters ECParameters = this.PublicKey.ExportExplicitParameters(false);
      if (ECParameters.Q.X is null)
        throw new NullReferenceException(nameof(ECParameters.Q.X));

      return Base64UrlEncoder.Encode(ECParameters.Q.X);      
    }
    public string GetPointCoordinateY()
    {
      ECParameters ECParameters = this.PublicKey.ExportExplicitParameters(false);
      if (ECParameters.Q.Y is null)
        throw new NullReferenceException(nameof(ECParameters.Q.Y));

      return Base64UrlEncoder.Encode(ECParameters.Q.Y);
    }

    private static ECDsa? GetPrivateKey(X509Certificate2 cert)
    {
      if (cert is null)
        throw new ArgumentNullException(nameof(cert));

      return cert.GetECDsaPrivateKey();
    }

    private static ECDsa? GetPublicKey(X509Certificate2 cert)
    {
      if (cert is null)
        throw new ArgumentNullException(nameof(cert));

      return cert.GetECDsaPublicKey();
    }
  }
}
