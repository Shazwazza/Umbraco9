using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedCache;

namespace Umbraco.Services
{
    public class FileSystemPublishedContentCache : IPublishedContentCache
    {
        private readonly IFileProvider _fileProvider;

        public FileSystemPublishedContentCache(IFileProvider fileProvider)
        {
            if (fileProvider == null) throw new ArgumentNullException(nameof(fileProvider));
            _fileProvider = fileProvider;
        }

        public Task<IPublishedContent> GetByIdAsync(bool preview, Guid contentId)
        {
            if (preview) throw new NotImplementedException("No preview support yet");
            throw new NotImplementedException();
        }

        public Task<IPublishedContent> GetByRouteAsync(bool preview, string route)
        {
            if (preview) throw new NotImplementedException("No preview support yet");

            var fileInfo = _fileProvider.GetFileInfo(string.Concat("UmbracoContent/", route + ".json"));

            if (fileInfo != null)
            {
                using (var file = fileInfo.CreateReadStream())
                using (var reader = new StreamReader(file))
                {
                    var serializer = new JsonSerializer
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };

                    var content = (IPublishedContent)serializer.Deserialize(reader, typeof(PublishedContent));                    
                    return Task.FromResult(content);
                }
            }
            return Task.FromResult((IPublishedContent)null);
        }

        public Task<string> GetRouteByIdAsync(bool preview, Guid contentId)
        {
            if (preview) throw new NotImplementedException("No preview support yet");
        }
    }
}