using SmartHealthCard.Token.Serializers.Json;

namespace SmartHealthCard.Token.Serializers.Jws
{
  public interface IJwsSerializer //: IJsonSerializer
  {
    /// <summary>
    /// Serialize an object to a JSON string byte[]
    /// </summary>
    byte[] Serialize<T>(T Obj, bool Minified = true);

    /// <summary>
    /// Deserialize a JSON string to typed object.
    /// </summary>
    T Deserialize<T>(byte[] bytes);
  }
}
