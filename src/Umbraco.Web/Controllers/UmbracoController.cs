using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Filters;

namespace Umbraco.Web.Controllers
{

    //public class UmbracoRouteConstraintAttribute :
    //    RouteDataActionConstraint
    ////RouteConstraintAttribute
    //{
    //    public UmbracoRouteConstraintAttribute()
    //        : base("umbraco", "yes")
    //    {

    //    }
    //}

    /// <summary>
    /// The default Umbraco rendering controller (i.e. previously RenderMvcController)
    /// </summary>
    //[UmbracoRouteConstraint]
    [UmbracoActionConstraint]
    [Route("{*_umbracoRoute:Required}")]
    public class UmbracoController : Controller
    {
        private readonly UmbracoContext _umbraco;

        public UmbracoController(UmbracoContext umbraco)
        {
            _umbraco = umbraco;
        }

        public virtual string Index(string txtFile)
        {
            //if (this.ViewBag.something != "viewdata works")
            //{
            //    throw new Exception("NOPE");
            //}

            //return this.ModelState["test"].RawValue.ToString();

            if (string.IsNullOrEmpty(_umbraco.Content))
            {
                throw new Exception("No content");
            }
            return _umbraco.Content;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }

    ////TODO: Do this for custom umbraco routes
    //[CustomLookoupConstraintFactory]
    //[Route("products/{sku}")]
    //public class CustomRouteController
    //{

    //}
}