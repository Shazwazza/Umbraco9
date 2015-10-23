using Microsoft.AspNet.Mvc;

namespace Umbraco.Web.Controllers
{
    public class CoolController : UmbracoController
    {
        public CoolController(UmbracoContext umbraco) : base(umbraco)
        {
        }

        public override ActionResult Index(string path)
        {
            return Content("THIS IS SO COOL " + path);
        }

       
    }
}