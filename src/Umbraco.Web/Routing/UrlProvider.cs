using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbraco.Web.Routing
{
    /// <summary>
    /// Provides urls.
    /// </summary>
    public class UrlProvider
    {
        #region Ctor and configuration      

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlProvider"/> class with an Umbraco context and a list of url providers.
        /// </summary>
        /// <param name="umbracoContext">The Umbraco context.</param>
        /// <param name="urlProviders">The list of url providers.</param>
        /// <param name="provider"></param>
        public UrlProvider(UmbracoContext umbracoContext, IEnumerable<IUrlProvider> urlProviders, UrlProviderMode provider = UrlProviderMode.Auto)
        {
            if (umbracoContext == null) throw new ArgumentNullException("umbracoContext");

            _umbracoContext = umbracoContext;
            _urlProviders = urlProviders;

            Mode = provider;
        }

        private readonly UmbracoContext _umbracoContext;
        private readonly IEnumerable<IUrlProvider> _urlProviders;

        /// <summary>
        /// Gets or sets the provider url mode.
        /// </summary>
        public UrlProviderMode Mode { get; set; }

        #endregion

        #region GetUrl

        /// <summary>
        /// Gets the url of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <returns>The url for the published content.</returns>
        /// <remarks>
        /// <para>The url is absolute or relative depending on <c>Mode</c> and on the current url.</para>
        /// <para>If the provider is unable to provide a url, it returns "#".</para>
        /// </remarks>
        public async Task<string> GetUrlAsync(Guid id)
        {
            return await GetUrlAsync(id, new Uri(_umbracoContext.RequestPath, UriKind.Relative), Mode);
        }

        /// <summary>
        /// Gets the nice url of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <param name="absolute">A value indicating whether the url should be absolute in any case.</param>
        /// <returns>The url for the published content.</returns>
        /// <remarks>
        /// <para>The url is absolute or relative depending on <c>Mode</c> and on <c>current</c>, unless
        /// <c>absolute</c> is true, in which case the url is always absolute.</para>
        /// <para>If the provider is unable to provide a url, it returns "#".</para>
        /// </remarks>
        public async Task<string> GetUrlAsync(Guid id, bool absolute)
        {
            var mode = absolute ? UrlProviderMode.Absolute : Mode;
            return await GetUrlAsync(id, new Uri(_umbracoContext.RequestPath, UriKind.Relative), mode);
        }

        /// <summary>
        /// Gets the nice url of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <param name="current">The current absolute url.</param>
        /// <param name="absolute">A value indicating whether the url should be absolute in any case.</param>
        /// <returns>The url for the published content.</returns>
        /// <remarks>
        /// <para>The url is absolute or relative depending on <c>Mode</c> and on <c>current</c>, unless
        /// <c>absolute</c> is true, in which case the url is always absolute.</para>
        /// <para>If the provider is unable to provide a url, it returns "#".</para>
        /// </remarks>
        public async Task<string> GetUrlAsync(Guid id, Uri current, bool absolute)
        {
            var mode = absolute ? UrlProviderMode.Absolute : Mode;
            return await GetUrlAsync(id, current, mode);
        }

        /// <summary>
        /// Gets the nice url of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <param name="mode">The url mode.</param>
        /// <returns>The url for the published content.</returns>
        /// <remarks>
        /// <para>The url is absolute or relative depending on <c>mode</c> and on the current url.</para>
        /// <para>If the provider is unable to provide a url, it returns "#".</para>
        /// </remarks>
        public async Task<string> GetUrlAsync(Guid id, UrlProviderMode mode)
        {
            return await GetUrlAsync(id, new Uri(_umbracoContext.RequestPath, UriKind.Relative), mode);
        }

        /// <summary>
        /// Gets the nice url of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <param name="current">The current absolute url.</param>
        /// <param name="mode">The url mode.</param>
        /// <returns>The url for the published content.</returns>
        /// <remarks>
        /// <para>The url is absolute or relative depending on <c>mode</c> and on <c>current</c>.</para>
        /// <para>If the provider is unable to provide a url, it returns "#".</para>
        /// </remarks>
        public async Task<string> GetUrlAsync(Guid id, Uri current, UrlProviderMode mode)
        {
            foreach (var provider in _urlProviders)
            {
                var result = await provider.GetUrlAsync(id, current, mode);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        #endregion

        #region GetOtherUrls

        /// <summary>
        /// Gets the other urls of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <returns>The other urls for the published content.</returns>
        /// <remarks>
        /// <para>Other urls are those that <c>GetUrl</c> would not return in the current context, but would be valid
        /// urls for the node in other contexts (different domain for current request, umbracoUrlAlias...).</para>
        /// <para>The results depend on the current url.</para>
        /// </remarks>
        public Task<IEnumerable<string>> GetOtherUrlsAsync(Guid id)
        {
            return GetOtherUrlsAsync(id, new Uri(_umbracoContext.RequestPath, UriKind.Relative));
        }

        /// <summary>
        /// Gets the other urls of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <param name="current">The current absolute url.</param>
        /// <returns>The other urls for the published content.</returns>
        /// <remarks>
        /// <para>Other urls are those that <c>GetUrl</c> would not return in the current context, but would be valid
        /// urls for the node in other contexts (different domain for current request, umbracoUrlAlias...).</para>
        /// </remarks>
        public async Task<IEnumerable<string>> GetOtherUrlsAsync(Guid id, Uri current)
        {
            var result = new List<string>();
            foreach (var provider in _urlProviders)
            {
                var urls = await provider.GetOtherUrlsAsync(id, current);
                if (urls != null)
                {
                    result.AddRange(urls);
                }
            }
            return result;
        }

        #endregion
    }
}