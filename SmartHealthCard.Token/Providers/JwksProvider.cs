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

    public JsonWebKeySet GetJwks(Uri WellKnownJwksUri)
    {
      var HttpClient = new HttpClient();
      var task = Task.Run(() => HttpClient.GetStringAsync(WellKnownJwksUri));
      task.Wait();
      string JsonWebKeySetJson = task.Result;
      return JsonSerializer.FromJson<JsonWebKeySet>(JsonWebKeySetJson);
    }
  }
}
