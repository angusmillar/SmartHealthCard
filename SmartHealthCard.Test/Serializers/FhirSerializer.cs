using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text.Json;

namespace SmartHealthCard.Test.Serializers
{
  public static class FhirSerializer
  {
    public static string SerializeToJson(Resource resource)
    {     
      var options = new JsonSerializerOptions().ForFhir(ModelInfo.ModelInspector).Pretty();
      return JsonSerializer.Serialize(resource, options);
    }
  }
}
