using System.Globalization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.Primitives;
using Umbraco.ViewModels;
using Umbraco.Web.Controllers;

namespace Umbraco.Controllers
{
    public class TestSurfaceController : SurfaceController
    {
        public IActionResult DoThis(UmbracoFormTest model)
        {
            if (ModelState.IsValid == false)
            {
                this.ViewBag.warning = "This is a warning from view data!";
                return CurrentUmbracoPage();
            }

            this.ViewBag.warning = "Success!";

            ModelState.SetModelValue("test",
                new ValueProviderResult(new StringValues(new [] { "YAY", "YAY" }), CultureInfo.CurrentCulture));

            return CurrentUmbracoPage();
        }
    }
}