using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Core.Services;

namespace Umbraco.Web.Routing
{
    internal class PublishedContentRequestEngine
    {
        private readonly ITemplateService _templateService;
        private readonly PublishedContentRequest _pcr;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoutingContext _routingContext;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedContentRequestEngine"/> class with a content request.
        /// </summary>
        /// <param name="templateService"></param>
        /// <param name="pcr">The content request.</param>
        /// <param name="loggerFactory"></param>
        /// <param name="httpContextAccessor"></param>
        public PublishedContentRequestEngine(
            ITemplateService templateService,
            PublishedContentRequest pcr,
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            if (templateService == null) throw new ArgumentNullException(nameof(templateService));
            if (pcr == null) throw new ArgumentException("pcr is null.");
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));

            _templateService = templateService;
            _pcr = pcr;
            _httpContextAccessor = httpContextAccessor;
            _logger = loggerFactory.CreateLogger(typeof(PublishedContentRequestEngine).FullName);
            _routingContext = pcr.RoutingContext;

            //var umbracoContext = _routingContext.UmbracoContext;
            //if (umbracoContext == null) throw new ArgumentException("pcr.RoutingContext.UmbracoContext is null.");
            //if (umbracoContext.RoutingContext != _routingContext) throw new ArgumentException("RoutingContext confusion.");
            // no! not set yet.
            //if (umbracoContext.PublishedContentRequest != _pcr) throw new ArgumentException("PublishedContentRequest confusion.");
        }

        #region Public

        /// <summary>
        /// Prepares the request.
        /// </summary>
        /// <returns>
        /// Returns false if the request was not successfully prepared
        /// </returns>
        public async Task<bool> PrepareRequestAsync()
        {
            if (_pcr.RouteData == null) return false;

            // note - at that point the original legacy module did something do handle IIS custom 404 errors
            //   ie pages looking like /anything.aspx?404;/path/to/document - I guess the reason was to support
            //   "directory urls" without having to do wildcard mapping to ASP.NET on old IIS. This is a pain
            //   to maintain and probably not used anymore - removed as of 06/2012. @zpqrtbnk.
            //
            //   to trigger Umbraco's not-found, one should configure IIS and/or ASP.NET cusom 404 errors
            //   so that they point to a non-existing page eg /redirect-404.aspx
            //   TODO: SD: We need more information on this for when we release 4.10.0 as I'm not sure what this means.

            //find domain
            //FindDomain();

            // if request has been flagged to redirect then return
            // whoever called us is in charge of actually redirecting
            if (_pcr.IsRedirect)
            {
                return false;
            }

            // set the culture on the thread - once, so it's set when running document lookups
            if (_pcr.Culture != null)
            {
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = _pcr.Culture;
            }

            //find the published content if it's not assigned. This could be manually assigned with a custom route handler, or
            // with something like EnsurePublishedContentRequestAttribute or UmbracoVirtualNodeRouteHandler. Those in turn call this method
            // to setup the rest of the pipeline but we don't want to run the finders since there's one assigned.
            if (_pcr.PublishedContent == null && _pcr.RouteData.Values.ContainsKey("_umbracoRoute"))
            {
                // find the document & template
                await FindPublishedContentAndTemplateAsync();
            }

            // handle wildcard domains
            //HandleWildcardDomains();

            // set the culture on the thread -- again, 'cos it might have changed due to a finder or wildcard domain
            if (_pcr.Culture != null)
            {
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = _pcr.Culture;
            }

            // trigger the Prepared event - at that point it is still possible to change about anything
            // even though the request might be flagged for redirection - we'll redirect _after_ the event
            //
            // also, OnPrepared() will make the PublishedContentRequest readonly, so nothing can change
            //
            _pcr.OnPrepared();

            // we don't take care of anything so if the content has changed, it's up to the user
            // to find out the appropriate template

            //complete the PCR and assign the remaining values
            return ConfigureRequest();
        }

        /// <summary>
        /// Called by PrepareRequest once everything has been discovered, resolved and assigned to the PCR. This method
        /// finalizes the PCR with the values assigned.
        /// </summary>
        /// <returns>
        /// Returns false if the request was not successfully configured
        /// </returns>
        /// <remarks>
        /// This method logic has been put into it's own method in case developers have created a custom PCR or are assigning their own values
        /// but need to finalize it themselves.
        /// </remarks>
        public bool ConfigureRequest()
        {
            if (_pcr.HasPublishedContent == false)
            {
                return false;
            }

            // set the culture on the thread -- again, 'cos it might have changed in the event handler
            if (_pcr.Culture != null)
            {
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = _pcr.Culture;
            }

            // if request has been flagged to redirect, or has no content to display,
            // then return - whoever called us is in charge of actually redirecting
            if (_pcr.IsRedirect || _pcr.HasPublishedContent == false)
            {
                return false;
            }

            // we may be 404 _and_ have a content

            // can't go beyond that point without a PublishedContent to render
            // it's ok not to have a template, in order to give MVC a chance to hijack routes

            // note - the page() ctor below will cause the "page" to get the value of all its
            // "elements" ie of all the IPublishedContent property. If we use the object value,
            // that will trigger macro execution - which can't happen because macro execution
            // requires that _pcr.UmbracoPage is already initialized = catch-22. The "legacy"
            // pipeline did _not_ evaluate the macros, ie it is using the data value, and we
            // have to keep doing it because of that catch-22.

            return true;
        }

        /// <summary>
        /// Updates the request when there is no template to render the content.
        /// </summary>
        /// <remarks>This is called from Mvc when there's a document to render but no template.</remarks>
        public async Task UpdateRequestOnMissingTemplateAsync()
        {
            // clear content
            var content = _pcr.PublishedContent;
            _pcr.PublishedContent = null;

            await HandlePublishedContentAsync(); // will go 404
            FindTemplate();

            // if request has been flagged to redirect then return
            // whoever called us is in charge of redirecting
            if (_pcr.IsRedirect)
                return;

            if (_pcr.HasPublishedContent == false)
            {
                // means the engine could not find a proper document to handle 404
                // restore the saved content so we know it exists
                _pcr.PublishedContent = content;
                return;
            }

            if (_pcr.HasTemplate == false)
            {
                // means we may have a document, but we have no template
                // at that point there isn't much we can do and there is no point returning
                // to Mvc since Mvc can't do much either
                return;
            }

            // see note in PrepareRequest()
        }

        #endregion

        #region Domain

        ///// <summary>
        ///// Finds the site root (if any) matching the http request, and updates the PublishedContentRequest accordingly.
        ///// </summary>        
        ///// <returns>A value indicating whether a domain was found.</returns>
        //internal bool FindDomain()
        //{
        //    const string tracePrefix = "FindDomain: ";

        //    // note - we are not handling schemes nor ports here.

        //    _logger.LogDebug("{0}Uri=\"{1}\"", tracePrefix, _pcr.Uri);

        //    // try to find a domain matching the current request
        //    var domainAndUri = DomainHelper.DomainForUri(Services.DomainService.GetAll(false), _pcr.Uri);

        //    // handle domain
        //    if (domainAndUri != null && domainAndUri.UmbracoDomain.LanguageIsoCode.IsNullOrWhiteSpace() == false)
        //    {
        //        // matching an existing domain
        //        _logger.LogDebug("{0}Matches domain=\"{1}\", rootId={2}, culture=\"{3}\"",
        //            tracePrefix,
        //            domainAndUri.UmbracoDomain.DomainName,
        //            domainAndUri.UmbracoDomain.RootContentId,
        //            domainAndUri.UmbracoDomain.LanguageIsoCode);

        //        _pcr.UmbracoDomain = domainAndUri.UmbracoDomain;
        //        _pcr.DomainUri = domainAndUri.Uri;
        //        _pcr.Culture = new CultureInfo(domainAndUri.UmbracoDomain.LanguageIsoCode);

        //        // canonical? not implemented at the moment
        //        // if (...)
        //        // {
        //        //  _pcr.RedirectUrl = "...";
        //        //  return true;
        //        // }
        //    }
        //    else
        //    {
        //        // not matching any existing domain
        //        _logger.LogDebug("{0}Matches no domain", tracePrefix);

        //        var defaultLanguage = Services.LocalizationService.GetAllLanguages().FirstOrDefault();
        //        _pcr.Culture = defaultLanguage == null ? CultureInfo.CurrentUICulture : new CultureInfo(defaultLanguage.IsoCode);
        //    }

        //    _logger.LogDebug("{0}Culture=\"{1}\"",  tracePrefix, _pcr.Culture.Name);

        //    return _pcr.UmbracoDomain != null;
        //}

        ///// <summary>
        ///// Looks for wildcard domains in the path and updates <c>Culture</c> accordingly.
        ///// </summary>
        //internal void HandleWildcardDomains()
        //{
        //    const string tracePrefix = "HandleWildcardDomains: ";

        //    if (_pcr.HasPublishedContent == false)
        //        return;

        //    var nodePath = _pcr.PublishedContent.Path;
        //    _logger.LogDebug("{0}Path=\"{1}\"", tracePrefix,  nodePath);
        //    var rootNodeId = _pcr.HasDomain ? _pcr.UmbracoDomain.RootContentId : (int?)null;
        //    var domain = DomainHelper.FindWildcardDomainInPath(Services.DomainService.GetAll(true), nodePath, rootNodeId);

        //    if (domain != null && domain.LanguageIsoCode.IsNullOrWhiteSpace() == false)
        //    {
        //        _pcr.Culture = new CultureInfo(domain.LanguageIsoCode);
        //        _logger.LogDebug("{0}Got domain on node {1}, set culture to \"{2}\".", tracePrefix, domain.RootContentId, _pcr.Culture.Name);
        //    }
        //    else
        //    {
        //        _logger.LogDebug("{0}No match.", tracePrefix);
        //    }
        //}

        #endregion

        #region Document and template

        /// <summary>
        /// Finds the Umbraco document (if any) matching the request, and updates the PublishedContentRequest accordingly.
        /// </summary>
        /// <returns>A value indicating whether a document and template were found.</returns>
        private async Task FindPublishedContentAndTemplateAsync()
        {
            // run the document finders
            await FindPublishedContentAsync();

            // if request has been flagged to redirect then return
            // whoever called us is in charge of actually redirecting
            // -- do not process anything any further --
            if (_pcr.IsRedirect)
                return;

            // not handling umbracoRedirect here but after LookupDocument2
            // so internal redirect, 404, etc has precedence over redirect

            // handle not-found, redirects, access...
            await HandlePublishedContentAsync();

            // find a template
            FindTemplate();

            //// handle umbracoRedirect
            //FollowExternalRedirect();
        }

        /// <summary>
        /// Tries to find the document matching the request, by running the IPublishedContentFinder instances.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is no finder collection.</exception>
        internal async Task FindPublishedContentAsync()
        {
            //const string tracePrefix = "FindPublishedContent: ";

            // look for the document
            // the first successful finder, if any, will set this.PublishedContent, and may also set this.Template
            // some finders may implement caching

            if (_routingContext.PublishedContentFinders == null)
                throw new InvalidOperationException("There is no finder collection.");

            //iterate but return on first one that finds it
            foreach (var publishedContentFinder in _routingContext.PublishedContentFinders)
            {
                if (await publishedContentFinder.TryFindContentAsync(_pcr))
                {
                    break;
                }
            }

            // indicate that the published content (if any) we have at the moment is the
            // one that was found by the standard finders before anything else took place.
            _pcr.SetIsInitialPublishedContent();
        }

        /// <summary>
        /// Handles the published content (if any).
        /// </summary>
        /// <remarks>
        /// Handles "not found", internal redirects, access validation...
        /// things that must be handled in one place because they can create loops
        /// </remarks>
        private async Task HandlePublishedContentAsync()
        {
            const string tracePrefix = "HandlePublishedContent: ";

            // because these might loop, we have to have some sort of infinite loop detection 
            int i = 0, j = 0;
            const int maxLoop = 8;
            do
            {
                _logger.LogDebug("{0}{1}", tracePrefix, (i == 0 ? "Begin" : "Loop"));

                // handle not found
                if (_pcr.HasPublishedContent == false)
                {
                    _pcr.Is404 = true;
                    _logger.LogDebug("{0}No document, try last chance lookup", tracePrefix);

                    // if it fails then give up, there isn't much more that we can do
                    var lastChance = _routingContext.PublishedContentLastChanceFinder;
                    if (lastChance == null || await lastChance.TryFindContentAsync(_pcr) == false)
                    {
                        _logger.LogDebug("{0}Failed to find a document, give up", tracePrefix);
                        break;
                    }

                    _logger.LogDebug("{0}Found a document", tracePrefix);
                }

                //// follow internal redirects as long as it's not running out of control ie infinite loop of some sort
                //j = 0;
                //while (FollowInternalRedirects() && j++ < maxLoop)
                //{ }
                //if (j == maxLoop) // we're running out of control
                //    break;

                //// ensure access
                //if (_pcr.HasPublishedContent)
                //    EnsurePublishedContentAccess();

                // loop while we don't have page, ie the redirect or access
                // got us to nowhere and now we need to run the notFoundLookup again
                // as long as it's not running out of control ie infinite loop of some sort

            } while (_pcr.HasPublishedContent == false && i++ < maxLoop);

            if (i == maxLoop || j == maxLoop)
            {
                _logger.LogDebug("{0}Looks like we're running into an infinite loop, abort", tracePrefix);
                _pcr.PublishedContent = null;
            }

            _logger.LogDebug("{0}End", tracePrefix);
        }

        ///// <summary>
        ///// Follows internal redirections through the <c>umbracoInternalRedirectId</c> document property.
        ///// </summary>
        ///// <returns>A value indicating whether redirection took place and led to a new published document.</returns>
        ///// <remarks>
        ///// <para>Redirecting to a different site root and/or culture will not pick the new site root nor the new culture.</para>
        ///// <para>As per legacy, if the redirect does not work, we just ignore it.</para>
        ///// </remarks>
        //private bool FollowInternalRedirects()
        //{
        //    const string tracePrefix = "FollowInternalRedirects: ";

        //    if (_pcr.PublishedContent == null)
        //        throw new InvalidOperationException("There is no PublishedContent.");

        //    bool redirect = false;
        //    var internalRedirect = _pcr.PublishedContent.GetPropertyValue<string>(Constants.Conventions.Content.InternalRedirectId);

        //    if (string.IsNullOrWhiteSpace(internalRedirect) == false)
        //    {
        //        _logger.LogDebug("{0}Found umbracoInternalRedirectId={1}", () => tracePrefix, () => internalRedirect);

        //        int internalRedirectId;
        //        if (int.TryParse(internalRedirect, out internalRedirectId) == false)
        //            internalRedirectId = -1;

        //        if (internalRedirectId <= 0)
        //        {
        //            // bad redirect - log and display the current page (legacy behavior)
        //            //_pcr.Document = null; // no! that would be to force a 404
        //            _logger.LogDebug("{0}Failed to redirect to id={1}: invalid value", () => tracePrefix, () => internalRedirect);
        //        }
        //        else if (internalRedirectId == _pcr.PublishedContent.Id)
        //        {
        //            // redirect to self
        //            _logger.LogDebug("{0}Redirecting to self, ignore", () => tracePrefix);
        //        }
        //        else
        //        {
        //            // redirect to another page
        //            var node = _routingContext.UmbracoContext.ContentCache.GetById(internalRedirectId);

        //            _pcr.SetInternalRedirectPublishedContent(node); // don't use .PublishedContent here
        //            if (node != null)
        //            {
        //                redirect = true;
        //                _logger.LogDebug("{0}Redirecting to id={1}", () => tracePrefix, () => internalRedirectId);
        //            }
        //            else
        //            {
        //                _logger.LogDebug("{0}Failed to redirect to id={1}: no such published document", () => tracePrefix, () => internalRedirectId);
        //            }
        //        }
        //    }

        //    return redirect;
        //}

        ///// <summary>
        ///// Ensures that access to current node is permitted.
        ///// </summary>
        ///// <remarks>Redirecting to a different site root and/or culture will not pick the new site root nor the new culture.</remarks>
        //private void EnsurePublishedContentAccess()
        //{
        //    const string tracePrefix = "EnsurePublishedContentAccess: ";

        //    if (_pcr.PublishedContent == null)
        //        throw new InvalidOperationException("There is no PublishedContent.");

        //    var path = _pcr.PublishedContent.Path;

        //    var publicAccessAttempt = Services.PublicAccessService.IsProtected(path);

        //    if (publicAccessAttempt)
        //    {
        //        _logger.LogDebug("{0}Page is protected, check for access", () => tracePrefix);

        //        var membershipHelper = new MembershipHelper(_routingContext.UmbracoContext);

        //        if (membershipHelper.IsLoggedIn() == false)
        //        {
        //            _logger.LogDebug("{0}Not logged in, redirect to login page", () => tracePrefix);

        //            var loginPageId = publicAccessAttempt.Result.LoginNodeId;

        //            if (loginPageId != _pcr.PublishedContent.Id)
        //                _pcr.PublishedContent = _routingContext.UmbracoContext.ContentCache.GetById(loginPageId);
        //        }
        //        else if (Services.PublicAccessService.HasAccess(_pcr.PublishedContent.Id, Services.ContentService, _pcr.GetRolesForLogin(membershipHelper.CurrentUserName)) == false)
        //        {
        //            _logger.LogDebug("{0}Current member has not access, redirect to error page", () => tracePrefix);
        //            var errorPageId = publicAccessAttempt.Result.NoAccessNodeId;
        //            if (errorPageId != _pcr.PublishedContent.Id)
        //                _pcr.PublishedContent = _routingContext.UmbracoContext.ContentCache.GetById(errorPageId);
        //        }
        //        else
        //        {
        //            _logger.LogDebug("{0}Current member has access", () => tracePrefix);
        //        }
        //    }
        //    else
        //    {
        //        _logger.LogDebug("{0}Page is not protected", () => tracePrefix);
        //    }
        //}

        /// <summary>
        /// Finds a template for the current node, if any.
        /// </summary>
        private void FindTemplate()
        {
            // NOTE: at the moment there is only 1 way to find a template, and then ppl must
            // use the Prepared event to change the template if they wish. Should we also
            // implement an ITemplateFinder logic?

            const string tracePrefix = "FindTemplate: ";

            if (_pcr.PublishedContent == null)
            {
                _pcr.ResetTemplate();
                return;
            }

            // read the alternate template alias from querystring
            // only if the published content is the initial once, else the alternate template
            // does not apply
            // + optionnally, apply the alternate template on internal redirects
            var useAltTemplate = (_pcr.IsInitialPublishedContent || (_pcr.IsInternalRedirectPublishedContent));
            string altTemplate = null;
            if (useAltTemplate)
            {
                altTemplate = _httpContextAccessor.HttpContext.Request.Query["altTemplate"];               
            }

            if (string.IsNullOrWhiteSpace(altTemplate))
            {
                // we don't have an alternate template specified. use the current one if there's one already,
                // which can happen if a content lookup also set the template (LookupByNiceUrlAndTemplate...),
                // else lookup the template id on the document then lookup the template with that id.

                if (_pcr.HasTemplate)
                {
                    _logger.LogDebug("{0}Has a template already, and no alternate template.", tracePrefix);
                    return;
                }

                var templateId = _pcr.PublishedContent.TemplateId;

                if (templateId != Guid.Empty)
                {
                    _logger.LogDebug("{0}Look for template id={1}", tracePrefix, templateId);
                    var template = _templateService.GetTemplate(templateId);
                    if (template == null)
                        throw new InvalidOperationException("The template with Id " + templateId + " does not exist, the page cannot render");
                    _pcr.SetTemplate(template);
                    _logger.LogDebug("{0}Got template id={1} alias=\"{2}\"", tracePrefix, template.Id, template.Alias);
                }
                else
                {
                    _logger.LogDebug("{0}No specified template.", tracePrefix);
                }
            }
            else
            {
                // we have an alternate template specified. lookup the template with that alias
                // this means the we override any template that a content lookup might have set
                // so /path/to/page/template1?altTemplate=template2 will use template2

                // ignore if the alias does not match - just trace

                if (_pcr.HasTemplate)
                    _logger.LogDebug("{0}Has a template already, but also an alternate template.", tracePrefix);
                _logger.LogDebug("{0}Look for alternate template alias=\"{1}\"", tracePrefix, altTemplate);

                var template = _templateService.GetTemplate(altTemplate);
                if (template != null)
                {
                    _pcr.SetTemplate(template);
                    _logger.LogDebug("{0}Got template id={1} alias=\"{2}\"", tracePrefix, template.Id, template.Alias);
                }
                else
                {
                    _logger.LogDebug("{0}The template with alias=\"{1}\" does not exist, ignoring.", tracePrefix, altTemplate);
                }
            }

            if (_pcr.HasTemplate == false)
            {
                _logger.LogDebug("{0}No template was found.", tracePrefix);

                // initial idea was: if we're not already 404 and UmbracoSettings.HandleMissingTemplateAs404 is true
                // then reset _pcr.Document to null to force a 404.
                //
                // but: because we want to let MVC hijack routes even though no template is defined, we decide that
                // a missing template is OK but the request will then be forwarded to MVC, which will need to take
                // care of everything.
                //
                // so, don't set _pcr.Document to null here
            }
            else
            {
                _logger.LogDebug("{0}Running with template id={1} alias=\"{2}\"", tracePrefix, _pcr.TemplateModel.Id, _pcr.TemplateModel.Alias);
            }
        }

        #endregion
    }
}