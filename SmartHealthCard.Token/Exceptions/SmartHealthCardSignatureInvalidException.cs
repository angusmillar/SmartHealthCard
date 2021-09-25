namespace SmartHealthCard.Token.Exceptions
{
  public class SmartHealthCardSignatureInvalidException : SmartHealthCardDecoderException
  {
    public SmartHealthCardSignatureInvalidException(string Message)
      : base(Message) { }
  }
}
