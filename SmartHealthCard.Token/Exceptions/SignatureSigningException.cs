using System;

namespace SmartHealthCard.Token.Exceptions
{
    public class SignatureSigningException : Exception
    {        
        public SignatureSigningException(string message)
            : base(message){ }

    }
}