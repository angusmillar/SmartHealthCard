using SmartHealthCard.Token.Serializers.Json;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Serializers.Jws
{
  public interface IJwsSerializer //: IJsonSerializer
  {
    /// <summary>
    /// Serialize an object to a JSON string byte[]
    /// </summary>
    Task<byte[]> SerializeAsync<T>(T Obj, bool Minified = true);

    /// <summary>
    /// Deserialize a JSON string to typed object.
    /// </summary>
    Task<T> DeserializeAsync<T>(byte[] bytes);
  }
}
