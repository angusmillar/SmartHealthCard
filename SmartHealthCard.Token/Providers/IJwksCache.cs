using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Support;
using System;

namespace SmartHealthCard.Token.Providers
{
  public interface IJwksCache
  {
    Result<JsonWebKeySet> Get(Uri WellKnownUrl);
    void Set(Uri WellKnownUrl, JsonWebKeySet JsonWebKeySet);
  }
}