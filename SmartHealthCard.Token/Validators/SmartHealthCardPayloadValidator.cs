using SmartHealthCard.Token.JwsToken;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.Token.Support;

namespace SmartHealthCard.Token.Validators
{
  public class SmartHealthCardPayloadValidator : IJwsPayloadValidator
  {   
    public Result Validate<T>(T Obj)
    {
      if (Obj is SmartHealthCardModel SmartHealthCardModel)
      {
        //Validate the JWS Payload is a valid Smart Health Care JWS Payload
        return SmartHealthCardModel.Validate();
      }
      else
      {
        throw new System.InvalidCastException($"{typeof(SmartHealthCardPayloadValidator).Name} can only validate objects of type {typeof(SmartHealthCardModel).Name}");
      }
    }

  }
}
