using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web;
using Umbraco.Web.Controllers;

namespace Umbraco.PluginExamples
{
    
    public class ExamplePluginSurfaceController : SurfaceController
    {
        public ExamplePluginSurfaceController(UmbracoContext umbracoContext) : base(umbracoContext)
        {
        }
    }
}
