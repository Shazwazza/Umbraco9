using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Dnx.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Web.Models;

namespace Umbraco.Web.Routing
{
    public class PathContentFinder : IContentFinder
    {
        private readonly PublishedContentRequest _pcr;
        private readonly IApplicationEnvironment _appEnv;

        public PathContentFinder(PublishedContentRequest pcr, IApplicationEnvironment appEnv)
        {
            _pcr = pcr;
            _appEnv = appEnv;
        }

        public Task<bool> TryFindContentAsync(RouteData routeData)
        {
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

                    var content = (PublishedContent) serializer.Deserialize(file, typeof (PublishedContent));
                    _pcr.PublishedContent = content;

                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}
