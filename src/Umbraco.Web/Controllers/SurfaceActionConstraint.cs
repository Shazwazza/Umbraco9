using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Controllers;

namespace Umbraco.Web.Controllers
{
    public class SurfaceActionConstraint : IActionConstraint
    {

        private readonly UmbracoContext _umbCtx;
        public SurfaceActionConstraint(UmbracoContext umbCtx)
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

            //NOTE: This was for testing at some point!

            //if (((ControllerActionDescriptor)context.CurrentCandidate.Action)
            //    .ControllerName == "TestSurface")
            //{
            //    return true;
            //}
            
            return false;
        }
    }
}