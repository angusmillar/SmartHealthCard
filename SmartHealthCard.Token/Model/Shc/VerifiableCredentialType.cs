using SmartHealthCard.Token.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Model.Shc
{
  public enum VerifiableCredentialType
  {
    [EnumInfo(literal: "https://smarthealth.cards#health-card", description: "HealthCard")]
    HealthCard,
    [EnumInfo(literal: "https://smarthealth.cards#covid19", description: "Covid19")]
    Covid19,
    [EnumInfo(literal: "https://smarthealth.cards#immunization", description: "Immunization")]
    Immunization,
    [EnumInfo(literal: "https://smarthealth.cards#laboratory", description: "Laboratory")]
    Laboratory
  }

  public static class VerifiableCredentialTypeSupport
  {
    public static readonly Dictionary<string, VerifiableCredentialType> VerifiableCredentialTypeDictionary = new Dictionary<string, VerifiableCredentialType>()
        {
          { VerifiableCredentialType.Covid19.GetLiteral(), VerifiableCredentialType.Covid19 },
          { VerifiableCredentialType.HealthCard.GetLiteral(), VerifiableCredentialType.HealthCard },
          { VerifiableCredentialType.Immunization.GetLiteral(), VerifiableCredentialType.Immunization },
          { VerifiableCredentialType.Laboratory.GetLiteral(), VerifiableCredentialType.Laboratory }
        };
  }


}
