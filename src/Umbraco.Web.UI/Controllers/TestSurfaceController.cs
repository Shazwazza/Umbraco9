using System.Globalization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.Primitives;
using Umbraco.Web.Controllers;

namespace Umbraco.Controllers
{
    public class TestSurfaceController : SurfaceController
    {
        public IActionResult DoThis()
        {
            this.ViewBag.warning = "This is a warning from view data!";

            ModelState.SetModelValue("test",
                new ValueProviderResult(new StringValues(new [] { "YAY", "YAY" }), CultureInfo.CurrentCulture));

            return CurrentUmbracoPage();
        }
    }
}