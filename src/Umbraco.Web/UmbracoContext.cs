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
            if (Initialized) return;

            _routeCtx = routeCtx;

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

            Initialized = true;
        }

        public bool Initialized { get; private set; }

        public bool HasContent => Content != null;

        public Content Content { get; set; }
    }
}