namespace SmartHealthCard.Token.Exceptions
{
  public class SignatureSigningException : SmartHealthCardException
  {
    public SignatureSigningException(string Message)
        : base(Message) { }

  }
}