using System.IO;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Controllers;

namespace Umbraco.Web.Controllers
{
    public class UmbracoActionConstraint : IActionConstraint
    {

        private readonly UmbracoContext _umbCtx;
        public UmbracoActionConstraint(UmbracoContext umbCtx)
        {
            _umbCtx = umbCtx;
        }


        public int Order
        {
            get
            {
                return 0;
            }
        }

        public bool Accept(ActionConstraintContext context)
        {
            //Initialize the context, this will be called a few times but the initialize logic
            // only executes once. There might be a nicer way to do this but the RouteContext and 
            // other request scoped instances are not available yet.
            _umbCtx.Initialize(context.RouteContext);

            if (_umbCtx.HasContent == false) return false;
            
            //Is this a POST
            if (context.RouteContext.HttpContext.Request.Method == "POST")
            {
                if (((ControllerActionDescriptor)context.CurrentCandidate.Action)
                    .ControllerName == "TestSurface")
                {
                    return true;
                }
            }

            ////NOTE: This get's bound currently
            //context.RouteContext.RouteData.Values["txtFile"] = filePath;

            string altTemplate = context.RouteContext.HttpContext.Request.Query["altTemplate"];
            if (string.IsNullOrEmpty(altTemplate))
            {
                altTemplate = "Umbraco";
            }

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
            if (((ControllerActionDescriptor)context.CurrentCandidate.Action).ControllerName == altTemplate)
            {
                return true;
            }

            return false;
        }
    }
}