using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.Logging;
using Moq;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Controllers;
using Umbraco.Web.Models;
using Umbraco.Web.Routing;
using Xunit;

namespace Umbraco.Tests
{
    public class UmbracoRouterTests
    {
        [Fact]
        public async void RouteUmbracoContentAsync_Umbraco_Context_Initialized()
        {
            var router = new UmbracoRouter(Mock.Of<IRouter>());
            var httpCtxAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.Request).Returns(Mock.Of<HttpRequest>());
            httpCtxAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext.Object);
            var umbCtx = new UmbracoContext(httpCtxAccessor.Object);
            var urlProvider = new UrlProvider(umbCtx, Enumerable.Empty<IUrlProvider>());
            var routingCtx = new RoutingContext(Enumerable.Empty<IContentFinder>(), Mock.Of<ILastChanceContentFinder>(), urlProvider);
            var pcr = new PublishedContentRequest(routingCtx, Mock.Of<ITemplateService>(), Mock.Of<ILoggerFactory>(), httpCtxAccessor.Object);

            var result = await router.RouteUmbracoContentAsync(umbCtx, pcr, new RouteData());

            Assert.Equal(true, umbCtx.Initialized);
            Assert.Equal(false, result);
        }

        [Fact]
        public void GetUmbracoRouteValues_Returns_Default()
        {
            var router = new UmbracoRouter(Mock.Of<IRouter>());
            var httpCtxAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.Request).Returns(Mock.Of<HttpRequest>());
            httpCtxAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext.Object);
            var umbCtx = new UmbracoContext(httpCtxAccessor.Object);            
            var urlProvider = new UrlProvider(umbCtx, Enumerable.Empty<IUrlProvider>());
            var routingCtx = new RoutingContext(Enumerable.Empty<IContentFinder>(), Mock.Of<ILastChanceContentFinder>(), urlProvider);
            var pcr = new PublishedContentRequest(routingCtx, Mock.Of<ITemplateService>(), Mock.Of<ILoggerFactory>(), httpCtxAccessor.Object)
            {
                PublishedContent = new PublishedContent()
            };
            umbCtx.Initialize(pcr);
            var actionDescriptors = new Mock<IActionDescriptorsCollectionProvider>();
            actionDescriptors.Setup(provider => provider.ActionDescriptors).Returns(new ActionDescriptorsCollection(new List<ActionDescriptor>(), 0));

            var result = router.GetUmbracoRouteValues(umbCtx, new UmbracoControllerTypeCollection(actionDescriptors.Object));

            Assert.Equal("Umbraco", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public void GetUmbracoRouteValues_Find_Custom_Controller()
        {
            var router = new UmbracoRouter(Mock.Of<IRouter>());
            var httpCtxAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.Request).Returns(Mock.Of<HttpRequest>());
            httpCtxAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext.Object);
            var umbCtx = new UmbracoContext(httpCtxAccessor.Object);
            var urlProvider = new UrlProvider(umbCtx, Enumerable.Empty<IUrlProvider>());
            var routingCtx = new RoutingContext(Enumerable.Empty<IContentFinder>(), Mock.Of<ILastChanceContentFinder>(), urlProvider);
            var templateService = new Mock<ITemplateService>();
            templateService.Setup(service => service.GetTemplate("Hello")).Returns(Mock.Of<ITemplate>(template => template.Alias == "Hello"));
            var pcr = new PublishedContentRequest(routingCtx, templateService.Object, Mock.Of<ILoggerFactory>(), httpCtxAccessor.Object)
            {
                PublishedContent = new PublishedContent()
                {
                    ContentType = "Custom"
                }
            };
            pcr.TrySetTemplate("Hello");
            umbCtx.Initialize(pcr);
            var actionDescriptors = new Mock<IActionDescriptorsCollectionProvider>();
            actionDescriptors.Setup(provider => provider.ActionDescriptors).Returns(new ActionDescriptorsCollection(
                new List<ActionDescriptor>()
                {
                    new ControllerActionDescriptor()
                    {
                        Name = "Hello",
                        ControllerName = "Custom",
                        ControllerTypeInfo = typeof(UmbracoController).GetTypeInfo()
                    }
                }, 0));

            var result = router.GetUmbracoRouteValues(umbCtx, new UmbracoControllerTypeCollection(actionDescriptors.Object));

            Assert.Equal("Custom", result.ControllerName);
            Assert.Equal("Hello", result.ActionName);
        }

    }
}
