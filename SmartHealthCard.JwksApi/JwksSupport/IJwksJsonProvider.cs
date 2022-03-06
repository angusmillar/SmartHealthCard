namespace SmartHealthCard.JwksApi.JwksSupport
{
  public interface IJwksJsonProvider
  {
    string GetJwksJson(bool FromPEMFile = false);
  }
}