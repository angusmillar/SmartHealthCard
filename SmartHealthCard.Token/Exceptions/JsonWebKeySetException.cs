using System;

namespace SmartHealthCard.Token.Exceptions
{
    public class JsonWebKeySetException : SmartHealthCardException
  {        
        public JsonWebKeySetException(string message)
            : base(message){ }

    }
}