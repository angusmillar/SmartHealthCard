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
        throw new InvalidTokenException($"{nameof(Token)}: A JWS Token must have three parts separated by dots (e.g Header.Payload.Signature).");

      this.Parts = parts;
    }
    
    public JwsParts(string[] Parts)
    {
      if (Parts is null)
        throw new ArgumentNullException(nameof(Parts));
      if (Parts.Length != 3)
        throw new InvalidTokenException($"{nameof(Parts)}: A JWS Token must have three parts separated by dots (e.g Header.Payload.Signature).");

      this.Parts = Parts;
    }
   
    public string Header => this.Parts[(int)JwtPartsIndex.Header];
   
    public string Payload => this.Parts[(int)JwtPartsIndex.Payload];
    
    public string Signature => this.Parts[(int)JwtPartsIndex.Signature];
    
    public string[] Parts { get; }


  }
}