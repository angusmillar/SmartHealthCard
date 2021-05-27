using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartHealthCard.Token.Model.Jwks
{
  public class JsonWebKeySet
  {
    [JsonConstructor]
    public JsonWebKeySet(List<JsonWebKey> Keys)
    {
      this.Keys = Keys;      
    }

    public JsonWebKeySet()
    {
      this.Keys = new List<JsonWebKey>();
    }

    [JsonProperty("keys")]
    public List<JsonWebKey> Keys { get; set; }    
  }
}
