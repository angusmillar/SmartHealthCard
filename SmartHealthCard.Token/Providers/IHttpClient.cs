using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Providers
{
  public interface IHttpClient
  {
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage Request, CancellationToken? CancellationToken);
  }
}
