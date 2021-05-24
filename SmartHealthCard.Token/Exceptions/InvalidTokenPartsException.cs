using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Exceptions
{
  public class InvalidTokenPartsException : SmartHealthCardException
  {   
    public InvalidTokenPartsException(string Message)
        : base(Message)
    {
    }
  }
}
