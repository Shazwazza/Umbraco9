using System;
using System.Collections.Generic;

namespace Umbraco.Web.Routing
{
    /// <summary>
    /// Provides context for the routing of a request.
    /// </summary>
    public class RoutingContext
    {
        private readonly Lazy<UrlProvider> _urlProvider;
        private readonly Lazy<IEnumerable<IContentFinder>> _publishedContentFinders;
        private readonly Lazy<ILastChanceContentFinder> _publishedContentLastChanceFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingContext"/> class.
        /// </summary>
        /// <param name="contentFinders">The document lookups resolver.</param>
        /// <param name="contentLastChanceFinder"> </param>
        /// <param name="urlProvider">The nice urls provider.</param>
        public RoutingContext(
            IEnumerable<IContentFinder> contentFinders,
            ILastChanceContentFinder contentLastChanceFinder,
            UrlProvider urlProvider)
        {
            _publishedContentFinders = new Lazy<IEnumerable<IContentFinder>>(() => contentFinders, false);
            _publishedContentLastChanceFinder = new Lazy<ILastChanceContentFinder>(() => contentLastChanceFinder, false);
            _urlProvider = new Lazy<UrlProvider>(() => urlProvider, false);
        }

        //internal RoutingContext(
        //    UmbracoContext umbracoContext,
        //    Lazy<IEnumerable<IContentFinder>> contentFinders,
        //    Lazy<ILastChanceContentFinder> contentLastChanceFinder,
        //    Lazy<UrlProvider> urlProvider)
        //{
        //    _publishedContentFinders = contentFinders;
        //    _publishedContentLastChanceFinder = contentLastChanceFinder;
        //    _urlProvider = urlProvider;
        //}

        /// <summary>
        /// Gets the published content finders.
        /// </summary>
        internal IEnumerable<IContentFinder> PublishedContentFinders
        {
            get { return _publishedContentFinders.Value; }
        }

        /// <summary>
        /// Gets the published content last chance finder.
        /// </summary>
        internal IContentFinder PublishedContentLastChanceFinder
        {
            get { return _publishedContentLastChanceFinder.Value; }
        }

        /// <summary>
        /// Gets the urls provider.
        /// </summary>
        public UrlProvider UrlProvider
        {
            get { return _urlProvider.Value; }
        }
    }
}