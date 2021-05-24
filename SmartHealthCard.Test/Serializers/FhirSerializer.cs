using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Test.Serializers
{
  public static class FhirSerializer
  {
    public static string SerializeToJson(Resource Resource)
    {     
      var serializer = new FhirJsonSerializer(new SerializerSettings()
      {
        Pretty = true
      });
      return serializer.SerializeToString(Resource);
    }
  }
}
