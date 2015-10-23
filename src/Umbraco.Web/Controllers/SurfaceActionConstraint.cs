using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Controllers;

namespace Umbraco.Web.Controllers
{
    public class SurfaceActionConstraint : IActionConstraint
    {

        private string _basePath;
        public SurfaceActionConstraint(string basePath)
        {
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

            ////This would be like looking up content in Umbraco
            //var path = context.RouteContext.RouteData.Values["_umbracoRoute"] + ".txt";

            //var filePath = Path.Combine(_basePath, "Content", path);

            //if (!File.Exists(filePath))
            //{
            //    return false;
            //}

            //_fileContent.SetValue(File.ReadAllText(filePath));

            //NOTE: This was for testing at some point!

            //if (((ControllerActionDescriptor)context.CurrentCandidate.Action)
            //    .ControllerName == "TestSurface")
            //{
            //    return true;
            //}

            ////Is this a POST
            //if (context.RouteContext.HttpContext.Request.Method == "POST")
            //{
            //    if (((ControllerActionDescriptor)context.CurrentCandidate.Action)
            //        .ControllerName == "TestSurface")
            //    {
            //        return true;
            //    }
            //}


            return false;
        }
    }
}