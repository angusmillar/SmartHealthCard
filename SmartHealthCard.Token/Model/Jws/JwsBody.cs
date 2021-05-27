using Newtonsoft.Json;

namespace SmartHealthCard.Token.Model.Jws
{
  /// <summary>
  /// See JSON Web Token (JWT) Standard section 4.1.  Registered Claim Names
  /// See: https://datatracker.ietf.org/doc/html/rfc7519#section-4.1
  /// </summary>
  public class JwsBody
  {
    [JsonProperty("iss", Required = Required.Default)]
    public string? Iss { get; set; }

    [JsonProperty("sub", Required = Required.Default)]
    public string? Sub { get; set; }

    [JsonProperty("aud", Required = Required.Default)]
    public string? Aud { get; set; }

    [JsonProperty("exp", Required = Required.Default)]
    public string? Exp { get; set; }

    [JsonProperty("nbf", Required = Required.Default)]
    public string? Nbf { get; set; }

    [JsonProperty("iat", Required = Required.Default)]
    public string? Iat { get; set; }

    [JsonProperty("jti", Required = Required.Default)]
    public string? Jti { get; set; }
  }
}
