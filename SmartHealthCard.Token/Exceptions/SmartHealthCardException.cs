using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Exceptions
{
  public abstract class SmartHealthCardException : SmartHealthCardException
  {
    public SmartHealthCardException(string Message)
        : base(Message)
    {
    }
  }
}
