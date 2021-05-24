using SmartHealthCard.Token.JwsToken;
using SmartHealthCard.Token.Model.Shc;

namespace SmartHealthCard.Token
{
  public class SmartHealthCardHeaderValidator : IJwsHeaderValidator
  {   
    public void Validate<T>(T Obj)
    {
      if (Obj is SmartHealthCareJWSHeaderModel SmartHealthCareJWSHeaderModel)
      {
        //Validate the JWS Header is a valid Smart Health Care JWS Header
        SmartHealthCareJWSHeaderModel.Validate();
      }     
    }

  }




}
