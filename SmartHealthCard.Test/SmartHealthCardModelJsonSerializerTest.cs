using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SmartHealthCard.Token.Serializers.Shc;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Encoders;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardModelJsonSerializerTest
  {

    [Fact]
    public void Valid_SHC_Json()
    {
      //### Prepare ######################################################
      string SmartHealthCardCovidJson = Utf8EncodingSupport.GetString(ResourceData.SmartHealthCardCovidExample);

      //### Act ##########################################################
      
      SmartHealthCardModel SmartHealthCardModel = SmartHealthCardModelJsonSerializer.FromJson(SmartHealthCardCovidJson);
      
      //### Assert #######################################################

      Assert.Equal("1621444043.769", SmartHealthCardModel.IssuanceDate);
      Assert.Equal(new Uri("https://smarthealth.cards/examples/issuer"), SmartHealthCardModel.Issuer);
      Assert.NotNull(SmartHealthCardModel.VerifiableCredential);
      Assert.NotNull(SmartHealthCardModel.VerifiableCredential.VerifiableCredentialTypeList);
      Assert.Single(SmartHealthCardModel.VerifiableCredential.VerifiableCredentialTypeList);
      Assert.Equal(VerifiableCredentialType.Covid19, SmartHealthCardModel.VerifiableCredential.VerifiableCredentialTypeList[0]);
      Assert.NotNull(SmartHealthCardModel.VerifiableCredential.CredentialSubject);
      Assert.Equal("4.0.1", SmartHealthCardModel.VerifiableCredential.CredentialSubject.FhirVersion);
      Assert.NotNull(SmartHealthCardModel.VerifiableCredential.CredentialSubject.FhirBundle);

    }
  }
}
