using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHealthCard.Token.Model.Shc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Serializers.Shc
{
  public static class SmartHealthCardModelJsonSerializer
  {   
    public static string ToJson(this SmartHealthCardModel SmartHealthCardModel, bool Minified = true)
    {
      JsonSerializer Serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();  
      
      if (Minified)
        Serializer.Formatting = Formatting.None;
      else
        Serializer.Formatting = Formatting.Indented;

      var sb = new StringBuilder();
      using var stringWriter = new StringWriter(sb);
      using var jsonWriter = new JsonTextWriter(stringWriter);
      Serializer.Serialize(jsonWriter, SmartHealthCardModel);
      string MinifiedSmartHealthCardJson = sb.ToString();
      return MinifiedSmartHealthCardJson;
    }

    public static SmartHealthCardModel FromJson(this string SmartHealthCardJson)
    {
      TokenPayloadData TokenPayloadData = new TokenPayloadData();
      return  TokenPayloadData.ParseToSmartHealthCardModel(SmartHealthCardJson);      
    }

    internal class TokenPayloadData
    {
      public TokenPayloadData()
      {
        this.VerifiableCredentialTypeList = new List<string>();
      }
      public string? Issuer { get; set; }
      public string? IssuanceDate { get; set; }
      public string? FhirVersion { get; set; }
      public string? FhirBundle { get; set; }

      public List<string> VerifiableCredentialTypeList = new List<string>();

      public SmartHealthCardModel ParseToSmartHealthCardModel(string json)
      {
        ValidateJson(json);
        return ValidateSmartHealthCardModel();
      }

      private void ValidateJson(string json)
      {
        JObject SmartHealthCardJObject = JObject.Parse(json);
        if (SmartHealthCardJObject is not null)
        {
          JToken? IssuerJToken = SmartHealthCardJObject["iss"];
          if (IssuerJToken is null)
          {
            throw new FormatException("Unable to local the 'iss' element in JSON payload ");
          }
          else
          {
            this.Issuer = IssuerJToken.ToString();
          }

          JToken? IssuanceDateJToken = SmartHealthCardJObject["nbf"];
          if (IssuanceDateJToken is null)
          {
            throw new FormatException("Unable to local the 'nbf' element in JSON payload ");
          }
          else
          {
            this.IssuanceDate = IssuanceDateJToken.ToString();
          }

          JToken? VCJToken = SmartHealthCardJObject["vc"];
          if (VCJToken is null)
          {
            throw new FormatException("Unable to local the 'vc' element in JSON payload ");
          }
          else
          {
            JToken? TypeJTokenList = VCJToken["type"];
            if (TypeJTokenList is null)
            {
              throw new FormatException("Unable to local the 'vc.type' element in JSON payload ");
            }
            else
            {
              List<JToken> VerifiableCredentialTypeJTokenList = TypeJTokenList.ToList();
              foreach (JToken Type in VerifiableCredentialTypeJTokenList)
              {
                this.VerifiableCredentialTypeList.Add(Type.ToString());
              }
            }

            JToken? CredentialSubjectJToken = VCJToken["credentialSubject"];
            if (CredentialSubjectJToken is null)
            {
              throw new FormatException("Unable to local the 'vc.credentialSubject' element in JSON payload ");
            }
            else
            {
              JToken? FhirVersionJToken = CredentialSubjectJToken["fhirVersion"];
              if (FhirVersionJToken is null)
              {
                throw new FormatException("Unable to local the 'vc.credentialSubject.fhirVersion' element in JSON payload ");
              }
              else
              {
                this.FhirVersion = FhirVersionJToken.ToString();
              }

              JToken? FhirBundleJToken = CredentialSubjectJToken["fhirBundle"];
              if (FhirBundleJToken is null)
              {
                throw new FormatException("Unable to local the 'vc.credentialSubject.fhirBundle' element in JSON payload ");
              }
              else
              {
                this.FhirBundle = FhirBundleJToken.ToString();
              }
            }
          }
        }
      }

      private SmartHealthCardModel ValidateSmartHealthCardModel()
      {
        //Populate the SMART Health Card logical model
        Uri IssuerUri;
        if (!Uri.TryCreate(this.Issuer, UriKind.Absolute, result: out Uri? TempIssuerUri))
        {
          throw new FormatException($"The Issuer (iss) value was unable to be parsed as a Uri. Value was : {this.Issuer}");
        }
        else
        {
          IssuerUri = TempIssuerUri;
        }

        string IssuanceDate;
        if (!Double.TryParse(this.IssuanceDate, out double TempDouble))
        {
          throw new FormatException($"The IssuanceDate (nbf) value was unable to be parsed as a double for Unix TimeStamp conversion. Value was : {this.IssuanceDate}");
        }
        else
        {
          IssuanceDate = TempDouble.ToString();
        }

        List<Uri> VerifiableCredentialTypeList = new List<Uri>();
        if (this.VerifiableCredentialTypeList.Count() == 0)
        {
          throw new FormatException($"The Verifiable Credential Type (vc.type) was empty.");
        }
        else
        {
          foreach (string UriString in this.VerifiableCredentialTypeList)
          {
            if (!Uri.TryCreate(UriString, UriKind.Absolute, result: out Uri? TempTypeUri))
            {
              throw new FormatException($"The Issuer (iss) value was unable to be parsed as a Uri. Value was : {this.Issuer}");
            }
            else
            {
              VerifiableCredentialTypeList.Add(TempTypeUri);
            }
          }
        }

        if (string.IsNullOrWhiteSpace(this.FhirVersion))
        {
          throw new FormatException($"The FhirVersion (vc.credentialSubject.fhirVersion) value was found to be empty.");
        }

        if (string.IsNullOrWhiteSpace(this.FhirBundle))
        {
          throw new FormatException($"The FhirBundle (vc.credentialSubject.fhirBundle) value was found to be empty.");
        }

        SmartHealthCardModel SmartHealthCardModel = new SmartHealthCardModel(IssuerUri, IssuanceDate,
            new VerifiableCredential(VerifiableCredentialTypeList,
              new CredentialSubject(this.FhirVersion, this.FhirBundle)));

        return SmartHealthCardModel;
      }

    }

  }
}
