using System.Globalization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.Primitives;

namespace Umbraco.Web.Controllers
{
    public class TestSurfaceController : SurfaceController
    {

        
        public IActionResult DoThis()
        {
            this.ViewBag.something = "viewdata works";

            ModelState.SetModelValue("test",
                new ValueProviderResult(new StringValues(new [] { "YAY", "YAY" }), CultureInfo.CurrentCulture));

            return new CrazyActionResult(ViewData);
        }
    }
}