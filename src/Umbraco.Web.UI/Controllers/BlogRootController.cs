using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Umbraco.Web;
using Umbraco.Web.Controllers;
using Umbraco.Web.Models;

namespace Umbraco.Controllers
{
    public class BlogRootController : UmbracoController
    {
        public BlogRootController(UmbracoContext umbraco) : base(umbraco)
        {
        }

        public override Task<ActionResult> Index(IPublishedContent publishedContent)
        {
            return Task.FromResult((ActionResult)Content("THIS IS SO COOL, I am a hijacked route"));
        }

       
    }
}