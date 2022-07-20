using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Test.Model
{
  static class FhirDataSupport
  {
    public static Bundle GetCovid19DetectedFhirBundleExample()
    {
      Patient PatientResource = GetPatientResource("Coyote", "Wile E", new DateTime(1973, 09, 30), "61481059995");

      Coding Code = new(system: "http://loinc.org", code: "94558-4"); //SARS-CoV-2 (COVID-19) Ag [Presence] in Respiratory specimen by Rapid immunoassay
      Coding Value = new(system: "http://snomed.info/sct", code: "260373001"); //Detected
      Observation CovidResultObservationResource = GetObservationResource(
        ObsCode: Code, 
        ObsValue: Value, 
        EffectiveDate: new DateTime(2021, 05, 24), 
        PerformerOrganisationName: "ACME Healthcare", 
        IdentityAssuranceLevelCode: "IAL1.4");

      List<Resource> BundleResourceList = new()
      {
        PatientResource,
        CovidResultObservationResource
      };

      Bundle Bundle = new();
      Bundle.Type = Bundle.BundleType.Collection;
      Bundle.Entry = GetBundleResourceEntryList(BundleResourceList);

      return Bundle;
    }

    public static Bundle GetCovid19NotDetectedFhirBundleExample()
    {
      Patient PatientResource = GetPatientResource("Coyote", "Wile E", new DateTime(1973, 09, 30), "61481059995");

      Coding Code = new(system: "http://loinc.org", code: "94558-4"); //SARS-CoV-2 (COVID-19) Ag [Presence] in Respiratory specimen by Rapid immunoassay
      Coding Value = new(system: "http://snomed.info/sct", code: "260415000"); //Not Detected
      Observation CovidResultObservationResource = GetObservationResource(
        ObsCode: Code,
        ObsValue: Value,
        EffectiveDate: new DateTime(2021, 05, 24),
        PerformerOrganisationName: "ACME Healthcare",
        IdentityAssuranceLevelCode: "IAL1.4");

      List<Resource> BundleResourceList = new()
      {
        PatientResource,
        CovidResultObservationResource
      };

      Bundle Bundle = new();
      Bundle.Type = Bundle.BundleType.Collection;
      Bundle.Entry = GetBundleResourceEntryList(BundleResourceList);

      return Bundle;
    }

    public static Bundle GetINRFhirBundleExample(DateTime ReleaseDate, decimal PT, decimal INR)
    {
      //OBX|1|NM|F0005^PT^2178^5902-2^PT^LN||13|s|9-13||||F|||202207201031
      //OBX|2|NM|F0003^INR^2178^6301-6^INR^LN||1.2||0.9-1.2||||F|||202207201031
      
      Patient PatientResource = GetPatientResource("Coyote", "Wile E", new DateTime(1973, 09, 30), "61481059995");

      //PT
      Coding PTCode = new(system: "http://loinc.org", code: "5902-2", display: "PT"); 
      Quantity PTValue = new();
      PTValue.Value = PT;
      PTValue.Code = "s";
      PTValue.System = "http://unitsofmeasure.org";


      var PTRange = new Observation.ReferenceRangeComponent()
      {
        Low = new Quantity()
        {
          Value = 9m,
          Unit = "s",
          Code = "s",
          System = "http://unitsofmeasure.org"
        },
        High = new Quantity()
        {
          Value = 13m,
          Unit = "s",
          Code = "s",
          System = "http://unitsofmeasure.org"
        },                
      };

      Observation PTResultObservationResource = GetObservationResource(
        ObsCode: PTCode,
        ObsValue: PTValue,
        Range: PTRange,
        EffectiveDate: ReleaseDate,
        PerformerOrganisationName: "ACME Healthcare",
        IdentityAssuranceLevelCode: "IAL1.4");



      //INR
      Coding InrCode = new(system: "http://loinc.org", code: "6301-6", display: "INR");
      Quantity InrValue = new();
      InrValue.Value = INR;

      var INRRange = new Observation.ReferenceRangeComponent()
      {
        Low = new Quantity()
        {
          Value = 0.9m,         
        },
        High = new Quantity()
        {
          Value = 1.2m,          
        },        
      };


      Observation InrResultObservationResource = GetObservationResource(
        ObsCode: InrCode,
        ObsValue: InrValue,
        Range: INRRange,
        EffectiveDate: ReleaseDate,
        PerformerOrganisationName: "ACME Healthcare",
        IdentityAssuranceLevelCode: "IAL1.4");

      List<Resource> BundleResourceList = new()
      {
        PatientResource,
        PTResultObservationResource,
        InrResultObservationResource
      };

      Bundle Bundle = new();
      Bundle.Type = Bundle.BundleType.Collection;
      Bundle.Entry = GetBundleResourceEntryList(BundleResourceList);

      return Bundle;
    }

    private static Patient GetPatientResource(string FamilyName, string GivenName, DateTime DateOfBirth, string PhoneNumber)
    {
      Patient Patient = new();
      Patient.Name = new List<HumanName>()
      {
        new HumanName()
        {
           Family = FamilyName,
           Given = new List<string>(){ GivenName }
        }
      };
      Patient.BirthDateElement = new Date(DateOfBirth.Year, DateOfBirth.Month, DateOfBirth.Day);
      Patient.Telecom = new List<ContactPoint>()
      {
        new ContactPoint(null, null, PhoneNumber)
      };
      return Patient;
    }

    private static Observation GetObservationResource(Coding ObsCode, Coding ObsValue, DateTime EffectiveDate, string PerformerOrganisationName, string IdentityAssuranceLevelCode = null)
    {
      Observation Observation = new();
      if (!string.IsNullOrEmpty(IdentityAssuranceLevelCode))
      {
        Observation.Meta = new Meta()
        {
          Security = new List<Coding>()
         {
           new Coding(system: null, code: IdentityAssuranceLevelCode)
         }
        };
      }
      Observation.Status = ObservationStatus.Final;
      Observation.Category = new List<CodeableConcept>()
      {
        new CodeableConcept(system: "http://terminology.hl7.org/CodeSystem/observation-category", code: "laboratory")
      };
      Observation.Code = new CodeableConcept()
      {
        Coding = new List<Coding>()
         {
           ObsCode
         }
      };
      
      Observation.Effective = new FhirDateTime(EffectiveDate.Year, EffectiveDate.Month, EffectiveDate.Day);
      Observation.Value = new CodeableConcept()
      {
          Coding = new List<Coding>()
          {
            ObsValue
          }
      };
      Observation.Performer = new List<ResourceReference>()
      {
        new ResourceReference(null, PerformerOrganisationName)
      };

      return Observation;
    }
    private static Observation GetObservationResource(Coding ObsCode, Quantity ObsValue, Observation.ReferenceRangeComponent Range,  DateTime EffectiveDate, string PerformerOrganisationName, string IdentityAssuranceLevelCode = null)
    {
      Observation Observation = new();
      if (!string.IsNullOrEmpty(IdentityAssuranceLevelCode))
      {
        Observation.Meta = new Meta()
        {
          Security = new List<Coding>()
         {
           new Coding(system: null, code: IdentityAssuranceLevelCode)
         }
        };
      }
      Observation.Status = ObservationStatus.Final;
      Observation.Category = new List<CodeableConcept>()
      {
        new CodeableConcept(system: "http://terminology.hl7.org/CodeSystem/observation-category", code: "laboratory")
      };
      Observation.Code = new CodeableConcept()
      {
        Coding = new List<Coding>()
         {
           ObsCode
         }
      };

      Observation.Effective = new FhirDateTime(EffectiveDate.Year, EffectiveDate.Month, EffectiveDate.Day);
      Observation.Value = ObsValue;
      Observation.Performer = new List<ResourceReference>()
      {
        new ResourceReference(null, PerformerOrganisationName)
      };
      if (Range != null)
      {
        Observation.ReferenceRange = new List<Observation.ReferenceRangeComponent>()
        {
          Range
        };
      }
      
      return Observation;
    }

    private static List<Bundle.EntryComponent> GetBundleResourceEntryList(List<Resource> ResourceList)
    {
      string ReferencePrefix = "resource";
      Resource PatientResource = ResourceList.SingleOrDefault(x => x.TypeName == ResourceType.Patient.GetLiteral());
      if (PatientResource is null)
        throw new ApplicationException("The Bundle must have one and only one Patient resource.");

      //Ensure the first Resource is the Patient Resource, so the subject reference is correct in the next step.
      if (!ResourceList[0].Equals(PatientResource))
      {
        //Move the Patient Resource to first
        ResourceList.Remove(PatientResource);
        ResourceList.Insert(0, PatientResource);
      }
      
      ResourceReference PatientSubjectReference = new($"{ReferencePrefix}:0");

      var EntryComponentList = new List<Bundle.EntryComponent>();
      for (int i = 0; i < ResourceList.Count; i++)     
      {
        if (ResourceList[i] is Observation Obs)
        {
          Obs.Subject = PatientSubjectReference;
        }
        else if (ResourceList[i] is not Patient)
        {
          throw new ApplicationException($"Only support Bundle with Patient and Observation resources");
        }

        EntryComponentList.Add(new Bundle.EntryComponent()
        {
          FullUrl = $"{ReferencePrefix}:{i}",
          Resource = ResourceList[i]
        });
      }
      return EntryComponentList;
    }
  }
}
