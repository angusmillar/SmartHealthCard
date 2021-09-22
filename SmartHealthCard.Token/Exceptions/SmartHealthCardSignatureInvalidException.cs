namespace SmartHealthCard.Token.Exceptions
{
  public class SmartHealthCardSignatureInvalidException : SmartHealthCardException
  {
    public SmartHealthCardSignatureInvalidException(string Message)
      : base(Message) { }
  }
}
