using System;

namespace SmartHealthCard.Token.Exceptions
{
  public class SmartHealthCardPayloadException : Exception
  {
    public SmartHealthCardPayloadException(string Message) 
      : base(Message){ }
  }
}
