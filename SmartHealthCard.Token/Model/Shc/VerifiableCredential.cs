using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartHealthCard.Token.Enums;
using SmartHealthCard.Token.Exceptions;

namespace SmartHealthCard.Token.Model.Shc
{
  public class VerifiableCredential
  {
    public VerifiableCredential(List<VerifiableCredentialType> VerifiableCredentialTypeList, CredentialSubject CredentialSubject)
    {
      this.VerifiableCredentialTypeList = VerifiableCredentialTypeList;
      this.CredentialSubject = CredentialSubject;
    }

    [Newtonsoft.Json.JsonProperty(propertyName: "type", ItemConverterType = typeof(VerifiableCredentialTypeConverter), Required = Required.Always)]
    public List<VerifiableCredentialType> VerifiableCredentialTypeList { get; set; }
    [JsonProperty("credentialSubject", Required = Required.Always)]
    public CredentialSubject CredentialSubject { get; set; }

    public class VerifiableCredentialTypeConverter : Newtonsoft.Json.JsonConverter
    {      
      public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
      {
        if (value is VerifiableCredentialType VerifiableCredentialType)
        {
          writer.WriteValue(VerifiableCredentialType.GetLiteral());
        }
      }

      public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
      {
        if (reader.Value is object)
        {
          string VerifiableCredentialTypeString = (string)reader.Value;
          if (VerifiableCredentialTypeSupport.VerifiableCredentialTypeDictionary.ContainsKey(VerifiableCredentialTypeString))
          {
            return VerifiableCredentialTypeSupport.VerifiableCredentialTypeDictionary[VerifiableCredentialTypeString];
          }
          else
          {            
            throw new SmartHealthCardPayloadException($"One of the VerifiableCredentialTypes (vc.type) was not an allowed value, type found was: {VerifiableCredentialTypeString}. The supported types are: {GetSupportedVerifiableCredentialTypeUriStringList()}");
          }
        }
        else
        {
          throw new SmartHealthCardPayloadException($"Must provide at least one VerifiableCredentialTypes (vc.type). The supported types are: {GetSupportedVerifiableCredentialTypeUriStringList()}");
        }        
      }

      private static string GetSupportedVerifiableCredentialTypeUriStringList()
      {
        string AllowedTypesList = string.Empty;
        foreach (var item in VerifiableCredentialTypeSupport.VerifiableCredentialTypeDictionary)
        {
          AllowedTypesList += $"{item.Key}, ";
        }
        AllowedTypesList = AllowedTypesList.Trim().TrimEnd(',');
        return AllowedTypesList;
      }

      public override bool CanRead
      {
        get { return true; }
      }

      public override bool CanConvert(Type objectType)
      {
        return objectType == typeof(string);
      }
    }


  }
}
