using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Jwks;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SmartHealthCard.Token.Algorithms
{
  public sealed class ES256Algorithm : IAlgorithm 
  {
    private readonly X509Certificate2? Certificate;
    private readonly ECDsa? PublicKey;
    private readonly ECDsa? PrivateKey;

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

    public ES256Algorithm(ECDsa? PublicKey, ECDsa? PrivateKey)
    {                 
       this.PublicKey = PublicKey;
       this.PrivateKey = PrivateKey;
    }

    public string Name => "ES256";
    public string KeyTypeName => "EC";
    public string CurveName => "P-256";
   
    private HashAlgorithmName HashAlgorithmName => HashAlgorithmName.SHA256;

    public byte[] Sign(byte[] bytesToSign)
    {
      if (this.PrivateKey is null)
        throw new SignatureSigningException("Unable to sign as no private key has been provided.");

      return this.PrivateKey.SignData(bytesToSign, this.HashAlgorithmName);
    }
    public bool Verify(byte[] bytesToSign, byte[] signature)
    {
      if (this.PublicKey is null)
        throw new SignatureSigningException("Unable to verify signature as no public key has been provided.");

      return this.PublicKey.VerifyData(bytesToSign, signature, this.HashAlgorithmName);
    }
    
    public string GetPointCoordinateX()
    {
      if (this.PublicKey is null)
        throw new SignatureSigningException("Unable to obtain Point Coordinate X from public key as no public key has been provided.");

      ECParameters ECParameters = this.PublicKey.ExportExplicitParameters(false);
      if (ECParameters.Q.X is null)
        throw new NullReferenceException(nameof(ECParameters.Q.X));

      return Base64UrlEncoder.Encode(ECParameters.Q.X);      
    }
    public string GetPointCoordinateY()
    {
      if (this.PublicKey is null)
        throw new SignatureSigningException("Unable to obtain Point Coordinate Y from public key as no public key has been provided.");

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

    public static ES256Algorithm FromJWKS(string Kid, JsonWebKeySet JsonWebKeySet)
    {
      JsonWebKey? Key = JsonWebKeySet.Keys.SingleOrDefault(x => x.Kid.Equals(Kid, StringComparison.CurrentCultureIgnoreCase));
      if (Key is null)
        throw new JsonWebKeySetException($"Unable to loacate the required key for kid value of {Kid}");

      ECDsa PublicKey = ECDsa.Create(new ECParameters
      {
        Curve = ECCurve.NamedCurves.nistP256,
        Q = new ECPoint
        {
          X = Base64UrlEncoder.Decode(Key.X),
          Y = Base64UrlEncoder.Decode(Key.Y)
        }
      });

      return new ES256Algorithm(PublicKey, null);
    }

    public string GetKid()
    {
      if (this.Certificate is null)
        throw new NullReferenceException("Unable to get certificate thumbprint as no certificate provided.");

      var Intermediate = new JWKThumbprintComputationIntermediate(
       this.CurveName,
       this.KeyTypeName,
       this.GetPointCoordinateX(),
       this.GetPointCoordinateY());

      var JsonSerializer = new Serializers.Json.JsonSerializer(Minified: true);
      string Json = JsonSerializer.ToJson(Intermediate);
      byte[] Bytes = Hashers.SHA256Hasher.GetSHA256Hash(Json);
      return Encoders.Base64UrlEncoder.Encode(Bytes);
    }
  }
}
