namespace SmartHealthCard.Token.Exceptions
{
  public class InvalidTokenException : SmartHealthCardException
  {   
    public InvalidTokenException(string Message)
        : base(Message){ }
  }
}
