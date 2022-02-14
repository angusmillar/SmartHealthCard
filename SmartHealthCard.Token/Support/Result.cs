using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHealthCard.Token.Support
{
  public partial class Result
  {
    protected Result(bool Success, bool Retryable, string ErrorMessage) 
    {
      this.Success = Success;
      this.ErrorMessage = ErrorMessage;
      this.Retryable = Retryable;    
    }

    private static readonly string ErrorMessagesDelimiter = ", ";

    /// <summary>
    /// The action was Successful and the result value property will be set. 
    /// </summary>
    public bool Success { get; private set; }
    /// <summary>
    /// Indicates that the operation failed and no Result value can be returned. Refer the ErrorMessage for more detail.   
    /// </summary>
    public bool Failure => !Success;
    /// <summary>
    /// Indicate that this result believes the operation could be retried again. 
    /// It is a decision of the caller to check Retryable or not, checking Success or Failure is usualy enough.
    /// /// If Retryable is True then Success SHALL BE False and Failure SHALL BE True 
    /// If Success is True then Retryable will be False
    /// </summary>    
    public bool Retryable { get; private set; }
    /// <summary>
    /// An error message that is populated when Success is False and Failure is true, is empty string of Success
    /// </summary>
    public string ErrorMessage { get; private set; }
    
    public static Result Ok()
    {
      return new Result(Success: true, Retryable: false, ErrorMessage: string.Empty);
    }

    public static Result Retry(string message)
    {
      return new Result(Success: false, Retryable: true, ErrorMessage: message);
    }

    public static Result Fail(string message)
    {
      return new Result(Success: false, Retryable: false, ErrorMessage: message);
    }

    public static Result Combine(params Result[] ResultArray)
            => Combine(ResultArray, ErrorMessagesDelimiter);
    public static Result Combine(string ErrorMessageDelimiter, params Result[] ResultArray)
            => Combine(ResultArray, ErrorMessageDelimiter);

    public static Result Combine<T>(params Result<T>[] results)
            => Combine(results, ErrorMessagesDelimiter);
    public static Result Combine<T>(string ErrorMessageDelimiter, params Result<T>[] ResultArray)
            => Combine(ResultArray, ErrorMessageDelimiter);

    public static Result Combine(IEnumerable<Result> ResultList, string? ErrorMessageDelimiter = null)
    {
      List<Result> FailedResultList = ResultList.Where(x => x.Failure).ToList();

      if (FailedResultList.Count == 0)
        return Result.Ok();

      string CombinedMessages = string.Join(ErrorMessageDelimiter ?? ErrorMessagesDelimiter, AggregateMessages(FailedResultList.Select(x => x.ErrorMessage)));

      return Result.Fail(CombinedMessages);
    }

    public static Result Combine<T>(IEnumerable<Result<T>> TypedResultList, string? ErrorMessageDelimiter = null)
    {
      IEnumerable<Result> UntypedResultList = TypedResultList.Select(result => (Result)result);
      return Combine(UntypedResultList, ErrorMessageDelimiter);
    }

    private static IEnumerable<string> AggregateMessages(IEnumerable<string> messages)
    {
      var dict = new Dictionary<string, int>();
      foreach (var message in messages)
      {
        if (!dict.ContainsKey(message))
          dict.Add(message, 0);

        dict[message]++;
      }
      return dict.Select(x => x.Value == 1 ? x.Key : $"{x.Key} ({x.Value}×)");
    }

  }

  public class Result<T> : Result
  {
    private T? ResultValue { get; set; }
    public T Value
    {
      get
      {
        if (Success && ResultValue is object)
        {
          return ResultValue;
        }
        else
        {
          throw new ArgumentNullException("");
        }
      }
    }

    public static Result<T> Ok(T value)
    {
      return new Result<T>(Value: value, Success: true, ErrorMessage: string.Empty);
    }

    public static new Result<T> Retry(string message)
    {
      return new Result<T>(Success: false, Retryable: true, ErrorMessage: message);
    }

    public static new Result<T> Fail(string message)
    {
      return new Result<T>(Success: false, Retryable: false, ErrorMessage: message);
    }


    protected internal Result(T Value, bool Success, string ErrorMessage)
        : base(Success: Success, Retryable: false, ErrorMessage: ErrorMessage)
    {
      this.ResultValue = Value;
    }
    
    protected internal Result(bool Success, bool Retryable, string ErrorMessage)
        : base(Success: Success, Retryable: Retryable, ErrorMessage: ErrorMessage)
    {
      this.ResultValue = default;
    }

  }


}
