using SmartHealthCard.Token.Compression;
using SmartHealthCard.Token.Model.Jws;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Support;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Serializers.Shc
{
  public sealed class SmartHealthCardJwsPayloadSerializer : IJwsPayloadSerializer, IJsonSerializer//, IJwsSerializer, IJsonSerializer
  {
    private readonly IJsonSerializer JsonSerializer;
    public SmartHealthCardJwsPayloadSerializer(IJsonSerializer JsonSerializer)
    {
      this.JsonSerializer = JsonSerializer;
    }

    public async Task<Result<byte[]>> SerializeAsync<T>(T Obj, bool Minified = true)
    {
      if (!Minified)
        throw new ArgumentException($"{nameof(Minified)} must be true for Smart Health Card JWS Payload JSON.");

      if (Obj is SmartHealthCardModel SmartHealthCardModel)
      {
        Result<string> ToJsonResult = ToJson(SmartHealthCardModel, true);
        if (ToJsonResult.Failure)
          return Result<byte[]>.Fail(ToJsonResult.ErrorMessage);

        return  Result<Byte[]>.Ok(await DeflateCompression.CompressAsync(ToJsonResult.Value));        
      }
      else
      {
        throw new ArgumentException($"The {this.GetType().Name} can only work with an input of type {typeof(SmartHealthCardModel).Name}");
      }
    }

    public async Task<Result<T>> DeserializeAsync<T>(byte[] bytes)
    {
      string MinifiedSmartHealthCardJson = await DeflateCompression.UncompressAsync(bytes);
      if (typeof(T) == typeof(SmartHealthCardModel))
      {
        Result<T> SmartHealthCardModelResult = FromJson<T>(MinifiedSmartHealthCardJson);
        if (SmartHealthCardModelResult.Failure)          
          return Result<T>.Fail(SmartHealthCardModelResult.ErrorMessage);
        return await Task.Run(() => FromJson<T>(MinifiedSmartHealthCardJson));               
      }
      else if (typeof(T) == typeof(JwsBody))
      {
        //We must use the PayloadSerializer.Deserialize and not the JsonSerializer.FromJson() because for 
        //SMART Health Cards the payload is DEFLATE compressed bytes and not a JSON string
        Result<T> JwsBodyResult = FromJson<T>(MinifiedSmartHealthCardJson);
        if (JwsBodyResult.Failure)
          return Result<T>.Fail(JwsBodyResult.ErrorMessage);
        return await Task.Run(() => FromJson<T>(MinifiedSmartHealthCardJson));
      }
      else
      {
        throw new TypeAccessException(typeof(T).Name);
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
