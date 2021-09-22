using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Jws;
using SmartHealthCard.Token.Support;
using System;

namespace SmartHealthCard.Token.JwsToken
{
  public sealed class JwsSignatureValidator : IJwsSignatureValidator
  {    
    public Result<bool> Validate(IAlgorithm Algorithm, string Token)
    {
      if (string.IsNullOrEmpty(Token))
        return Result<bool>.Fail("The provided Token was found to be null or empty.");
 
      Result<JwsParts> JwtPartsParseResult = JwsParts.ParseToken(Token);
      if (JwtPartsParseResult.Failure)
        return Result<bool>.Fail(JwtPartsParseResult.ErrorMessage);

      byte[] BytesToSign = Utf8EncodingSupport.GetBytes(JwtPartsParseResult.Value.Header, (byte)'.', JwtPartsParseResult.Value.Payload);
      byte[] Signature = Base64UrlEncoder.Decode(JwtPartsParseResult.Value.Signature);
      Result<bool> VerifyResult = Algorithm.Verify(BytesToSign, Signature);
      if (VerifyResult.Success)
      {
        //Return The JWS's signature validity boolean
        return Result<bool>.Ok(VerifyResult.Value);         
      }
      else
      {
        return Result<bool>.Fail($"Error attempting to validate the JWS signing signature. The signature validity could not be determined. {VerifyResult.ErrorMessage}");
      }      
    }
  }
}
