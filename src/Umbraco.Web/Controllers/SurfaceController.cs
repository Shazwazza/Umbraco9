using Microsoft.AspNet.Mvc;
using Umbraco.Web.Routing;

namespace Umbraco.Web.Controllers
{
    
    public abstract class SurfaceController : Controller
    {
        //TODO: Create other methods like ReturnToCurrentUmbracoPage, etc...

        protected IActionResult CurrentUmbracoPage()
        {
            return new ProxyControllerActionResult(ViewData);
        }
    }
}