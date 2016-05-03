using System;
using Microsoft.AspNet.Mvc;

namespace Umbraco.Web.Controllers
{
    public class UmbracoControllerHelper
    {
        private readonly UmbracoContext _umbracoContext;

        public UmbracoControllerHelper(UmbracoContext umbracoContext)
        {
            _umbracoContext = umbracoContext;
        }

        public ActionResult UmbracoViewForRoute(Func<string, object, ActionResult> view, object model = null)
        {
            //TODO: Need view engine
            return view("~/Views/" + _umbracoContext.PublishedContentRequest.TemplateAlias + ".cshtml", 
                model ?? _umbracoContext.PublishedContent);
        }
    }
}