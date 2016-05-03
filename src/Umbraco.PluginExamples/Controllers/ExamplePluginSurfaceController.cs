using Umbraco.Web;
using Umbraco.Web.Controllers;

namespace Umbraco.PluginExamples.Controllers
{
    

    public class ExamplePluginSurfaceController : SurfaceController
    {
        public ExamplePluginSurfaceController(UmbracoContext umbracoContext) : base(umbracoContext)
        {
        }
    }
}
