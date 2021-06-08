using System;

namespace SmartHealthCard.JwksEndpoint.CertificateSupport
{
  public class CertificateLoadException : Exception
  {
    public CertificateLoadException(string? message) : base(message)
    {
    }
  }
}
