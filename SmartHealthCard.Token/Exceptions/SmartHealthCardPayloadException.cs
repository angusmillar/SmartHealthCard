using System;

namespace SmartHealthCard.Token.Exceptions
{
  public class SmartHealthCardPayloadException : SmartHealthCardException
  {
    public SmartHealthCardPayloadException(string Message) 
      : base(Message){ }
  }
}
