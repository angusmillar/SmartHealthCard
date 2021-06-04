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
    
    public async Task<Result<byte[]>> SerializeAsync<T>(T Obj, bool Minified = true)
    {
      if (Obj is SmartHealthCareJWSHeaderModel SmartHealthCareJWSHeaderModel)
      {
        Result<string> ToJsonResult =  ToJson(SmartHealthCareJWSHeaderModel, Minified);
        if (ToJsonResult.Failure)
          return Result<byte[]>.Fail($"{ToJsonResult.ErrorMessage}");
        
        return await Task.Run(() => Result<byte[]>.Ok(Encoders.Utf8EncodingSupport.GetBytes(ToJsonResult.Value)));        
      }
      else
      {
        throw new InvalidCastException($"The {this.GetType().Name} Serialize method can only work with an input of type {typeof(SmartHealthCareJWSHeaderModel).Name}");
      }
    }

    public async Task<Result<T>> DeserializeAsync<T>(byte[] bytes)
    {
      string json = Encoders.Utf8EncodingSupport.GetString(bytes);
      if (typeof(T) == typeof(SmartHealthCareJWSHeaderModel))
      {
        return await Task.Run(() => FromJson<T>(json));       
      }
      else
      {
        throw new InvalidCastException(typeof(T).Name);
      }
    }
    
    public Result<string> ToJson<T>(T Obj, bool Minified = true) => JsonSerializer.ToJson(Obj);

    public Result<T> FromJson<T>(string Json) => JsonSerializer.FromJson<T>(Json);

    public Result<T> FromJsonStream<T>(Stream JsonStream)
    {
      return JsonSerializer.FromJsonStream<T>(JsonStream);
    }
  }
}
