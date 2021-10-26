using Newtonsoft.Json;
using SmartHealthCard.Token.DateTimeSupport;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Support;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SmartHealthCard.Token.Model.Shc
{ 
  public class SmartHealthCardModel
  {
    public SmartHealthCardModel(Uri Issuer, DateTimeOffset IssuanceDate, VerifiableCredential VerifiableCredential)
    {
      this.Issuer = Issuer;      
      this.IssuanceDate = UnixEpoch.GetSecondsSince(IssuanceDate.ToUniversalTime());
      this.VerifiableCredential = VerifiableCredential;
    }

    [JsonConstructor]
    public SmartHealthCardModel(Uri Issuer, double IssuanceDate, VerifiableCredential VerifiableCredential)
    {
      this.Issuer = Issuer;
      this.IssuanceDate = IssuanceDate;
      this.VerifiableCredential = VerifiableCredential;
    }

    [JsonProperty("iss", Required = Required.Always)]
    public Uri Issuer { get; set; }
    [JsonProperty("nbf", Required = Required.Default)]
    public double? IssuanceDate { get; set; }
    [JsonProperty("vc", Required = Required.Always)]
    public VerifiableCredential VerifiableCredential { get; set; }

    public DateTimeOffset GetIssuanceDate()
    {
      double NbfDouble;
      try
      {
        NbfDouble = Convert.ToDouble(this.IssuanceDate);
      }
      catch
      {
        throw new SmartHealthCardPayloadException($"IssuanceDate (nbf) must be a number, found the value of {this.IssuanceDate}.");
      }
      return UnixEpoch.UnixTimeStampToLocalDateTimeOffset(NbfDouble);     
    }

    internal Result Validate()
    {     
      return this.ValidateIssuanceDate();       
    }

    /// <summary>
    /// Note that the Json parser for IssuanceDate or (nbf) allows a blank nbf property in the token. In this case the IssuanceDate double is equal to zero '0'.
    /// Therefore this validation still runs yet the IssuanceDate is now equal to the Unix Epoch minimum datetime which 00:00:00 UTC on 1 January 1970.
    /// Given that SMART Health Cards or JWT token did not exist prior to this date this should not be a problem; unless someone wishes to create a token that is valid from before that datetime.
    /// It is therefore a design descion of this libaray to not support IssuanceDate's prior to 00:00:00 UTC on 1 January 1970.  
    /// </summary>
    /// <returns></returns>
    private Result ValidateIssuanceDate()
    {
      var EpochNow = UnixEpoch.GetSecondsSince(DateTimeOffset.UtcNow);

      //(https://datatracker.ietf.org/doc/html/rfc7519#section-4.1.5)
      //Quote: Implementers MAY provide for some small leeway, usually no more than a few minutes, to account for clock skew. 
      //This libaray will add 2 min for the leeway.
      int ExtraTimeMargin = 120;
      
      double IssuanceDateEpoch;
      try
      {
        IssuanceDateEpoch = Convert.ToDouble(this.IssuanceDate);
      }
      catch
      {
        return Result.Fail($"IssuanceDate (nbf) must be a number, found the value of {this.IssuanceDate}.");        
      }

      if ((EpochNow + ExtraTimeMargin) < IssuanceDateEpoch)
      {
        DateTimeOffset Date = UnixEpoch.UnixTimeStampToLocalDateTimeOffset(EpochNow + ExtraTimeMargin);        
        return Result.Fail($"The token's Issuance Date (nbf) timestamp is earlier than the current date and time. The token is not valid untill: {Date.ToString(CultureInfo.CurrentCulture)}.");
      }
      
      return Result.Ok();
    }
  }
}
