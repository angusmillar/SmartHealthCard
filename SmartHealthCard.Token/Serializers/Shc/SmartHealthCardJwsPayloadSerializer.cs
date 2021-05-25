using SmartHealthCard.Token.Compression;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Jws;
using System;

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
      SmartHealthCardModel SmartHealthCardModel = SmartHealthCardModelJsonSerializer.FromJson(MinifiedSmartHealthCardJson);
      return (T)(object)SmartHealthCardModel;
    }   
  }
}
