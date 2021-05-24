using Newtonsoft.Json;

namespace SmartHealthCard.Token.Model.Jwks
{
  public class JsonWebKey
  {
    public JsonWebKey(string Kty, string Kid, string Use, string Alg, string Crv, string X, string Y)
    {
      this.Kty = Kty;
      this.Kid = Kid;
      this.Use = Use;
      this.Alg = Alg;
      this.Crv = Crv;
      this.X = X;
      this.Y = Y;
    }

    [JsonProperty("kty")]
    public string Kty { get; set; }
    [JsonProperty("kid")]
    public string Kid { get; set; }
    [JsonProperty("use")]
    public string Use { get; set; }
    [JsonProperty("alg")]
    public string Alg { get; set; }
    [JsonProperty("cry")]
    public string Crv { get; set; }
    [JsonProperty("x")]
    public string X { get; set; }
    [JsonProperty("y")]
    public string Y { get; set; }
  }
}
