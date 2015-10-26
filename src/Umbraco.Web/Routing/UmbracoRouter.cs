using System.Threading.Tasks;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Umbraco.Core;
using Umbraco.Web.Controllers;

namespace Umbraco.Web.Routing
{
    public class UmbracoRouter : IRouter
    {
        private readonly IRouter _next;
        
        public UmbracoRouter(IRouter next)
        {
            _next = next;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            // We just want to act as a pass-through for link generation
            return _next.GetVirtualPath(context);
        }

        public async Task RouteAsync(RouteContext context)
        {
            // Saving and restoring the original route data ensures that any values we
            // add won't 'leak' if action selection doesn't match.
            var oldRouteData = context.RouteData;
            
            // For diagnostics and link-generation purposes, routing should include
            // a list of IRoute instances that lead to the ultimate destination.
            // It's the responsibility of each IRouter to add the 'next' before 
            // calling it.
            var newRouteData = new RouteData(oldRouteData);
            newRouteData.Routers.Add(_next);
            
            //It's an umbraco route, need to find out if it matches any content
            if (newRouteData.Values.ContainsKey("_umbracoRoute"))
            {
                var umbCtx = context.HttpContext.RequestServices.GetRequiredService<UmbracoContext>();
                var pcr = context.HttpContext.RequestServices.GetRequiredService<PublishedContentRequest>();
                if (await RouteUmbracoContentAsync(umbCtx, pcr, newRouteData))
                {
                    var umbControllerTypes = context.HttpContext.ApplicationServices.GetRequiredService<UmbracoControllerTypeCollection>();
                    SetUmbracoRouteValues(umbCtx, umbControllerTypes, newRouteData);
                }
            }
            
            await ExecuteNext(context, newRouteData, oldRouteData);
        }

        private async Task<bool> RouteUmbracoContentAsync(UmbracoContext umbCtx, PublishedContentRequest pcr, RouteData routeData)
        {
            //Initialize the context, this will be called a few times but the initialize logic
            // only executes once. There might be a nicer way to do this but the RouteContext and 
            // other request scoped instances are not available yet.
            umbCtx.Initialize(pcr);

            //Prepare the request if it hasn't already been done
            if (pcr.IsPrepared == false)
            {
                //TODO: See https://github.com/aspnet/Mvc/issues/3412
                // we want this method to be async
                //NOTE: We are using the GetAwaiter() syntax in order to get a real exception instead of an Aggregate exception
                if (await pcr.PrepareAsync(routeData))
                {
                    if (umbCtx.HasContent == false) return false;
                }
            }
            return umbCtx.HasContent;            
        }

        private void SetUmbracoRouteValues(UmbracoContext umbCtx, UmbracoControllerTypeCollection umbControllerTypes, RouteData newRouteData)
        {
            //Let's match controller names:        
            var found = umbControllerTypes.GetControllerName(umbCtx.PublishedContent.ContentType);
            //check if there are actually any controllers that match the content type
            if (found.IsNullOrWhiteSpace() == false)
            {
                newRouteData.Values["controller"] = found;
                //Set the a matching 
                newRouteData.Values["action"] = umbControllerTypes.GetControllerActionName(found, umbCtx.PublishedContentRequest.TemplateAlias);
            }
            else
            {
                //There is no controller type registered for this content type, so we'll match if this is the default
                newRouteData.Values["controller"] = "Umbraco";
                newRouteData.Values["action"] = "Index";
            }
        }

        private async Task ExecuteNext(RouteContext context, RouteData newRouteData, RouteData oldRouteData)
        {
            try
            {
                context.RouteData = newRouteData;
                await _next.RouteAsync(context);
            }
            finally
            {
                if (!context.IsHandled)
                {
                    context.RouteData = oldRouteData;
                }
            }
        }
        
    }
}