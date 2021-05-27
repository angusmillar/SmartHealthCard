using SmartHealthCard.Token.Model.Jwks;
using System;

namespace SmartHealthCard.Token.Providers
{
  public interface IJwksProvider
  {
    JsonWebKeySet GetJwks(Uri WellKnownJwksUri);
  }
}