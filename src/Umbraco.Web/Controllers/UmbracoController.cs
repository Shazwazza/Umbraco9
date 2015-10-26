using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Filters;
using Umbraco.Web.Models;
using Umbraco.Web.Routing;

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
    //[UmbracoActionConstraint]
    public class UmbracoController : Controller
    {
        private readonly UmbracoContext _umbraco;

        public UmbracoController(UmbracoContext umbraco)
        {
            _umbraco = umbraco;
        }
                
        public virtual ActionResult Index(IPublishedContent publishedContent)
        {
            //if (this.ViewBag.something != "viewdata works")
            //{
            //    throw new Exception("NOPE");
            //}

            //return this.ModelState["test"].RawValue.ToString();

            if (_umbraco.PublishedContent == null)
            {
                throw new Exception("No content");
            }

            return UmbracoViewForRoute(publishedContent);
        }

        protected ActionResult UmbracoViewForRoute(IPublishedContent publishedContent)
        {
            //TODO: Need view engine
            return View("~/Views/" + _umbraco.PublishedContentRequest.TemplateAlias + ".cshtml", _umbraco.PublishedContent);
        }
    }

    ////TODO: Do this for custom umbraco routes
    //[CustomLookoupConstraintFactory]
    //[Route("products/{sku}")]
    //public class CustomRouteController
    //{

    //}
}