using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using SmartHealthCard.Token.Compression;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers;
using SmartHealthCard.Token.Serializers.Jws;

namespace SmartHealthCard.Token.Serializers.Shc
{ 
  public sealed class SmartHealthCardJwsPayloadSerializer : IJwsPayloadSerializer
  {
    private readonly Newtonsoft.Json.JsonSerializer Serializer;

    public SmartHealthCardJwsPayloadSerializer()
    {
      this.Serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
    }

    public byte[] Serialize<T>(T SmartHealthCard)
    {
      if (SmartHealthCard is SmartHealthCardModel SmartHealthCardModel)
      {       
        return DeflateCompression.Compress(SmartHealthCardModel.ToJson());
      }
      else
      {
        throw new ArgumentException($"The {this.GetType().Name} can only work with an input of type {typeof(SmartHealthCardModel).Name}");
      }
    }

    public T Deserialize<T>(byte[] bytes)
    {
      string MinifiedSmartHealthCardJson = DeflateCompression.Uncompress(bytes);
      JObject SmartHealthCardJObject = JObject.Parse(MinifiedSmartHealthCardJson);
      SmartHealthCardModel SmartHealthCardModel = SmartHealthCardModelJsonSerializer.FromJson(SmartHealthCardJObject.ToString());
      return (T)(object)SmartHealthCardModel;
    }
   
  }
}
