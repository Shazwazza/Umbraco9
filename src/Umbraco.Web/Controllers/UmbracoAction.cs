using System;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.DependencyInjection;

namespace Umbraco.Web.Controllers
{
    public class UmbracoAction : Attribute, IActionConstraintFactory
        //, IControllerModelConvention
        //,IActionModelConvention
    {
        //public void Apply(ActionModel model)
        //{            
        //    model.RouteConstraints.Clear();
        //    model.RouteConstraints.Add(new UmbracoRouteConstraintAttribute());
        //}

        //public void Apply(ControllerModel model)
        //{
        //    model.RouteConstraints.Clear();
        //    model.RouteConstraints.Add(new UmbracoRouteConstraintAttribute());
        //}

        public IActionConstraint CreateInstance(IServiceProvider services)
        {
            var appEnv = services.GetRequiredService<IApplicationEnvironment>();
            var hostEnv = services.GetRequiredService<IHostingEnvironment>();
            
            //don't do this later!
            var fileContent = services.GetRequiredService<FileContentContext>();

            return new UmbracoActionConstraint(hostEnv.WebRootPath, fileContent);
        }
    }
}