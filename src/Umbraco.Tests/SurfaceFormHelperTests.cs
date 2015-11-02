using System.Collections.Generic;
using Microsoft.AspNet.DataProtection;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Primitives;
using Moq;
using Umbraco.Web.Routing;
using Xunit;

namespace Umbraco.Tests
{
    public class SurfaceFormHelperTests
    {
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