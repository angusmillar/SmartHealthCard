using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using System;

namespace SmartHealthCard.Token.Serializers.Shc
{
  public class SmartHealthCardJwsHeaderSerializer : IJwsHeaderSerializer
  {    
    private readonly JsonSerializer Serializer;
    public SmartHealthCardJwsHeaderSerializer()
    {
      Serializer = new Json.JsonSerializer(Minified: true);
    }
    
    public byte[] Serialize<T>(T Obj)
    {
      if (Obj is SmartHealthCareJWSHeaderModel SmartHealthCareJWSHeaderModel)
      {
       
        return Serializer.Serialize(SmartHealthCareJWSHeaderModel);
      }
      else
      {
        throw new ArgumentException($"The {this.GetType().Name} Serialize method can only work with an input of type {typeof(SmartHealthCareJWSHeaderModel).Name}");
      }
    }

    public T Deserialize<T>(byte[] bytes)
    {      
      return (T)(object)Serializer.Deserialize<SmartHealthCareJWSHeaderModel>(bytes);      
    }
  }
}
