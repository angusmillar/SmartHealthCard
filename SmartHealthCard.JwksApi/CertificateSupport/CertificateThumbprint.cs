namespace SmartHealthCard.JwksApi.CertificateSupport
{
  /// <summary>
  /// Used to read Certificate Thumb-print values from the appsettings.json file for the servive
  /// </summary>
  public class CertificateThumbprint
  {
    /// <summary>
    /// A certificates Thumb-print used to locate the certificate in the windows certificate store.
    /// </summary>
    public string? Thumbprint { get; set; }
  }
  
}
