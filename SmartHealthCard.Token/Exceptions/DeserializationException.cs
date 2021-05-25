namespace SmartHealthCard.Token.Exceptions
{
  class DeserializationException : SmartHealthCardException
  {
    public DeserializationException(string Message)
        : base(Message){ }
  }
}
