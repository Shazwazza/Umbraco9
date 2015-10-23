using System.IO;
using Microsoft.AspNet.Routing;
using Microsoft.Dnx.Runtime;

namespace Umbraco.Web
{
  
    /// <summary>
    /// Request based context
    /// </summary>
    public class UmbracoContext
    {
        private RouteContext _routeCtx;
        private readonly IApplicationEnvironment _appEnv;

        public UmbracoContext(IApplicationEnvironment appEnv)
        {
           
            _appEnv = appEnv;
        }
        
        public void Initialize(RouteContext routeCtx)
        {
            if (Initialized) return;

            _routeCtx = routeCtx;

            //This would be like looking up content in Umbraco
            var path = _routeCtx.RouteData.Values["_umbracoRoute"] + ".txt";

            var filePath = Path.Combine(_appEnv.ApplicationBasePath, "UmbracoContent", path);

            Content = !File.Exists(filePath) ? null : File.ReadAllText(filePath);

            Initialized = true;
        }

        public bool Initialized { get; private set; }

        public bool HasContent => string.IsNullOrEmpty(Content) == false;

        public string Content { get; set; }
    }
}