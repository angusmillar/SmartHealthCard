using System;

namespace SmartHealthCard.Token.Exceptions
{
    public class SignatureSigningException : SmartHealthCardException
  {        
        public SignatureSigningException(string message)
            : base(message){ }

    }
}