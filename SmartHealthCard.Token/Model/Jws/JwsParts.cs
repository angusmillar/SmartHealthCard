using System;
using SmartHealthCard.Token.Enums;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Token.Model.Jws
{  
  public class JwsParts
  {
    public JwsParts(string[] Parts)
    {
      if (Parts is null)
        throw new ArgumentNullException(nameof(Parts));
      if (Parts.Length != 3)
        throw new InvalidTokenPartsException($"{nameof(Parts)}: A JWS Token must have three parts separated by dots (e.g Header.Payload.Signature).");      
      this.Parts = Parts;
    }

    public static Result<JwsParts> ParseToken(string Token)
    {
      if (string.IsNullOrEmpty(Token))
        return Result<JwsParts>.Fail("The provided Token was found to be null or empty.");

      var parts = Token.Split('.');
      if (parts.Length != 3)
        return Result<JwsParts>.Fail($"{nameof(Token)}: A JWS Token must have three parts separated by dots (e.g Header.Payload.Signature).");        

      return Result<JwsParts>.Ok(new JwsParts(parts));
    }

    public string Header => this.Parts[(int)JwtPartsIndex.Header];
   
    public string Payload => this.Parts[(int)JwtPartsIndex.Payload];
    
    public string Signature => this.Parts[(int)JwtPartsIndex.Signature];
    
    public string[] Parts { get; private set; }


  }
}