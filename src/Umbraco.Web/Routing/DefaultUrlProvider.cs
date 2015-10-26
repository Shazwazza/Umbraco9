using System;
using System.Collections.Generic;

namespace Umbraco.Web.Routing
{
    public class DefaultUrlProvider : IUrlProvider
    {
        private readonly UmbracoContext _umbracoContext;

        public DefaultUrlProvider(UmbracoContext umbracoContext)
        {
            if (umbracoContext == null) throw new ArgumentNullException(nameof(umbracoContext));
            _umbracoContext = umbracoContext;
        }

        public IEnumerable<string> GetOtherUrls(Guid id, Uri current)
        {
            throw new NotImplementedException();
        }

        public string GetUrl(Guid id, Uri current, UrlProviderMode mode)
        {
            throw new NotImplementedException();
        }
    }
}