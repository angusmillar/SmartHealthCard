using Newtonsoft.Json;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Support;

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

    internal Result Validate()
    {
      Result ValidateAlgResult = this.ValidateAlg();
      Result ValidateZipResult = this.ValidateZip();
      Result ValidateKidResult = this.ValidateKid();

      return Result.Combine(ValidateAlgResult, ValidateZipResult, ValidateKidResult);      
    }

    private Result ValidateAlg()
    {
      string ExpectedAlg = "ES256";
      if (string.IsNullOrWhiteSpace(this.Alg))
      {
        return Result.Fail("The Algorithm (alg) property was empty.");        
      }
      if (!this.Alg.ToUpper().Equals(ExpectedAlg))
      {
        return Result.Fail($"For Smart Health Cards the JWS header Algorithm (alg) property must be '{ExpectedAlg}', yet found {this.Alg}.");       
      }
      return Result.Ok();
    }

    private Result ValidateZip()
    {
      string ExpectedZip = "DEF";
      if (string.IsNullOrWhiteSpace(this.Zip))
      {
        return Result.Fail("The Compression (zip) property was empty.");        
      }
      if (!this.Zip.ToUpper().Equals(ExpectedZip))
      {
        return Result.Fail($"For Smart Health Cards the JWS header Compression (zip) property must be '{ExpectedZip}', yet found {this.Zip}.");        
      }
      return Result.Ok();
    }

    private Result ValidateKid()
    {      
      if (string.IsNullOrWhiteSpace(this.Kid))
      {
        return Result.Fail("The JWK Thumb-print of the key (kid) property was empty.");        
      }
      return Result.Ok();
    }


  }
}
