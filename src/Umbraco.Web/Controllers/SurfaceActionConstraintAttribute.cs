using System;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.DependencyInjection;

namespace Umbraco.Web.Controllers
{
    public class SurfaceActionConstraintAttribute : Attribute, IActionConstraintFactory
    {
        
        public IActionConstraint CreateInstance(IServiceProvider services)
        {
            var umbCtx = services.GetRequiredService<UmbracoContext>();            
            return new SurfaceActionConstraint(umbCtx);
        }
    }
}