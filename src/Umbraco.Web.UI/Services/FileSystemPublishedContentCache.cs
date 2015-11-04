using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.FileProviders;
using Microsoft.Dnx.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedCache;
using Umbraco.Core;

namespace Umbraco.Services
{
    public class FileSystemPublishedContentCache : IPublishedContentCache
    {
        private readonly IApplicationEnvironment _appEnv;
        private readonly IFileProvider _fileProvider;

        public FileSystemPublishedContentCache(IApplicationEnvironment appEnv)
        {            
            if (appEnv == null) throw new ArgumentNullException(nameof(appEnv));
            _appEnv = appEnv;
            _fileProvider = new PhysicalFileProvider(appEnv.ApplicationBasePath);

            _fileCache = new Lazy<IDictionary<Guid, Tuple<IPublishedContent, IFileInfo>>>(() => FindAllContent("UmbracoContent"));
        }

        private readonly Lazy<IDictionary<Guid, Tuple<IPublishedContent, IFileInfo>>> _fileCache;

        private IDictionary<Guid, Tuple<IPublishedContent, IFileInfo>> FindAllContent(string path)
        {
            var current = new Dictionary<Guid, Tuple<IPublishedContent, IFileInfo>>();
            var directoryContents = _fileProvider.GetDirectoryContents(path);
            foreach (var fileContent in directoryContents)
            {
                if (fileContent.IsDirectory)
                {
                    var found = FindAllContent(GetRelativePathFromFile(fileContent));
                    foreach (var fileInfo in found)
                    {
                        current[fileInfo.Key] = fileInfo.Value;
                    }
                }
                else if(fileContent.PhysicalPath.EndsWith(".json"))
                {
                    var publishedContent = ReadContent(fileContent);
                    current[publishedContent.Id] = new Tuple<IPublishedContent, IFileInfo>(publishedContent, fileContent);
                }
            }
            return current;
        }

        private string GetRelativePathFromFile(IFileInfo fileInfo)
        {
            return fileInfo.PhysicalPath.Substring(_appEnv.ApplicationBasePath.Length).TrimStart('\\');
        }

        public Task<IPublishedContent> GetByIdAsync(bool preview, Guid contentId)
        {
            if (preview) throw new NotImplementedException("No preview support yet");
            throw new NotImplementedException();
        }

        public Task<IPublishedContent> GetByRouteAsync(bool preview, string route)
        {
            if (preview) throw new NotImplementedException("No preview support yet");
            
            var fileInfo = _fileProvider.GetFileInfo(string.Concat("UmbracoContent/", route , ".json"));

            if (fileInfo != null && fileInfo.Exists)
            {
                return Task.FromResult(ReadContent(fileInfo));
            }

            //check for folder +  'Index.json'
            var dirInfo = _fileProvider.GetDirectoryContents(string.Concat("UmbracoContent/", route));
            if (dirInfo.Exists)
            {
                fileInfo = _fileProvider.GetFileInfo(string.Concat("UmbracoContent/", route, "/index.json"));
                if (fileInfo != null)
                {
                    return Task.FromResult(ReadContent(fileInfo));
                }
            }
            return Task.FromResult((IPublishedContent)null);
        }

        public Task<string> GetRouteByIdAsync(bool preview, Guid contentId)
        {
            if (preview) throw new NotImplementedException("No preview support yet");

            if (_fileCache.Value.ContainsKey(contentId))
            {
                return Task.FromResult(
                    GetRelativePathFromFile(_fileCache.Value[contentId].Item2)
                    .Replace("\\", "/")                                        
                    .ToLowerInvariant()
                    .TrimEnd(".json")
                    .TrimStart("umbracocontent/")
                    .EnsureStartsWith("/"));
            }
            return Task.FromResult((string) null);
        }

        private IPublishedContent ReadContent(IFileInfo fileInfo)
        {
            using (var file = fileInfo.CreateReadStream())
            using (var reader = new StreamReader(file))
            {
                var serializer = new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var content = (IPublishedContent)serializer.Deserialize(reader, typeof(PublishedContent));
                return content;
            }
        }
        
    }
}