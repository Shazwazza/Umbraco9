using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Umbraco.Web.Models;
using Umbraco.Core;

namespace Umbraco.Web.ActionResults
{
    /// <summary>
	/// Redirects to an Umbraco page by Id or Entity
	/// </summary>
	public class RedirectToUmbracoPageResult : ActionResult, IKeepTempDataResult
    {

        public RedirectToUmbracoPageResult(UmbracoContext umbracoContext)
        {
            UmbracoContext = umbracoContext;
            PublishedContent = UmbracoContext.PublishedContent;            
        }
         
        private string _url;
        private RedirectResult _wrapped;
        public UmbracoContext UmbracoContext { get; }
        public IPublishedContent PublishedContent { get; }
        public string Url
        {
            get
            {
                if (!_url.IsNullOrWhiteSpace()) return _url;                

                var result = UmbracoContext.PublishedContentRequest.RoutingContext.UrlProvider.GetUrl(PublishedContent.Id);
                if (result != "#")
                {
                    _url = result;
                    return _url;
                }
                throw new InvalidOperationException($"Could not route to entity with id {PublishedContent.Id}, the NiceUrlProvider could not generate a URL");
            }
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            _wrapped = new RedirectResult(Url);
            _wrapped.Permanent = false;
            _wrapped.ExecuteResult(context);
        }
               
    }
}
