using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Serializers.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Providers
{
  public class JwksProvider : IJwksProvider
  {
    private readonly IJsonSerializer JsonSerializer;
    public JwksProvider(IJsonSerializer JsonSerializer)
    {
      this.JsonSerializer = JsonSerializer;
    }

    public async Task<JsonWebKeySet> GetJwksAsync(Uri WellKnownJwksUri)
    {
      var HttpClient = new HttpClient();
      var JwksJson = await HttpClient.GetStringAsync(WellKnownJwksUri);     
      return JsonSerializer.FromJson<JsonWebKeySet>(JwksJson);
    }
  }
}
