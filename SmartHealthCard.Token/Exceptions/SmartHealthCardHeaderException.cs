using System;

namespace SmartHealthCard.Token.Exceptions
{
  public class SmartHealthCardHeaderException : Exception
  {
    public SmartHealthCardHeaderException(string Message)
      : base(Message) { }
  }
}
