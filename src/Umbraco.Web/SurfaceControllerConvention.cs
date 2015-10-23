using Microsoft.AspNet.Mvc.ApplicationModels;
using Umbraco.Web.Controllers;

namespace Umbraco.Web
{
    /// <summary>
    /// This applies routing conventions to all SurfaceControllers
    /// </summary>
    /// <remarks>
    /// Mostly, this ensures that SurfaceController actions can only be requests via POST
    /// </remarks>
    public class SurfaceControllerConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel model)
        {
            foreach (var c in model.Controllers)
            {
                if (typeof(SurfaceController).IsAssignableFrom(c.ControllerType))
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