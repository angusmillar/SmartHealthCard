namespace SmartHealthCard.JwksApi
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
