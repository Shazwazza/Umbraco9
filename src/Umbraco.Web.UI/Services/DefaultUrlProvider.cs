using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;

namespace Umbraco.Services
{
    public class DefaultUrlProvider : IUrlProvider
    {
        private readonly IPublishedContentCache _contentCache;

        public DefaultUrlProvider(IPublishedContentCache contentCache)
        {
            if (contentCache == null) throw new ArgumentNullException(nameof(contentCache));
            _contentCache = contentCache;
        }

        public Task<IEnumerable<string>> GetOtherUrlsAsync(Guid id, Uri current)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUrlAsync(Guid id, Uri current, UrlProviderMode mode)
        {
            if (!current.IsAbsoluteUri)
                throw new ArgumentException("Current url must be absolute.", "current");

            // will not use cache if previewing
            var route = await _contentCache.GetRouteByIdAsync(false, id);            
            if (route == null) return null;

            return route;
        }
    }
}