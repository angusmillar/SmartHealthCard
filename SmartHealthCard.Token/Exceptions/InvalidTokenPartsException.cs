using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Exceptions
{
  public class InvalidTokenPartsException : ArgumentOutOfRangeException
  {   
    public InvalidTokenPartsException(string paramName)
        : base(paramName, "A JWS Token must have three parts seperated by dots (e.g Header.Payload.Signature).")
    {
    }
  }
}
