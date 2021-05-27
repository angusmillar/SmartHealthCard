using SmartHealthCard.Token.Algorithms;
using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Jws;
using System;

namespace SmartHealthCard.Token.JwsToken
{
  public sealed class JwsSignatureValidator : IJwsSignatureValidator
  {    
    public void Validate(IAlgorithm Algorithm, string Token)
    {
      if (string.IsNullOrEmpty(Token))      
        throw new ArgumentException($"The provided {nameof(Token)} was found to be empty.");
      
      JwsParts JwtParts = new JwsParts(Token);
      byte[] BytesToSign = Utf8EncodingSupport.GetBytes(JwtParts.Header, (byte)'.', JwtParts.Payload);
      byte[] Signature = Base64UrlEncoder.Decode(JwtParts.Signature);
      if (!Algorithm.Verify(BytesToSign, Signature))
      {
        throw new SignatureVerificationException("The JWS signature is invalid.");
      }     
    }
  }
}
