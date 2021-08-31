using SmartHealthCard.Token.Model.Jwks;
using System;

namespace SmartHealthCard.Token.Providers
{
  public class JwksCacheItem
  {
    public JwksCacheItem(Uri wellKnownUrl, JsonWebKeySet jsonWebKeySet, DateTime obtainedDate)
    {
      WellKnownUrl = wellKnownUrl;
      JsonWebKeySet = jsonWebKeySet;
      ObtainedDate = obtainedDate;
    }

    public Uri WellKnownUrl { get; set; }
    public JsonWebKeySet JsonWebKeySet { get; set; }
    public DateTime ObtainedDate { get; set; }
  }
}
