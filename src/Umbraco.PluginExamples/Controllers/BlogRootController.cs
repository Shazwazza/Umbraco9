using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Umbraco.PluginExamples.Models;
using Umbraco.Web;
using Umbraco.Web.Controllers;
using Umbraco.Web.Models;

namespace Umbraco.PluginExamples.Controllers
{
    public class BlogRootController : Controller, IUmbracoController
    {
        private readonly UmbracoContext _umbraco;
        private readonly UmbracoControllerHelper _controllerHelper;

        public BlogRootController(UmbracoContext umbraco, UmbracoControllerHelper controllerHelper)
        {
            _umbraco = umbraco;
            _controllerHelper = controllerHelper;
        }

        public ActionResult Index(IPublishedContent publishedContent)
        {
            return _controllerHelper.UmbracoViewForRoute(View, new TestModel
            {
                Message = "THIS IS SO COOL, I am a hijacked route",
                Name = "Blog root"
            });
        }

       
    }
}