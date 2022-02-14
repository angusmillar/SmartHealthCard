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
      this.FhirVersion = FhirVersion;

      //We must minify the FHIR Bundle as provided by users here as the
      //JSON Serializer/Deserializer does not minify JRaw types.
      //So if we don't do this here the JWS Token's payload's property 'vc.credentialSubject.fhirBundle'
      //will not be minified yet the rest of the JSON will be.
      //We are storing the fhirBundle as JRaw so that we do not have to parse the FHIR bundle, as that
      //would require dependencies on something like .NET FHIR API (https://fire.ly/products/firely-net-sdk/)
      //Maybe we should do this but at present I feel it is the role of the fhirBundle creator to ensure they get it right
      //I highly advise they use the .NET FHIR API
      this.FhirBundleData = GetMinifiedJsonJRaw(FhirBundle);
    }

    private static JRaw GetMinifiedJsonJRaw(string FhirBundle)
    {
      var obj = JsonConvert.DeserializeObject(FhirBundle);
      string MinifiyedFhirBundle = JsonConvert.SerializeObject(obj, Formatting.None);
      return new JRaw(MinifiyedFhirBundle);
    }

    [Newtonsoft.Json.JsonConstructor]
    public CredentialSubject(string FhirVersion, JRaw FhirBundle)
    {     
      this.FhirVersion = FhirVersion;      
      this.FhirBundleData = FhirBundle;
    }

    [JsonProperty("fhirVersion", Required = Required.Always)]
    public string FhirVersion { get; set; }

    [JsonProperty("fhirBundle", Required = Required.Always)]
    internal JRaw FhirBundleData { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string FhirBundle
    {
      get
      {
        if (FhirBundleData is not null)
        {
          var obj = JsonConvert.DeserializeObject(this.FhirBundleData.ToString());
          return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        else
        {
          return string.Empty;
        }        
      }
      set
      {
        FhirBundleData = GetMinifiedJsonJRaw(value);
      }
    }
  }
}
