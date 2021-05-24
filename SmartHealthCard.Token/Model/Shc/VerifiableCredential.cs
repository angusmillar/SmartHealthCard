using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.Model.Shc
{
  public class VerifiableCredential
  {
    public VerifiableCredential(List<Uri> VerifiableCredentialTypeList, CredentialSubject CredentialSubject)
    {
      this.VerifiableCredentialTypeList = VerifiableCredentialTypeList;
      this.CredentialSubject = CredentialSubject;
    }

    [JsonProperty("type")] 
    public List<Uri> VerifiableCredentialTypeList { get; set; }
    [JsonProperty("credentialSubject")]
    public CredentialSubject CredentialSubject { get; set; }
    
  }
}
