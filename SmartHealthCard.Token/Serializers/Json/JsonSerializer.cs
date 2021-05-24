using Newtonsoft.Json;
using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Serializers.Jws;
using System.IO;
using System.Text;
using static SmartHealthCard.Token.Encoders.Utf8EncodingSupport;

namespace SmartHealthCard.Token.Serializers.Json
{
  public sealed class JsonSerializer : IJwsHeaderSerializer, IJwsPayloadSerializer
  {
    private readonly Newtonsoft.Json.JsonSerializer Serializer;

    public JsonSerializer()
    {
      this.Serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();      
    }
    
    public byte[] Serialize<T>(T Obj)
    {
      var sb = new StringBuilder();
      using var stringWriter = new StringWriter(sb);
      using var jsonWriter = new JsonTextWriter(stringWriter);
      Serializer.Serialize(jsonWriter, Obj);
      return GetBytes(sb.ToString());
    }
    
    public T Deserialize<T>(byte[] bytes)
    {
      string json = Utf8EncodingSupport.GetString(bytes);
      using var stringReader = new StringReader(json);
      using var jsonReader = new JsonTextReader(stringReader);
      T? Item = Serializer.Deserialize<T>(jsonReader);
      if (Item is null)             
          throw new DeserializationException($"Unable to deserialize the JWS Header to type {typeof(T).Name}");
      return Item;
    }
  }
}
