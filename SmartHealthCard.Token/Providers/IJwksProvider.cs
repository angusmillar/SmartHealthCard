using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Support;
using System;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Providers
{
  public interface IJwksProvider
  {
    Task<Result<JsonWebKeySet>> GetJwksAsync(Uri WellKnownJwksUri, System.Threading.CancellationToken? CancellationToken = null);
  }
}