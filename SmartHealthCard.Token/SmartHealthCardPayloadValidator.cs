using SmartHealthCard.Token.JwsToken;
using SmartHealthCard.Token.Model.Shc;

namespace SmartHealthCard.Token
{
  public class SmartHealthCardPayloadValidator : IJwsPayloadValidator
  {   
    public void Validate<T>(T Obj)
    {
      if (Obj is SmartHealthCardModel SmartHealthCardModel)
      {
        //Validate the JWS Payload is a valid Smart Health Care JWS Payload
        SmartHealthCardModel.Validate();
      }            
    }

  }
}
