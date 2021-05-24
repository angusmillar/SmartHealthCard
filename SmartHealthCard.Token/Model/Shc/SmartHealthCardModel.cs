using Newtonsoft.Json;
using SmartHealthCard.Token.DateTimeSupport;
using SmartHealthCard.Token.Exceptions;
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
      this.IssuanceDate = UnixEpoch.GetSecondsSince(IssuanceDate.ToUniversalTime()).ToString();
      this.VerifiableCredential = VerifiableCredential;
    }

    public SmartHealthCardModel(Uri Issuer, string IssuanceDate, VerifiableCredential VerifiableCredential)
    {
      this.Issuer = Issuer;
      this.IssuanceDate = IssuanceDate;
      this.VerifiableCredential = VerifiableCredential;
    }

    [JsonProperty("iss")]
    public Uri Issuer { get; set; }
    [JsonProperty("nbf")]
    public string IssuanceDate { get; set; }
    [JsonProperty("vc")]
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

    internal void Validate()
    {
      this.ValidateVerifiableCredentialTypes();
      this.ValidateIssuanceDate();      
    }

    private void ValidateVerifiableCredentialTypes()
    {
      var VerifiableCredentialTypesDic = new Dictionary<VerifiableCredentialTypes, string>()
      {
        { VerifiableCredentialTypes.Covid19, "https://smarthealth.cards#covid19" },
        { VerifiableCredentialTypes.HealthCard, "https://smarthealth.cards#health-card" },
        { VerifiableCredentialTypes.Immunization, "https://smarthealth.cards#immunization" },
        { VerifiableCredentialTypes.Laboratory, "https://smarthealth.cards#laboratory" }
      };
      if (VerifiableCredential.VerifiableCredentialTypeList is null)
      {
        throw new SmartHealthCardPayloadException("Verifiable Credential Type List was found to be null.");
      }
      if (VerifiableCredential.VerifiableCredentialTypeList.Count() == 0)
      {
        throw new SmartHealthCardPayloadException("Verifiable Credential Type List was found to be empty.");
      }
      foreach (Uri uri in VerifiableCredential.VerifiableCredentialTypeList)
      {
        if (!VerifiableCredentialTypesDic.ContainsValue(uri.OriginalString.Trim().ToLower()))
        {
          throw new SmartHealthCardPayloadException($"Invalid Verifiable Credential Types of : {uri.OriginalString}.");
        }
      }      
    }

    private void ValidateIssuanceDate()
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
        throw new SmartHealthCardPayloadException($"IssuanceDate (nbf) must be a number, found the value of {this.IssuanceDate}.");
      }

      if (IssuanceDateEpoch > (EpochNow - ExtraTimeMargin))
      {
        DateTime Date = UnixEpoch.UnixTimeStampToDateTime(EpochNow + ExtraTimeMargin);
        throw new SmartHealthCardPayloadException($"The token's Issuance Date of {Date} is eariler than the current date.");
      }      
    }
  }
}
