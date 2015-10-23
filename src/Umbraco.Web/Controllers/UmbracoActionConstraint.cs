using System.IO;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Controllers;

namespace Umbraco.Web.Controllers
{
    public class UmbracoActionConstraint : IActionConstraint
    {

        private string _basePath;
        private FileContentContext _fileContent;
        public UmbracoActionConstraint(string basePath, FileContentContext fileContent)
        {
            _fileContent = fileContent;
            _basePath = basePath;
        }


        public int Order
        {
            get
            {
                return 0;
            }
        }

        public bool Accept(ActionConstraintContext context)
        {

            //This would be like looking up content in Umbraco
            var path = context.RouteContext.RouteData.Values["_umbracoRoute"] + ".txt";

            var filePath = Path.Combine(_basePath, "Content", path);

            if (!File.Exists(filePath))
            {
                return false;
            }

            _fileContent.Content = File.ReadAllText(filePath);


            //Is this a POST
            if (context.RouteContext.HttpContext.Request.Method == "POST")
            {
                if (((ControllerActionDescriptor)context.CurrentCandidate.Action)
                    .ControllerName == "TestSurface")
                {
                    return true;
                }
            }

            ////NOTE: This get's bound currently
            //context.RouteContext.RouteData.Values["txtFile"] = filePath;

            string altTemplate = context.RouteContext.HttpContext.Request.Query["altTemplate"];
            if (string.IsNullOrEmpty(altTemplate))
            {
                altTemplate = "Umbraco";
            }

            //string actionNameRequest =
            //    context.RouteContext.HttpContext.Request.Query["actionName"] ??
            //"Index";

            //object controllerNameFound;
            //if (context.CurrentCandidate.Action.Properties.TryGetValue("controllerName", out controllerNameFound))
            //{
            //    if ((string)controllerNameFound == controllerNameRequest)
            //    {
            //        return true;
            //    }
            //}

            //OR You could do this:
            if (((ControllerActionDescriptor)context.CurrentCandidate.Action).ControllerName == altTemplate)
            {
                return true;
            }

            return false;
        }
    }
}