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
  public class JwksProviderHttpClient
  {
    private readonly HttpClient HttpClient;
    private readonly IJsonSerializer JsonSerializer;

    public JwksProviderHttpClient(IJsonSerializer JsonSerializer,  HttpClient HttpClient)
    {
      this.HttpClient = HttpClient;
      this.JsonSerializer = JsonSerializer;
    }


    public async Task<Result<JsonWebKeySet>> Get(Uri WellKnownUrl, CancellationToken? CancellationToken = null)
    {
      var request = new HttpRequestMessage(HttpMethod.Get, WellKnownUrl);
      try
      {
        using (var response = await HttpClient.SendAsync(request, CancellationToken ?? new CancellationToken()))
        {
          if (response.StatusCode == HttpStatusCode.OK)
          {
            if (response.Content == null)
            {
              return Result<JsonWebKeySet>.Fail("Response content was null");
            }

            System.IO.Stream ResponseStream = await response.Content.ReadAsStreamAsync();
            //var responseJson = await response.Content.ReadAsStringAsync();
            Result<JsonWebKeySet> JsonWebKeySetResult = JsonSerializer.FromJsonStream<JsonWebKeySet>(ResponseStream);

            if (JsonWebKeySetResult.Success)
            {
              return JsonWebKeySetResult;
            }
            else
            {
              
              
              return Result<JsonWebKeySet>.Fail($"Failed to deserialize response: {JsonWebKeySetResult.ErrorMessage}");
            }            
          }
          else
          {
            string Message = string.Empty;
            if (response.Content != null)
            {
              var ErrorResponseContent = await response.Content.ReadAsStringAsync();
              return Result<JsonWebKeySet>.Fail($"Response status: {response.StatusCode}, Content: {ErrorResponseContent}");             
            }
            else
            {
              return Result<JsonWebKeySet>.Fail($"Response status: {response.StatusCode}, Content: [None]");
            }           
          }
        }
        
      }
      catch(HttpRequestException HttpRequestException)
      {
        //_logger.LogError(Exec, "HttpRequestException when calling the API");
        return Result<JsonWebKeySet>.Retry($"HttpRequestException when calling the API: {HttpRequestException.Message}");
      }
      catch (TimeoutException)
      {
        //_logger.LogError(exception, "TimeoutException during call to API");
        return Result<JsonWebKeySet>.Retry("TimeoutException during call to API");
      }
      catch (OperationCanceledException)
      {
        //_logger.LogError(exception, "Task was canceled during call to API");
        return Result<JsonWebKeySet>.Retry("Task was canceled during call to API");
      }
      catch (Exception)
      {
        //_logger.LogError(exception, "Unhanded exception when calling the API");
        throw;
      }
    }
  }
}
