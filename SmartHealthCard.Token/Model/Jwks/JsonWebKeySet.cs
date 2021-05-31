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
      this.Keys = Keys;
    }

    public JsonWebKeySet()
    {
      this.Keys = new List<JsonWebKey>();
    }

    [JsonProperty("keys", Required = Required.Always)]
    public List<JsonWebKey> Keys { get; set; }    
  }
}
