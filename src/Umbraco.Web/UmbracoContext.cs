using System;
using System.IO;
using Microsoft.AspNet.Http;
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
        private RouteData _routeData;
        private readonly IApplicationEnvironment _appEnv;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UmbracoContext(IApplicationEnvironment appEnv, IHttpContextAccessor httpContextAccessor)
        {
            _appEnv = appEnv;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(RouteData routeData)
        {
            if (routeData?.Values == null) return;
            if (routeData.Values.ContainsKey("_umbracoRoute") == false) return;

            if (Initialized) return;

            _routeData = routeData;

            RequestPath = _httpContextAccessor.HttpContext.Request.Path;
            
            //This would be like looking up content in Umbraco
            var path = routeData.Values["_umbracoRoute"] + ".txt";

            var filePath = Path.Combine(_appEnv.ApplicationBasePath, "UmbracoContent", path);

            if (File.Exists(filePath))
            {
                using (var file = File.OpenText(filePath))
                {
                    var serializer = new JsonSerializer
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                    Content = (PublishedContent)serializer.Deserialize(file, typeof(PublishedContent));
                }
            }

            //TODO: This name/etc. is temporary from old testing
            AltTemplate = _httpContextAccessor.HttpContext.Request.Query["altTemplate"];
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

        public IPublishedContent Content { get; set; }
        
    }
}