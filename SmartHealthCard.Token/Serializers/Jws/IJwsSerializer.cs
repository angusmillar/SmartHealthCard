namespace SmartHealthCard.Token.Serializers.Jws
{
  public interface IJwsSerializer 
  {
    /// <summary>
    /// Serialize an object a byte[]
    /// </summary>
    byte[] Serialize<T>(T Obj);

    /// <summary>
    /// Deserialize a string to typed object.
    /// </summary>
    T Deserialize<T>(byte[] bytes);
  }
}
