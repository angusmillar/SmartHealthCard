using SmartHealthCard.Token.Model.Jwks;
using System;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Providers
{
  public interface IJwksProvider
  {
    Task<JsonWebKeySet> GetJwksAsync(Uri WellKnownJwksUri);
  }
}