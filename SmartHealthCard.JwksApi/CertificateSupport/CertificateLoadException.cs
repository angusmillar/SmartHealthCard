using System;

namespace SmartHealthCard.JwksApi.CertificateSupport
{
  public class CertificateLoadException : Exception
  {
    public CertificateLoadException(string? message) : base(message)
    {
    }
  }
}
