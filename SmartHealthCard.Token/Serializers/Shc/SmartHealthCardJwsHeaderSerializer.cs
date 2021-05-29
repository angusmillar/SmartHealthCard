using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Support;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Serializers.Shc
{
  public class SmartHealthCardJwsHeaderSerializer :  IJwsHeaderSerializer, IJsonSerializer//, IJwsSerializer//, IJsonSerializer
  {    
    private readonly IJsonSerializer JsonSerializer;
    public SmartHealthCardJwsHeaderSerializer(IJsonSerializer JsonSerializer)
    {
      this.JsonSerializer = JsonSerializer;
    }
    
    public async Task<byte[]> SerializeAsync<T>(T Obj, bool Minified = true)
    {
      if (Obj is SmartHealthCareJWSHeaderModel SmartHealthCareJWSHeaderModel)
      {
        string Json =  ToJson(SmartHealthCareJWSHeaderModel, Minified);
        return await Task.Run(() => Encoders.Utf8EncodingSupport.GetBytes(Json));        
      }
      else
      {
        throw new ArgumentException($"The {this.GetType().Name} Serialize method can only work with an input of type {typeof(SmartHealthCareJWSHeaderModel).Name}");
      }
    }

    public async Task<T> DeserializeAsync<T>(byte[] bytes)
    {
      string json = Encoders.Utf8EncodingSupport.GetString(bytes);
      if (typeof(T) == typeof(SmartHealthCareJWSHeaderModel))
      {
        return await Task.Run(() => (T)(object)FromJson<SmartHealthCareJWSHeaderModel>(json));
      }       
      else
      {
        throw new TypeAccessException(typeof(T).Name);
      }

    }

    public string ToJson<T>(T Obj, bool Minified = true) => JsonSerializer.ToJson(Obj);

    public T FromJson<T>(string Json) => JsonSerializer.FromJson<T>(Json);

    public Result<T> FromJsonStream<T>(Stream JsonStream)
    {
      return JsonSerializer.FromJsonStream<T>(JsonStream);
    }
  }
}
