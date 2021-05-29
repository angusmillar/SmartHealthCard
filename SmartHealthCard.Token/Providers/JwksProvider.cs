using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Support;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Providers
{
  public class JwksProvider : IJwksProvider
  {
    private readonly IJsonSerializer JsonSerializer;
    private readonly HttpClient HttpClient;
    public JwksProvider(IJsonSerializer JsonSerializer)
    {
      this.JsonSerializer = JsonSerializer;
      this.HttpClient = new HttpClient();
    }

    public async Task<Result<JsonWebKeySet>> GetJwksAsync(Uri WellKnownJwksUri, System.Threading.CancellationToken? CancellationToken)
    {
      JwksProviderHttpClient Client = new JwksProviderHttpClient(this.JsonSerializer, this.HttpClient);      
      return await Client.Get(WellKnownJwksUri, CancellationToken);      
    }
  }
}
