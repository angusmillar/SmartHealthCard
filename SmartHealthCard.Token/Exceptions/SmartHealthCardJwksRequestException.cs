namespace SmartHealthCard.Token.Exceptions
{
  public class SmartHealthCardJwksRequestException : SmartHealthCardDecoderException
  {
    public SmartHealthCardJwksRequestException(string Message) 
      : base(Message){ }
  }
}
