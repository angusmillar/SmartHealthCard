using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Providers
{
  public class HttpClient : IHttpClient
  {
    private readonly System.Net.Http.HttpClient Client;
    public HttpClient()
    {
      Client = new System.Net.Http.HttpClient();
    }

    public static HttpClient Create()
    {
      return new HttpClient();
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage Request, CancellationToken? CancellationToken)
    {
      return Client.SendAsync(Request, HttpCompletionOption.ResponseHeadersRead, CancellationToken ?? new CancellationToken());
    }
  }
}
