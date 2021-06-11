using Newtonsoft.Json;
using SmartHealthCard.Token.DateTimeSupport;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Support;
using System;
using System.Collections.Generic;
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
    [JsonProperty("nbf", Required = Required.Always)]
    public double IssuanceDate { get; set; }
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
      return new DateTimeOffset(UnixEpoch.UnixTimeStampToDateTime(NbfDouble));     
    }

    internal Result Validate()
    {     
      return this.ValidateIssuanceDate();      
    }
    
    private Result ValidateIssuanceDate()
    {
      var EpochNow = UnixEpoch.GetSecondsSince(DateTimeOffset.UtcNow);
      int ExtraTimeMargin = 0; //Can add extra seconds onto expiry if required
      double IssuanceDateEpoch;
      try
      {
        IssuanceDateEpoch = Convert.ToDouble(this.IssuanceDate);
      }
      catch
      {
        return Result.Fail($"IssuanceDate (nbf) must be a number, found the value of {this.IssuanceDate}.");        
      }

      if (IssuanceDateEpoch > (EpochNow - ExtraTimeMargin))
      {
        DateTime Date = UnixEpoch.UnixTimeStampToDateTime(EpochNow + ExtraTimeMargin);
        return Result.Fail($"The token's Issuance Date of {Date} is earlier than the current date.");        
      }
      return Result.Ok();
    }
  }
}
