using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartHealthCard.Token.Model.Jwks
{
  public class JsonWebKeySet
  {
    public JsonWebKeySet(List<JsonWebKey> Keys)
    {
      this.Keys = Keys;      
    }

    [JsonProperty("keys")]
    public List<JsonWebKey> Keys { get; set; }
    
  }
}
