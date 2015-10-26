using Microsoft.AspNet.Mvc;
using Umbraco.Web.Models;

namespace Umbraco.Web.Controllers
{
    public class CoolController : UmbracoController
    {
        public CoolController(UmbracoContext umbraco) : base(umbraco)
        {
        }

        public override ActionResult Index(IPublishedContent publishedContent)
        {
            return Content("THIS IS SO COOL");
        }

       
    }
}