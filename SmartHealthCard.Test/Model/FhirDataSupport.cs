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
