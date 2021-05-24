using System;

namespace SmartHealthCard.Token.Exceptions
{
    public class SignatureVerificationException : SmartHealthCardException
  {        
        public SignatureVerificationException(string message)
            : base(message){ }

    }
}