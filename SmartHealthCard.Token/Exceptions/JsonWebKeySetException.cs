using System;

namespace SmartHealthCard.Token.Exceptions
{
    public class JsonWebKeySetException : Exception
    {        
        public JsonWebKeySetException(string message)
            : base(message){ }

    }
}