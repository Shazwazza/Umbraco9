using Microsoft.AspNet.Mvc.ActionConstraints;

namespace Umbraco.Web.Routing
{
    public class SurfaceActionConstraint : IActionConstraint
    {

        private readonly UmbracoContext _umbCtx;
        private readonly PublishedContentRequest _pcr;

        public SurfaceActionConstraint(UmbracoContext umbCtx, PublishedContentRequest pcr)
        {
            _umbCtx = umbCtx;
            _pcr = pcr;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {


            //Initialize the context, this will be called a few times but the initialize logic
            // only executes once. There might be a nicer way to do this but the RouteContext and 
            // other request scoped instances are not available yet.
            _umbCtx.Initialize(_pcr);

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