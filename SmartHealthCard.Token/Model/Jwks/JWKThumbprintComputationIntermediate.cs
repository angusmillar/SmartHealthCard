using Newtonsoft.Json;

namespace SmartHealthCard.Token.Model.Jwks
{
  public class JWKThumbprintComputationIntermediate
  {
    public JWKThumbprintComputationIntermediate(string Crv, string Kty, string X, string Y)
    {
      this.Crv = Crv;
      this.Kty = Kty;
      this.X = X;
      this.Y = Y;
    }

    [JsonProperty("crv")]
    public string Crv { get; set; }
    [JsonProperty("kty")]
    public string Kty { get; set; }      
    [JsonProperty("x")]
    public string X { get; set; }
    [JsonProperty("y")]
    public string Y { get; set; }
  }
}
