using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Framework.DependencyInjection;

namespace Umbraco.Web.Controllers
{
    public class CrazyActionResult : IActionResult
    {
        private readonly ViewDataDictionary _vdd;

        public CrazyActionResult(ViewDataDictionary vdd)
        {
            _vdd = vdd;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var actionFactory = context.HttpContext.RequestServices
                .GetService<IActionInvokerFactory>();
            var providers = context.HttpContext.RequestServices
                .GetService<IActionDescriptorsCollectionProvider>();

            var actionDesc = providers.ActionDescriptors
                .Items
                .OfType<ControllerActionDescriptor>()
                .First(x => x.ControllerName == "Umbraco");

            var copied = new ActionContext(context.HttpContext, context.RouteData, actionDesc);
            copied.ModelState.Merge(context.ModelState);

            copied.HttpContext.Items["vdd"] = _vdd;

            await actionFactory.CreateInvoker(copied).InvokeAsync();
        }
    }
}