using Microsoft.AspNet.Mvc;

namespace Umbraco.Web.Controllers
{
    
    [SurfaceActionConstraintFactory]
    [Route("{*_surfaceRoute:Required}")]
    public class SurfaceController : Controller
    {

    }
}