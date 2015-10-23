using System.IO;
using Microsoft.AspNet.Routing;
using Microsoft.Dnx.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Umbraco.Web.Models;

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
            if (routeCtx == null || routeCtx.RouteData == null || routeCtx.RouteData.Values == null) return;
            if (routeCtx.RouteData.Values.ContainsKey("_umbracoRoute") == false) return;

            if (Initialized) return;

            _routeCtx = routeCtx;

            RequestPath = _routeCtx.HttpContext.Request.Path;

            //This would be like looking up content in Umbraco
            var path = _routeCtx.RouteData.Values["_umbracoRoute"] + ".txt";

            var filePath = Path.Combine(_appEnv.ApplicationBasePath, "UmbracoContent", path);

            if (File.Exists(filePath))
            {
                using (var file = File.OpenText(filePath))
                {
                    var serializer = new JsonSerializer
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                    Content = (Content)serializer.Deserialize(file, typeof(Content));
                }                
            }

            //TODO: This name/etc. is temporary from old testing
            AltTemplate = _routeCtx.HttpContext.Request.Query["altTemplate"];
            if (string.IsNullOrEmpty(AltTemplate))
            {
                AltTemplate = "Umbraco";
            }

            Initialized = true;
        }

        public string AltTemplate { get; private set; }

        public string RequestPath { get; private set; }

        public bool Initialized { get; private set; }

        public bool HasContent => Content != null;

        public Content Content { get; set; }
    }
}