using Newtonsoft.Json;
using SmartHealthCard.Token.Exceptions;

namespace SmartHealthCard.Token.Model.Shc
{
  public class SmartHealthCareJWSHeaderModel
  {
    public SmartHealthCareJWSHeaderModel(string Alg, string Zip, string Kid)
    {
      this.Alg = Alg;
      this.Zip = Zip;
      this.Kid = Kid;
    }

    [JsonProperty("alg", Required = Required.Always)]
    public string Alg { get; set; }

    [JsonProperty("zip", Required = Required.Always)]
    public string Zip { get; set; }

    [JsonProperty("kid", Required = Required.Always)]
    public string Kid { get; set; }

    internal void Validate()
    {      
      this.ValidateAlg();
      this.ValidateZip(); 
      this.ValidateKid();      
    }

    private void ValidateAlg()
    {
      string ExpectedAlg = "ES256";
      if (string.IsNullOrWhiteSpace(this.Alg))
      {
        throw new SmartHealthCardHeaderException("The Algorythm (alg) property was empty.");
      }
      if (!this.Alg.ToUpper().Equals(ExpectedAlg))
      {
        throw new SmartHealthCardHeaderException($"For Smart Health Cards the JWS header Algorythm (alg) property must be '{ExpectedAlg}', yet found {this.Alg}.");
      }
    }

    private void ValidateZip()
    {
      string ExpectedZip = "DEF";
      if (string.IsNullOrWhiteSpace(this.Zip))
      {
        throw new SmartHealthCardHeaderException("The Algorythm (alg) property was empty.");
      }
      if (!this.Zip.ToUpper().Equals(ExpectedZip))
      {
        throw new SmartHealthCardHeaderException($"For Smart Health Cards the JWS header Compression (zip) property must be '{ExpectedZip}', yet found {this.Zip}.");
      }      
    }

    private void ValidateKid()
    {      
      if (string.IsNullOrWhiteSpace(this.Kid))
      {
        throw new SmartHealthCardHeaderException("The JWK Thumbprint of the key (kid) property was empty.");
      }      
    }


  }
}
