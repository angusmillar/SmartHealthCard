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
      T? Item = this.FromJson<T>(Utf8EncodingSupport.GetString(bytes));
      if (Item is null)
        throw new DeserializationException($"Unable to deserialize the JWS Header to type {typeof(T).Name}");
      return Item;      
    }

    public string ToJson<T>(T Obj)
    {
      var sb = new StringBuilder();
      using var stringWriter = new StringWriter(sb);
      using var jsonWriter = new JsonTextWriter(stringWriter);
      Serializer.Serialize(jsonWriter, Obj);
      return sb.ToString();
    }

    public T FromJson<T>(string Json)
    {      
      using var stringReader = new StringReader(Json);
      using var jsonReader = new JsonTextReader(stringReader);
      T? Item = Serializer.Deserialize<T>(jsonReader);
      if (Item is null)
        throw new DeserializationException($"Unable to deserialize the JWS Header to type {typeof(T).Name}");
      return Item;
    }
  }
}
