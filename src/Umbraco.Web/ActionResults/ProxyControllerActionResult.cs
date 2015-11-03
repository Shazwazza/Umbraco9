using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Framework.DependencyInjection;
using Umbraco.Core;
using Umbraco.Web.Models;

namespace Umbraco.Web.ActionResults
{
    internal class ProxyControllerActionResult : IActionResult
    {
        private readonly ViewDataDictionary _vdd;

        public ProxyControllerActionResult(ViewDataDictionary vdd)
        {
            _vdd = vdd;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var routeDef = context.RouteData.DataTokens.ContainsKey("umbraco-route-def")
                ? context.RouteData.DataTokens["umbraco-route-def"] as RouteDefinition
                : null;
            if (routeDef == null)
                throw new InvalidOperationException($"The route data DataTokens must contain an instance of {typeof(RouteDefinition)} with a key of umbraco-route-def");

            var actionFactory = context.HttpContext.RequestServices
                .GetService<IActionInvokerFactory>();
            var providers = context.HttpContext.RequestServices
                .GetService<IActionDescriptorsCollectionProvider>();

            var actionDesc = providers.ActionDescriptors
                .Items
                .OfType<ControllerActionDescriptor>()
                .First(x => x.ControllerName.InvariantEquals(routeDef.ControllerName) && x.Name.InvariantEquals(routeDef.ActionName));

            var copied = new ActionContext(context.HttpContext, context.RouteData, actionDesc);
            copied.ModelState.Merge(context.ModelState);

            copied.RouteData.DataTokens["umbraco-vdd"] = _vdd;

            await actionFactory.CreateInvoker(copied).InvokeAsync();
        }
    }
}