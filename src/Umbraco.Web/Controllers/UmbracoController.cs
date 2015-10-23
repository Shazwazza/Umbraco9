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

    //[UmbracoRouteConstraint]
    [UmbracoAction]
    [Route("{*_umbracoRoute:Required}")]
    //[Route("api/Company/{txtFile}", Name = "Company")]
    public class UmbracoController : Controller
    {
        private readonly FileContentContext _fileContent;

        public UmbracoController(FileContentContext fileContent)
        {
            _fileContent = fileContent;
        }
        
        public virtual string Index(string txtFile)
        {
            //if (this.ViewBag.something != "viewdata works")
            //{
            //    throw new Exception("NOPE");
            //}

            //return this.ModelState["test"].RawValue.ToString();

            if (string.IsNullOrEmpty(_fileContent.Content))
            {
                throw new Exception("No content");
            }
            return _fileContent.Content;
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