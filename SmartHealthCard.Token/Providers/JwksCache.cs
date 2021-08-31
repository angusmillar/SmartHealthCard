using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Providers
{
  public class JwksCache : IJwksCache
  {
    private readonly Dictionary<Uri, JwksCacheItem> JsonWebKeySetDictionaryCache;
    private readonly TimeSpan MaxCacheLife;

    public JwksCache(TimeSpan? MaxCacheLife)
    {
      JsonWebKeySetDictionaryCache = new Dictionary<Uri, JwksCacheItem>();
      this.MaxCacheLife = MaxCacheLife ?? new TimeSpan(0, 10, 0);
    }

    public void Set(Uri WellKnownUrl, JsonWebKeySet JsonWebKeySet)
    {
      var Item = new JwksCacheItem(WellKnownUrl, JsonWebKeySet, DateTime.Now); ;
      if (JsonWebKeySetDictionaryCache.ContainsKey(WellKnownUrl))
      {
        JsonWebKeySetDictionaryCache[WellKnownUrl] = Item;
      }
      else
      {
        JsonWebKeySetDictionaryCache.Add(WellKnownUrl, Item);
      }
    }
    public Result<JsonWebKeySet> Get(Uri WellKnownUrl)
    {
      //Check the JsonWebKeySet cache for the target
      DateTime now = DateTime.Now;
      if (JsonWebKeySetDictionaryCache.ContainsKey(WellKnownUrl))
      {
        JwksCacheItem JwksCacheItem = JsonWebKeySetDictionaryCache[WellKnownUrl];
        if (DateTime.Now.Subtract(JwksCacheItem.ObtainedDate) > MaxCacheLife)
        {
          //Expire this JsonWebKeySet and go get it again 
          JsonWebKeySetDictionaryCache.Remove(WellKnownUrl);
          return Result<JsonWebKeySet>.Fail("Cache value stale");
        }
        else
        {
          return Result<JsonWebKeySet>.Ok(JwksCacheItem.JsonWebKeySet);
        }
      }
      return Result<JsonWebKeySet>.Fail("Not found in cache");
    }
  }
}
