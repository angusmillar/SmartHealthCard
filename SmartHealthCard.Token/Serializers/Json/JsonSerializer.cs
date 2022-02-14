using Newtonsoft.Json;
using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Providers;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Support;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static SmartHealthCard.Token.Encoders.Utf8EncodingSupport;

namespace SmartHealthCard.Token.Serializers.Json
{
  public class JsonSerializer : IJwsHeaderSerializer, IJwsPayloadSerializer, IJsonSerializer
  {
    private readonly Newtonsoft.Json.JsonSerializer Serializer;

    public JsonSerializer()
    {
      this.Serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();      
    }

    public virtual async Task<Result<byte[]>> SerializeAsync<T>(T Obj, bool Minified = true)
    {      
      Result<string> ToJsonResult = await Task.Run(() => this.ToJson(Obj, Minified));
      if (ToJsonResult.Failure)
        return Result<byte[]>.Fail(ToJsonResult.ErrorMessage);

      return Result<byte[]>.Ok(GetBytes(ToJsonResult.Value));
    }
    
    public virtual async Task<Result<T>> DeserializeAsync<T>(byte[] bytes)
    {
      return await Task.Run(() => this.FromJson<T>(GetString(bytes)));      
    }

    public Result<string> ToJson<T>(T Obj, bool Minified = true)
    {
      if (!Minified)
        Serializer.Formatting = Formatting.Indented;

      var Builder = new StringBuilder();
      using var StringWriter = new StringWriter(Builder);
      using var JsonWriter = new  JsonTextWriter(StringWriter);
      try
      {
        Serializer.Serialize(JsonWriter, Obj);
      }
      catch(JsonException JsonException)
      {
        return Result<string>.Fail($"Error occurred while converting an instance of the object type {typeof(T).FullName} to JSON. The JSON serializer error was: {JsonException.Message}");
      }      
      return Result<string>.Ok(Builder.ToString());
    }

    public Result<T> FromJson<T>(string Json)
    {      
      using var StringReader = new StringReader(Json);
      using var JsonReader = new JsonTextReader(StringReader);
      try
      {
        T? Item = Serializer.Deserialize<T>(JsonReader);
        if (Item is null)
          return Result<T>.Fail($"The desalinizing of a JSON string failed while attempting to Deserialize to a type of {typeof(T).Name}, desalinizing returned an null object.");
        
        return Result<T>.Ok(Item);
      }
      catch(JsonException JsonException)
      {
        return Result<T>.Fail($"Error occurred while converting a JSON string to an instance of object type {typeof(T).FullName}. The JSON deserialize error was: {JsonException.Message}");
      }            
    }

    public Result<T> FromJsonStream<T>(Stream JsonStream)
    {      
      try 
      {
        using var streamReader = new StreamReader(JsonStream);
        using var jsonReader = new JsonTextReader(streamReader);
        T? Item = Serializer.Deserialize<T>(jsonReader);
        if (Item is null)
          return Result<T>.Fail($"The desalinizing of a JSON stream failed while attempting to Deserialize to a type of {typeof(T).Name}, desalinizing returned an null object.");

        return Result<T>.Ok(Item);
      }
      catch (JsonException JsonException)
      {
        return Result<T>.Fail($"Error occurred while converting a JSON stream to an instance of object type {typeof(T).FullName}. The JSON deserialize error was: {JsonException.Message}");
      }               
    }

  } 
}
