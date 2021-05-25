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

    public JsonSerializer(bool Minified = true)
    {
      this.Serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
      if (!Minified)       
        Serializer.Formatting = Formatting.Indented;
    }

    public byte[] Serialize<T>(T Obj)
    {
      return GetBytes(this.ToJson(Obj));     
    }
    
    public T Deserialize<T>(byte[] bytes)
    {
      T? Item = this.FromJson<T>(GetString(bytes));
      if (Item is null)
        throw new DeserializationException($"Unable to deserialize the JWS Header to type {typeof(T).Name}");
      return Item;      
    }

    public string ToJson<T>(T Obj)
    {
      var Builder = new StringBuilder();
      using var StringWriter = new StringWriter(Builder);
      using var JsonWriter = new  JsonTextWriter(StringWriter);
      Serializer.Serialize(JsonWriter, Obj);
      return Builder.ToString();
    }

    public T FromJson<T>(string Json)
    {      
      using var StringReader = new StringReader(Json);
      using var JsonReader = new JsonTextReader(StringReader);
      T? Item = Serializer.Deserialize<T>(JsonReader);
      if (Item is null)
        throw new DeserializationException($"Unable to deserialize the JWS Header to type {typeof(T).Name}");
      return Item;
    }
  }
}
