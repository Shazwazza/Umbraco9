namespace Umbraco.Web.Controllers
{
    public class CoolController : UmbracoController
    {
        public CoolController(UmbracoContext umbraco) : base(umbraco)
        {
        }

        public override string Index(string txtFile)
        {
            return "THIS IS SO COOL " + txtFile;
        }

       
    }
}