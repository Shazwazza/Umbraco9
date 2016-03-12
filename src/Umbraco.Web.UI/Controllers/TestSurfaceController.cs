using System.Globalization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Umbraco.ViewModels;
using Umbraco.Web;
using Umbraco.Web.Controllers;

namespace Umbraco.Controllers
{
    public class TestSurfaceController : SurfaceController
    {
        public TestSurfaceController(UmbracoContext umbracoContext) : base(umbracoContext)
        {
        }

        public IActionResult DoThis(UmbracoFormTest model)
        {
            if (ModelState.IsValid == false)
            {
                this.ViewBag.message = "There were validation errors";
                return CurrentUmbracoPage();
            }

            ViewBag.message = "Success!";
            TempData["message"] = "YAY Success!";

            ModelState.SetModelValue("test",
                new ValueProviderResult(new StringValues(new[] { "YAY", "YAY" }), CultureInfo.CurrentCulture));

            return RedirectToCurrentUmbracoPage();
        }


    }
}