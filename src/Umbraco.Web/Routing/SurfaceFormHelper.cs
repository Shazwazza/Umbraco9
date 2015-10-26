using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.DataProtection;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.WebUtilities;
using Microsoft.Framework.Logging;
using Microsoft.Framework.WebEncoders;
using Umbraco.Core;
using Umbraco.Web.Models;

namespace Umbraco.Web.Routing
{
    public sealed class SurfaceFormHelper
    {
        private readonly ILogger _logger;
        private readonly IDataProtector _dataProtector;

        public SurfaceFormHelper(ILoggerFactory loggerFactory, IDataProtectionProvider dataProtectionProvider)
        {
            _logger = loggerFactory.CreateLogger<SurfaceFormHelper>();
            _dataProtector = dataProtectionProvider.CreateProtector("Umbraco.Surface.Form");
        }

        /// <summary>
        /// Checks the request and query strings to see if it matches the definition of having a Surface controller
        /// posted/get value, if so, then we return a PostedDataProxyInfo object with the correct information.
        /// </summary>
        /// <param name="routeContext"></param>
        /// <returns></returns>
        public RouteDefinition GetFormInfo(RouteContext routeContext)
        {
            if (routeContext == null) throw new ArgumentNullException(nameof(routeContext));

            string encodedVal;

            switch (routeContext.HttpContext.Request.Method)
            {
                case "POST":
                    //get the value from the request.
                    //this field will contain an encrypted version of the surface route vals.
                    encodedVal = routeContext.HttpContext.Request.HasFormContentType 
                        ? (string)routeContext.HttpContext.Request.Form["ufprt"]
                        : null;
                    if (encodedVal.IsNullOrWhiteSpace()) return null;
                    break;
                case "GET":
                    //this field will contain an encrypted version of the surface route vals.
                    encodedVal = routeContext.HttpContext.Request.Query["ufprt"];
                    if (encodedVal.IsNullOrWhiteSpace()) return null;
                    break;
                default:
                    return null;
            }


            string decryptedString;
            try
            {
                decryptedString = Encoding.UTF8.GetString(_dataProtector.Unprotect(Convert.FromBase64String(encodedVal)));
            }
            catch (CryptographicException)
            {
                _logger.LogWarning("A value was detected in the ufprt parameter but Umbraco could not decrypt the string");
                return null;
            }

            var parsedQueryString = QueryHelpers.ParseQuery(decryptedString);
            var decodedParts = new Dictionary<string, string>();

            foreach (var key in parsedQueryString.Keys)
            {
                decodedParts[key] = parsedQueryString[key];
            }

            //validate all required keys exist

            //the controller
            if (decodedParts.All(x => x.Key != ReservedAdditionalKeys.Controller))
                return null;
            //the action
            if (decodedParts.All(x => x.Key != ReservedAdditionalKeys.Action))
                return null;
            //the area
            if (decodedParts.All(x => x.Key != ReservedAdditionalKeys.Area))
                return null;

            foreach (var item in decodedParts.Where(x => new[] {
                ReservedAdditionalKeys.Controller,
                ReservedAdditionalKeys.Action,
                ReservedAdditionalKeys.Area }.Contains(x.Key) == false))
            {
                // Populate route with additional values which aren't reserved values so they eventually to action parameters
                routeContext.RouteData.Values[item.Key] = item.Value;
            }

            //return the proxy info without the surface id... could be a local controller.
            return new RouteDefinition
            {
                ControllerName = WebUtility.UrlDecode(decodedParts.Single(x => x.Key == ReservedAdditionalKeys.Controller).Value),
                ActionName = WebUtility.UrlDecode(decodedParts.Single(x => x.Key == ReservedAdditionalKeys.Action).Value),
                Area = WebUtility.UrlDecode(decodedParts.Single(x => x.Key == ReservedAdditionalKeys.Area).Value),
            };
        }

        /// <summary>
        /// This is used in methods like BeginUmbracoForm and SurfaceAction to generate an encrypted string which gets submitted in a request for which
        /// Umbraco can decrypt during the routing process in order to delegate the request to a specific MVC Controller.
        /// </summary>
        /// <param name="urlEncoder"></param>
        /// <param name="controllerName"></param>
        /// <param name="controllerAction"></param>
        /// <param name="area"></param>
        /// <param name="additionalRouteVals"></param>
        /// <returns></returns>
        public string CreateEncryptedRouteString(IUrlEncoder urlEncoder, string controllerName, string controllerAction, string area, object additionalRouteVals = null)
        {

            //need to create a params string as Base64 to put into our hidden field to use during the routes
            var surfaceRouteParams = $"{ReservedAdditionalKeys.Controller}={urlEncoder.UrlEncode(controllerName)}&{ReservedAdditionalKeys.Action}={urlEncoder.UrlEncode(controllerAction)}&{ReservedAdditionalKeys.Area}={area}";

            var additionalRouteValsAsQuery = additionalRouteVals?.ToDictionary<object>().ToQueryString(urlEncoder);

            if (string.IsNullOrWhiteSpace(additionalRouteValsAsQuery) == false)
                surfaceRouteParams += "&" + additionalRouteValsAsQuery;
            
            return Convert.ToBase64String(_dataProtector.Protect(Encoding.UTF8.GetBytes(surfaceRouteParams)));
        }

        // Define reserved dictionary keys for controller, action and area specified in route additional values data
        private static class ReservedAdditionalKeys
        {
            internal const string Controller = "c";
            internal const string Action = "a";
            internal const string Area = "ar";
        }

    }
}