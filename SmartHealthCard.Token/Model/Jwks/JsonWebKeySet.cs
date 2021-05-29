using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace SmartHealthCard.Token.Model.Jwks
{
  public class JsonWebKeySet
  {
    [JsonConstructor]
    public JsonWebKeySet(List<JsonWebKey> Keys)
    {
      this.Keys = Keys ?? throw new ArgumentNullException(nameof(Keys));      
    }

    public JsonWebKeySet()
    {
      this.Keys = new List<JsonWebKey>();
    }

    [JsonProperty("keys")]
    public List<JsonWebKey> Keys { get; set; }    
  }
}
