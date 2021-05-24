using System;
using SmartHealthCard.Token.Enums;
using SmartHealthCard.Token.Exceptions;


namespace SmartHealthCard.Token.Model.Jws
{  
  public class JwsParts
  {
    public JwsParts(string token)
    {
      if (string.IsNullOrWhiteSpace(token))
      {
        throw new ArgumentException(nameof(token));
      }

      var parts = token.Split('.');
      if (parts.Length != 3)
        throw new InvalidTokenPartsException(nameof(token));

      this.Parts = parts;
    }
    
    public JwsParts(string[] parts)
    {
      if (parts is null)
        throw new ArgumentNullException(nameof(parts));
      if (parts.Length != 3)
        throw new InvalidTokenPartsException(nameof(parts));

      this.Parts = parts;
    }
   
    public string Header => this.Parts[(int)JwtPartsIndex.Header];
   
    public string Payload => this.Parts[(int)JwtPartsIndex.Payload];
    
    public string Signature => this.Parts[(int)JwtPartsIndex.Signature];
    
    public string[] Parts { get; }


  }
}