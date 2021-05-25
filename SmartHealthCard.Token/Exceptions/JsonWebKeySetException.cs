namespace SmartHealthCard.Token.Exceptions
{
  public class JsonWebKeySetException : SmartHealthCardException
  {
    public JsonWebKeySetException(string Message)
        : base(Message) { }
  }
}