using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Filters;
using Umbraco.Web.Models;
using Umbraco.Web.Routing;

namespace Umbraco.Web.Controllers
{
    /// <summary>
    /// The default Umbraco rendering controller (i.e. previously RenderMvcController)
    /// </summary>
    public class UmbracoController : Controller, IUmbracoController
    {
        private readonly UmbracoContext _umbraco;
        private UmbracoControllerHelper _controllerHelper;
        protected UmbracoControllerHelper ControllerHelper 
            => _controllerHelper ?? (_controllerHelper = new UmbracoControllerHelper(_umbraco));

        public UmbracoController(UmbracoContext umbraco)
        {
            _umbraco = umbraco;
        }
                
        public virtual Task<ActionResult> Index(IPublishedContent publishedContent)
        {         
            if (_umbraco.PublishedContent == null)
            {
                throw new Exception("No content");
            }

            return Task.FromResult(UmbracoViewForRoute());
        }

        protected ActionResult UmbracoViewForRoute()
        {
            //TODO: Need view engine
            return ControllerHelper.UmbracoViewForRoute(View);
        }
    }
    
}