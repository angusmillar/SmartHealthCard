using Newtonsoft.Json;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Jws;
using System;
using System.IO;
using System.Text;
using static SmartHealthCard.Token.Encoders.Utf8EncodingSupport;

namespace SmartHealthCard.Token.Serializers.Shc
{
  public class SmartHealthCardJwsHeaderSerializer : IJwsHeaderSerializer
  {    
    private readonly Newtonsoft.Json.JsonSerializer Serializer;
    public SmartHealthCardJwsHeaderSerializer()
    {
      this.Serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
    }
    
    public byte[] Serialize<T>(T Obj)
    {
      if (Obj is SmartHealthCareJWSHeaderModel SmartHealthCareJWSHeaderModel)
      {
        Serializer.Formatting = Formatting.Indented;
        var sb = new StringBuilder();
        using var stringWriter = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(stringWriter);
        Serializer.Serialize(jsonWriter, SmartHealthCareJWSHeaderModel);
        string SmartHealthCardHeaderJson = sb.ToString();       
        return GetBytes(SmartHealthCardHeaderJson);
      }
      else
      {
        throw new ArgumentException($"The {this.GetType().Name} Serialize method can only work with an input of type {typeof(SmartHealthCareJWSHeaderModel).Name}");
      }
    }

    public T Deserialize<T>(byte[] bytes)
    {
      string json = GetString(bytes);
      using var stringReader = new StringReader(json);
      using var jsonReader = new JsonTextReader(stringReader);
      var SmartHealthCareJWSHeaderModel = Serializer.Deserialize<SmartHealthCareJWSHeaderModel>(jsonReader);
      if (SmartHealthCareJWSHeaderModel == null)
        throw new DeserializationException($"Unable to deserialize the JWS Header to type {typeof(SmartHealthCareJWSHeaderModel).Name}");
      return (T)(object)SmartHealthCareJWSHeaderModel;      
    }
  }
}
