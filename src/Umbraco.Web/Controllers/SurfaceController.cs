using Microsoft.AspNet.Mvc;

namespace Umbraco.Web.Controllers
{
    
    [SurfaceActionConstraint]
    [Route("{*_surfaceRoute:Required}")]
    public abstract class SurfaceController : Controller
    {

    }
}