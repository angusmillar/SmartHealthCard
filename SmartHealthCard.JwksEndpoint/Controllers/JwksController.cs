using Microsoft.AspNetCore.Mvc;
using SmartHealthCard.JwksEndpoint.CertificateSupport;
using SmartHealthCard.JwksEndpoint.JwksSupport;

namespace SmartHealthCard.JwksEndpoint.Controllers
{
  [Route(".well-known/jwks.json")]
  [ApiController]  
  public class JwksController : ControllerBase
  {
    private readonly IJwksJsonProvider CertificateProvider;
    public JwksController(IJwksJsonProvider CertificateProvider)
    {
      this.CertificateProvider = CertificateProvider;
    }

    // GET: .well-known/jwks.json
    [HttpGet]
    [Produces("application/json")]
    public ActionResult Get()
    {
      try
      {
        return Content(CertificateProvider.GetJwksJson(), "application/json");
      }
      catch(CertificateLoadException CertificateLoadException)
      {
        return BadRequest(new ErrorOutcome("CertificateLoadError", CertificateLoadException.Message));
      }      
    }  
    
  } 
}
