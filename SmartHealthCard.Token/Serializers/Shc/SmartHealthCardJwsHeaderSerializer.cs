using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using System;

namespace SmartHealthCard.Token.Serializers.Shc
{
  public class SmartHealthCardJwsHeaderSerializer :  IJwsHeaderSerializer, IJsonSerializer//, IJwsSerializer//, IJsonSerializer
  {    
    private readonly IJsonSerializer JsonSerializer;
    public SmartHealthCardJwsHeaderSerializer(IJsonSerializer JsonSerializer)
    {
      this.JsonSerializer = JsonSerializer;
    }
    
    public byte[] Serialize<T>(T Obj, bool Minified = true)
    {
      if (Obj is SmartHealthCareJWSHeaderModel SmartHealthCareJWSHeaderModel)
      {
        string Json =  ToJson(SmartHealthCareJWSHeaderModel, Minified);
        return Encoders.Utf8EncodingSupport.GetBytes(Json);        
      }
      else
      {
        throw new ArgumentException($"The {this.GetType().Name} Serialize method can only work with an input of type {typeof(SmartHealthCareJWSHeaderModel).Name}");
      }
    }

    public T Deserialize<T>(byte[] bytes)
    {
      string json = Encoders.Utf8EncodingSupport.GetString(bytes);
      if (typeof(T) == typeof(SmartHealthCareJWSHeaderModel))
      {
        return (T)(object)FromJson<SmartHealthCareJWSHeaderModel>(json);
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
