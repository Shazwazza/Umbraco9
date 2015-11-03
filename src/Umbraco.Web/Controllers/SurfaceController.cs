using System;
using System.Collections.Specialized;
using Microsoft.AspNet.Mvc;
using Umbraco.Web.ActionResults;
using Umbraco.Web.Models;
using Umbraco.Web.Routing;

namespace Umbraco.Web.Controllers
{
    
    public abstract class SurfaceController : Controller
    {
        protected SurfaceController(UmbracoContext umbracoContext)
        {
            UmbracoContext = umbracoContext;
        }

        public UmbracoContext UmbracoContext { get; }
        public IPublishedContent CurrentPage => UmbracoContext.PublishedContent;

        ///// <summary>
        ///// Redirects to the Umbraco page with the given id and passes provided querystring
        ///// </summary>
        ///// <param name="pageId"></param>
        ///// <param name="queryStringValues"></param>
        ///// <returns></returns>
        //protected RedirectToUmbracoPageResult RedirectToUmbracoPage(Guid pageId, NameValueCollection queryStringValues)
        //{
        //    return new RedirectToUmbracoPageResult(pageId, queryStringValues, UmbracoContext);
        //}

        ///// <summary>
        ///// Redirects to the Umbraco page with the given id and passes provided querystring
        ///// </summary>
        ///// <param name="pageId"></param>
        ///// <param name="queryString"></param>
        ///// <returns></returns>
        //protected RedirectToUmbracoPageResult RedirectToUmbracoPage(Guid pageId, string queryString)
        //{
        //    return new RedirectToUmbracoPageResult(pageId, queryString, UmbracoContext);
        //}

        ///// <summary>
        ///// Redirects to the Umbraco page with the given id
        ///// </summary>
        ///// <param name="publishedContent"></param>
        ///// <returns></returns>
        //protected RedirectToUmbracoPageResult RedirectToUmbracoPage(IPublishedContent publishedContent)
        //{
        //    return new RedirectToUmbracoPageResult(publishedContent, UmbracoContext);
        //}

        ///// <summary>
        ///// Redirects to the Umbraco page with the given id and passes provided querystring
        ///// </summary>
        ///// <param name="publishedContent"></param>
        ///// <param name="queryStringValues"></param>
        ///// <returns></returns>
        //protected RedirectToUmbracoPageResult RedirectToUmbracoPage(IPublishedContent publishedContent, NameValueCollection queryStringValues)
        //{
        //    return new RedirectToUmbracoPageResult(publishedContent, queryStringValues, UmbracoContext);
        //}

        ///// <summary>
        ///// Redirects to the Umbraco page with the given id and passes provided querystring
        ///// </summary>
        ///// <param name="publishedContent"></param>
        ///// <param name="queryString"></param>
        ///// <returns></returns>
        //protected RedirectToUmbracoPageResult RedirectToUmbracoPage(IPublishedContent publishedContent, string queryString)
        //{
        //    return new RedirectToUmbracoPageResult(publishedContent, queryString, UmbracoContext);
        //}

        /// <summary>
        /// Redirects to the currently rendered Umbraco page
        /// </summary>
        /// <returns></returns>
        protected RedirectToUmbracoPageResult RedirectToCurrentUmbracoPage()
        {
            return new RedirectToUmbracoPageResult(UmbracoContext);
        }

        ///// <summary>
        ///// Redirects to the currently rendered Umbraco page and passes provided querystring
        ///// </summary>
        ///// <param name="queryStringValues"></param>
        ///// <returns></returns>
        //protected RedirectToUmbracoPageResult RedirectToCurrentUmbracoPage(NameValueCollection queryStringValues)
        //{
        //    return new RedirectToUmbracoPageResult(CurrentPage, queryStringValues, UmbracoContext);
        //}

        ///// <summary>
        ///// Redirects to the currently rendered Umbraco page and passes provided querystring
        ///// </summary>
        ///// <param name="queryStringValues"></param>
        ///// <returns></returns>
        //protected RedirectToUmbracoPageResult RedirectToCurrentUmbracoPage(string queryString)
        //{
        //    return new RedirectToUmbracoPageResult(CurrentPage, queryString, UmbracoContext);
        //}

        ///// <summary>
        ///// Redirects to the currently rendered Umbraco URL
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks>
        ///// this is useful if you need to redirect 
        ///// to the current page but the current page is actually a rewritten URL normally done with something like 
        ///// Server.Transfer.
        ///// </remarks>
        //protected RedirectToUmbracoUrlResult RedirectToCurrentUmbracoUrl()
        //{
        //    return new RedirectToUmbracoUrlResult(UmbracoContext);
        //}

        /// <summary>
        /// Returns the currently rendered Umbraco page
        /// </summary>
        /// <returns></returns>
        protected IActionResult CurrentUmbracoPage()
        {
            return new ProxyControllerActionResult(ViewData);
        }
    }
}