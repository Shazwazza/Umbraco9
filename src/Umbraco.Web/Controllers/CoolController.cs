namespace Umbraco.Web.Controllers
{
    public class CoolController : UmbracoController
    {
        public CoolController(FileContentContext fileContent) : base(fileContent)
        {
        }

        public override string Index(string txtFile)
        {
            return "THIS IS SO COOL " + txtFile;
        }

       
    }
}