using SmartHealthCard.Token.Providers;
using SmartHealthCard.Token.Support;
using System.IO;

namespace SmartHealthCard.Token.Serializers.Json
{
  public interface IJsonSerializer 
  {
    /// <summary>
    /// Serialize an object to a JSON string respecting the Minified flag to produce a Minified JSON string
    /// </summary>
    string ToJson<T>(T Obj, bool Minified = true);

    /// <summary>
    /// De-serialize a JSON string to typed object.
    /// </summary>
    T FromJson<T>(string Json);

    public Result<T> FromJsonStream<T>(Stream JsonStream);
  }
}
