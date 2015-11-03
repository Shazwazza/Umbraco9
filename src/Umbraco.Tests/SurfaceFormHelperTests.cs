using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.DataProtection;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Primitives;
using Moq;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Xunit;

namespace Umbraco.Tests
{
    public class SurfaceFormHelperTests
    {
        [Fact]
        public void GetFormInfo()
        {
            var protector = new Mock<IDataProtector>();
            var protectorProvider = new Mock<IDataProtectionProvider>();
            protectorProvider.Setup(x => x.CreateProtector(It.IsAny<string>())).Returns(protector.Object);
            var surfaceRouteParams = $"{SurfaceFormHelper.ReservedAdditionalKeys.Controller}=mycontroller&{SurfaceFormHelper.ReservedAdditionalKeys.Action}=myaction&{SurfaceFormHelper.ReservedAdditionalKeys.Area}=myarea";
            protector.Setup(dataProtector => dataProtector.Protect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes(surfaceRouteParams));
            protector.Setup(dataProtector => dataProtector.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes(surfaceRouteParams));
            var helper = new SurfaceFormHelper(Mock.Of<ILoggerFactory>(), protectorProvider.Object);
            var request = new Mock<HttpRequest>();
            request.Setup(httpRequest => httpRequest.Method).Returns("GET");
            request.Setup(httpRequest => httpRequest.Query).Returns(
                new FormCollection(new Dictionary<string, StringValues>
                {
                    {"ufprt", Convert.ToBase64String(Encoding.UTF8.GetBytes(surfaceRouteParams))}
                }));

            var httpCtxAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.Request).Returns(request.Object);
            httpCtxAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext.Object);            
            var routeCtx = new RouteContext(httpContext.Object);

            var result = helper.GetFormInfo(routeCtx);

            Assert.Equal("mycontroller", result.ControllerName);
            Assert.Equal("myaction", result.ActionName);
            Assert.Equal("myarea", result.Area);
        }

        [Fact]
        public void GetRequestSurfaceToken_Post()
        {
            var helper = new SurfaceFormHelper(Mock.Of<ILoggerFactory>(), Mock.Of<IDataProtectionProvider>());
            var request = new Mock<HttpRequest>();
            request.Setup(httpRequest => httpRequest.Method).Returns("POST");
            request.Setup(httpRequest => httpRequest.HasFormContentType).Returns(true);
            request.Setup(httpRequest => httpRequest.Form).Returns(
                new FormCollection(new Dictionary<string, StringValues>
                {
                    {"ufprt", "test"}
                }));
            var result = helper.GetRequestSurfaceToken(request.Object);
            Assert.Equal("test", result);
        }

        [Fact]
        public void GetRequestSurfaceToken_Get()
        {
            var helper = new SurfaceFormHelper(Mock.Of<ILoggerFactory>(), Mock.Of<IDataProtectionProvider>());
            var request = new Mock<HttpRequest>();
            request.Setup(httpRequest => httpRequest.Method).Returns("GET");
            request.Setup(httpRequest => httpRequest.Query).Returns(
                new FormCollection(new Dictionary<string, StringValues>
                {
                    {"ufprt", "test"}
                }));
            var result = helper.GetRequestSurfaceToken(request.Object);
            Assert.Equal("test", result);
        }

        [Fact]
        public void ValidateRequiredTokenParams()
        {
            var helper = new SurfaceFormHelper(Mock.Of<ILoggerFactory>(), Mock.Of<IDataProtectionProvider>());
            var result = helper.ValidateRequiredTokenParams(new Dictionary<string, string>
            {
                {SurfaceFormHelper.ReservedAdditionalKeys.Controller, "foo"},
                {SurfaceFormHelper.ReservedAdditionalKeys.Action, "bar"},
                {SurfaceFormHelper.ReservedAdditionalKeys.Area, "a"}
            });

            Assert.Equal(true, result);

            result = helper.ValidateRequiredTokenParams(new Dictionary<string, string>
            {
                {SurfaceFormHelper.ReservedAdditionalKeys.Controller, "foo"},
                {SurfaceFormHelper.ReservedAdditionalKeys.Action, "bar"}
            });

            Assert.Equal(false, result);
        }
    }
}