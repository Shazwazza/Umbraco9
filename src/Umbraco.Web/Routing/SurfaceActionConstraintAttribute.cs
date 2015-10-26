using System;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.Framework.DependencyInjection;

namespace Umbraco.Web.Routing
{
    public class SurfaceActionConstraintAttribute : Attribute, IActionConstraintFactory
    {
        
        public IActionConstraint CreateInstance(IServiceProvider services)
        {
            var umbCtx = services.GetRequiredService<UmbracoContext>();
            var pcr = services.GetRequiredService<PublishedContentRequest>();
            return new SurfaceActionConstraint(umbCtx, pcr);
        }
    }
}