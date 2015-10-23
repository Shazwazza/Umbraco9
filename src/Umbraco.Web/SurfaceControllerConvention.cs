using Microsoft.AspNet.Mvc.ApplicationModels;
using Umbraco.Web.Controllers;

namespace Umbraco.Web
{
    public class SurfaceControllerConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel model)
        {
            foreach (var c in model.Controllers)
            {
                if (typeof(SurfaceController).IsAssignableFrom(c.ControllerType.GetType()))
                {
                    foreach (var a in c.Actions)
                    {
                        a.HttpMethods.Add("POST");
                    }
                }
            }
        }
    }
}