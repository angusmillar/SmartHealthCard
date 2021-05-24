using System;

namespace SmartHealthCard.Token.Exceptions
{
  public class SmartHealthCardHeaderException : SmartHealthCardException
  {
    public SmartHealthCardHeaderException(string Message)
      : base(Message) { }
  }
}
