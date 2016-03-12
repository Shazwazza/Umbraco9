using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Core;
using Umbraco.Web.Controllers;
using Umbraco.Web.Models;

namespace Umbraco.Web.Routing
{
    /// <summary>
    /// The router that performs all umbraco content lookups based on the current request url/path
    /// </summary>
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
            if (newRouteData.Values.ContainsKey("_umbracoRoute") 
                //exit quickly if there's any file extension
                && newRouteData.Values["_umbracoRoute"].ToString().Contains(".") == false)
            {
                var umbCtx = context.HttpContext.RequestServices.GetRequiredService<UmbracoContext>();
                var umbControllerTypes = context.HttpContext.ApplicationServices.GetRequiredService<UmbracoControllerTypeCollection>();
                var pcr = context.HttpContext.RequestServices.GetRequiredService<PublishedContentRequest>();
                
                if (await RouteUmbracoContentAsync(umbCtx, pcr, newRouteData))
                {
                    var routeDef = GetUmbracoRouteValues(umbCtx, umbControllerTypes);
                    newRouteData.DataTokens["umbraco-route-def"] = routeDef;
                    var surfaceFormHelper = context.HttpContext.ApplicationServices.GetRequiredService<SurfaceFormHelper>();
                    var formInfo = surfaceFormHelper.GetFormInfo(context);
                    if (formInfo != null)
                    {
                        //there is form data, route to the surface controller
                        SetUmbracoRouteValues(formInfo, newRouteData);
                    }
                    else
                    {
                        //there is no form data, route normally
                        SetUmbracoRouteValues(routeDef, newRouteData);
                    }
                }
            }            

            await ExecuteNext(context, newRouteData, oldRouteData);
        }
       
        internal async Task<bool> RouteUmbracoContentAsync(UmbracoContext umbCtx, PublishedContentRequest pcr, RouteData routeData)
        {
            //Initialize the context, this will be called a few times but the initialize logic
            // only executes once. There might be a nicer way to do this but the RouteContext and 
            // other request scoped instances are not available yet.
            umbCtx.Initialize(pcr);

            //Prepare the request if it hasn't already been done
            if (pcr.IsPrepared == false)
            {                
                if (await pcr.PrepareAsync(routeData))
                {
                    if (umbCtx.HasContent == false) return false;
                }
            }
            return umbCtx.HasContent;            
        }

        internal RouteDefinition GetUmbracoRouteValues(UmbracoContext umbCtx, UmbracoControllerTypeCollection umbControllerTypes)
        {
            if (umbCtx.PublishedContent == null) throw new ArgumentNullException(nameof(umbCtx) + ".PublishedContent");

            //Let's match controller names:        
            var found = umbControllerTypes.GetControllerName(umbCtx.PublishedContent.ContentType);
            //check if there are actually any controllers that match the content type
            if (found.IsNullOrWhiteSpace() == false)
            {
                return new RouteDefinition()
                {
                    ControllerName = found,
                    ActionName = umbControllerTypes.GetControllerActionName(found, umbCtx.PublishedContentRequest.TemplateAlias)
                };
            }
            //There is no controller type registered for this content type, so we'll match if this is the default
            return new RouteDefinition()
            {
                ControllerName = "Umbraco",
                ActionName = "Index"
            };
        }

        private void SetUmbracoRouteValues(RouteDefinition routeDef, RouteData routeData)
        {
            routeData.Values["controller"] = routeDef.ControllerName;
            routeData.Values["action"] = routeDef.ActionName;
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