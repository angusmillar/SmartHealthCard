using System;

namespace SmartHealthCard.Token.Exceptions
{
    public class SignatureVerificationException : Exception
    {        
        public SignatureVerificationException(string message)
            : base(message){ }

    }
}