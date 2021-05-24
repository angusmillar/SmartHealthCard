using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Model.Shc
{
  public class CredentialSubject
  {
    public CredentialSubject(string FhirVersion, string FhirBundle)
    {
      //Here inflate Firly model and work out how to cutom serialis it
      JToken JToken = JToken.Parse(FhirBundle);
      this.FhirVersion = FhirVersion;
      this.FhirBundle = JToken;
    }

    [JsonProperty("fhirVersion")]
    public string FhirVersion { get; set; }
    [JsonProperty("fhirBundle")]
    //[Newtonsoft.Json.JsonConverter(typeof(FhirConverter))]
    public JToken FhirBundle { get; }
  }

  public class FhirConverter : Newtonsoft.Json.JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var date = (DateTime)value;
      var niceLookingDate = date.ToString("MMMM dd, yyyy 'at' H:mm tt");
      writer.WriteValue(niceLookingDate);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override bool CanRead
    {
      get { return false; }
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DateTime);
    }
  }
}
