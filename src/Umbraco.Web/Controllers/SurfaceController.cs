using Microsoft.AspNet.Mvc;

namespace Umbraco.Web.Controllers
{
    
    [SurfaceActionConstraint]
    [Route("{*_surfaceRoute:Required}")]
    public class SurfaceController : Controller
    {

    }
}