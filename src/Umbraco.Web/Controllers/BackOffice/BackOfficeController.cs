using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Umbraco.Web.Controllers.BackOffice
{
    [Area("Umbraco")]    
    public class BackOfficeController : Controller
    {
        public BackOfficeController()
        {
        }

        [Authorize(Policy = "umbraco-backoffice", ActiveAuthenticationSchemes = "umbraco-backoffice")]
        [HttpGet]
        [Route("[Area]")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("[Area]/Login")]
        public IActionResult Login(string returnUrl)
        {
            return View();
        }

        //[HttpPost]
        //[Route("[Area]/Login")]
        //public IActionResult Login(LoginModel model)
        //{
        //    return View();
        //}

    }
}
