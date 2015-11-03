using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.FileProviders;
using Microsoft.Dnx.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;

namespace Umbraco.Services
{
    public class FileSystemContentFinder : IContentFinder
    {
        private readonly IPublishedContentCache _contentCache;

        public FileSystemContentFinder(IPublishedContentCache contentCache)
        {
            _contentCache = contentCache;
        }

        public async Task<bool> TryFindContentAsync(PublishedContentRequest pcr)
        {
            var path = (string)pcr.RouteData.Values["_umbracoRoute"];

            var content = await _contentCache.GetByRouteAsync(false, path);
            
            if (content != null)
            {                
                pcr.PublishedContent = content;
                return true;                
            }

            return false;
        }
    }
}
