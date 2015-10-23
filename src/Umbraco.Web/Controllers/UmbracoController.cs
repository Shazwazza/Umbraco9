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

        //TODO: need to model bind the 'Content' item
        public virtual ActionResult Index(string path)
        {
            //if (this.ViewBag.something != "viewdata works")
            //{
            //    throw new Exception("NOPE");
            //}

            //return this.ModelState["test"].RawValue.ToString();

            if (_umbraco.Content == null)
            {
                throw new Exception("No content");
            }

            //TODO: Need view engine
            return View("~/Views/" + _umbraco.Content.View + ".cshtml", _umbraco.Content);
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