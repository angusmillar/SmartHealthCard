using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHealthCard.JwksEndpoint.Controllers
{
  public class ErrorOutcome
  {
    public ErrorOutcome(string errorCode, string message)
    {
      ErrorCode = errorCode;
      Message = message;
    }

    public string ErrorCode { get; set; }
    public string Message { get; set; }
  }
}
