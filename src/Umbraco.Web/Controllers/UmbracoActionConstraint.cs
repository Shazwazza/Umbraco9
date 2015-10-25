using System;
using System.IO;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Controllers;

namespace Umbraco.Web.Controllers
{
    /// <summary>
    /// This checks the current route to see if it is an Umbraco route
    /// </summary>
    /// <remarks>
    /// This is also the process that initializes the UmbracoContext
    /// </remarks>
    public class UmbracoActionConstraint : IActionConstraint
    {

        private readonly UmbracoContext _umbCtx;
        public UmbracoActionConstraint(UmbracoContext umbCtx)
        {
            _umbCtx = umbCtx;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            //Initialize the context, this will be called a few times but the initialize logic
            // only executes once. There might be a nicer way to do this but the RouteContext and 
            // other request scoped instances are not available yet.
            _umbCtx.Initialize(context.RouteContext.RouteData);

            if (_umbCtx.HasContent == false) return false;
            
            //Is this a POST
            if (context.RouteContext.HttpContext.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                if (((ControllerActionDescriptor)context.CurrentCandidate.Action).ControllerName == "TestSurface")
                {
                    return true;
                }
            }

            ////NOTE: This get's bound currently
            //context.RouteContext.RouteData.Values["txtFile"] = filePath;
            
            //string actionNameRequest =
            //    context.RouteContext.HttpContext.Request.Query["actionName"] ??
            //"Index";

            //object controllerNameFound;
            //if (context.CurrentCandidate.Action.Properties.TryGetValue("controllerName", out controllerNameFound))
            //{
            //    if ((string)controllerNameFound == controllerNameRequest)
            //    {
            //        return true;
            //    }
            //}

            //OR You could do this:
            if (((ControllerActionDescriptor)context.CurrentCandidate.Action).ControllerName == _umbCtx.AltTemplate)
            {
                return true;
            }

            return false;
        }
    }
}