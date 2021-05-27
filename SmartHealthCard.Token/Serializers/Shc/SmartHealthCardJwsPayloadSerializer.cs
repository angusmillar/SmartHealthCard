using SmartHealthCard.Token.Compression;
using SmartHealthCard.Token.Model.Jws;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using System;

namespace SmartHealthCard.Token.Serializers.Shc
{
  public sealed class SmartHealthCardJwsPayloadSerializer : IJwsPayloadSerializer, IJsonSerializer//, IJwsSerializer, IJsonSerializer
  {
    private readonly IJsonSerializer JsonSerializer;
    public SmartHealthCardJwsPayloadSerializer(IJsonSerializer JsonSerializer)
    {
      this.JsonSerializer = JsonSerializer;
    }

    public byte[] Serialize<T>(T Obj, bool Minified = true)
    {
      if (!Minified)
        throw new ArgumentException($"{nameof(Minified)} must be true for Smart Health Card Jws Payload JSON.");

      if (Obj is SmartHealthCardModel SmartHealthCardModel)
      {
        return DeflateCompression.Compress(ToJson(SmartHealthCardModel, true));        
      }
      else
      {
        throw new ArgumentException($"The {this.GetType().Name} can only work with an input of type {typeof(SmartHealthCardModel).Name}");
      }
    }

    public T Deserialize<T>(byte[] bytes)
    {
      string MinifiedSmartHealthCardJson = DeflateCompression.Uncompress(bytes);
      if (typeof(T) == typeof(SmartHealthCardModel))
      {
        SmartHealthCardModel SmartHealthCardModel = FromJson<SmartHealthCardModel>(MinifiedSmartHealthCardJson);        
        return (T)(object)SmartHealthCardModel;
      }
      else if (typeof(T) == typeof(JwsBody))
      {
        return (T)(object)FromJson<JwsBody>(MinifiedSmartHealthCardJson);
      }
      else
      {
        throw new TypeAccessException(typeof(T).Name);
      }

    }

    public string ToJson<T>(T Obj, bool Minified = true) => JsonSerializer.ToJson(Obj);

    public T FromJson<T>(string Json) => JsonSerializer.FromJson<T>(Json);

  }
}
