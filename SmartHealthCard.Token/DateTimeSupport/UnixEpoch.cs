using System;

namespace SmartHealthCard.Token.DateTimeSupport
{
  public static class UnixEpoch
  {
    /// <summary>
    /// Describes a point in time, defined as the number of seconds that have elapsed since 00:00:00 UTC, Thursday, 1 January 1970, not counting leap seconds.
    /// See https://en.wikipedia.org/wiki/Unix_time />
    ///  and : https://www.codegrepper.com/code-examples/csharp/c%23+convert+unix+timestamp+to+datetime
    /// </summary>
    public static DateTime Value { get; } = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public static double GetSecondsSince(DateTimeOffset time) =>
        Math.Round((time - Value).TotalSeconds);

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
      // Unix timestamp is seconds past epoch           
      return Value.AddSeconds(unixTimeStamp).ToLocalTime();
    }
  }
}
