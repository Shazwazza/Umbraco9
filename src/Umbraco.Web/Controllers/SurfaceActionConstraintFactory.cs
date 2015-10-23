using System;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.DependencyInjection;

namespace Umbraco.Web.Controllers
{
    public class SurfaceActionConstraintFactory : Attribute, IActionConstraintFactory
    {
        
        public IActionConstraint CreateInstance(IServiceProvider services)
        {
            var appEnv = services.GetRequiredService<IApplicationEnvironment>();
            var hostEnv = services.GetRequiredService<IHostingEnvironment>();

            ////don't do this later!
            //var fileContent = services.GetRequiredService<IContextAccessor<string>>();

            return new SurfaceActionConstraint(hostEnv.WebRootPath);
        }
    }
}