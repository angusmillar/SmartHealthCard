using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Providers
{
  public class JwksProvider : IJwksProvider
  {
    private readonly IHttpClient HttpClient;    
    private readonly IJsonSerializer JsonSerializer;
    private readonly IJwksCache JwksCache;
    private readonly TimeSpan? MaxCacheLife;
    
    public JwksProvider()
     : this(null, null, null) { }

    public JwksProvider(IHttpClient HttpClient) 
      :this(HttpClient, null, null) { }

    public JwksProvider(IHttpClient HttpClient, TimeSpan MaxCacheLife)
     : this(HttpClient, null, MaxCacheLife) { }

    public JwksProvider(IHttpClient HttpClient, IJsonSerializer JsonSerializer)
      : this(HttpClient, JsonSerializer, null) { }

    public JwksProvider(IJsonSerializer JsonSerializer) 
      : this(null, JsonSerializer, null) { }

    public JwksProvider(TimeSpan MaxCacheLife)
      : this(null, null, MaxCacheLife) { }

    public JwksProvider(IHttpClient? HttpClient, IJsonSerializer? JsonSerializer, TimeSpan? MaxCacheLife) 
    {
      this.HttpClient = this.HttpClient ?? Providers.HttpClient.Create();
      this.JsonSerializer = this.JsonSerializer ?? new JsonSerializer();
      this.MaxCacheLife = this.MaxCacheLife ?? new TimeSpan(0, 10, 0); //10 min Cache expiry
      this.JwksCache = new JwksCache(MaxCacheLife);
    }

    public async Task<Result<JsonWebKeySet>> GetJwksAsync(Uri WellKnownUrl, CancellationToken? CancellationToken = null)
    {
      //Check the Cache for the target
      Result<JsonWebKeySet> CacheResult = JwksCache.Get(WellKnownUrl);
      if (CacheResult.Success)
        return CacheResult;

      var request = new HttpRequestMessage(HttpMethod.Get, WellKnownUrl);      
      try
      {
        using (HttpResponseMessage Response = await HttpClient.SendAsync(request, CancellationToken ?? new CancellationToken()))
        {
          if (Response.StatusCode == HttpStatusCode.OK)
          {
            if (Response.Content == null)
            {
              return Result<JsonWebKeySet>.Fail("HttpClient response content was null");
            }
            using System.IO.Stream ResponseStream = await Response.Content.ReadAsStreamAsync();            
            Result<JsonWebKeySet> JsonWebKeySetResult = JsonSerializer.FromJsonStream<JsonWebKeySet>(ResponseStream);
            if (JsonWebKeySetResult.Failure)
               return Result<JsonWebKeySet>.Fail($"Failed to deserialize the JsonWebKeySet (JWKS) which was returned from the endpoint {WellKnownUrl.OriginalString}. {JsonWebKeySetResult.ErrorMessage}");
           
            //Store the JsonWebKeySet in the cache 
            JwksCache.Set(WellKnownUrl, JsonWebKeySetResult.Value);            
            return JsonWebKeySetResult;                        
          }
          else
          {
            string Message = string.Empty;
            if (Response.Content != null)
            {
              var ErrorResponseContent = await Response.Content.ReadAsStringAsync();
              return Result<JsonWebKeySet>.Fail($"Response status: {Response.StatusCode}, Content: {ErrorResponseContent}");             
            }
            else
            {
              return Result<JsonWebKeySet>.Fail($"Response status: {Response.StatusCode}, Content: [None]");
            }           
          }
        }        
      }
      catch(HttpRequestException HttpRequestException)
      {        
        return Result<JsonWebKeySet>.Retry($"HttpRequestException when calling the API: {HttpRequestException.Message}");
      }
      catch (TimeoutException)
      {       
        return Result<JsonWebKeySet>.Retry("TimeoutException during call to API");
      }
      catch (OperationCanceledException)
      {        
        return Result<JsonWebKeySet>.Fail("Task was canceled during call to API");
      }      
    }
  }
}
