//using System;
//using System.Collections.Generic;
//using Microsoft.AspNet.Http;
//using Microsoft.AspNet.Mvc.ActionConstraints;
//using Microsoft.AspNet.Mvc.Controllers;
//using Umbraco.Core;
//using Umbraco.Web.Controllers;

//namespace Umbraco.Web.Routing
//{
//    /// <summary>
//    /// This checks the current route to see if it is an Umbraco route
//    /// </summary>
//    /// <remarks>
//    /// This is also the process that initializes the UmbracoContext
//    /// </remarks>
//    public class UmbracoActionConstraint : IActionConstraint
//    {

//        private readonly UmbracoContext _umbCtx;
//        private readonly PublishedContentRequest _pcr;
//        private readonly UmbracoControllerTypeCollection _umbControllerTypes;

//        public UmbracoActionConstraint(
//            UmbracoContext umbCtx, PublishedContentRequest pcr, UmbracoControllerTypeCollection umbControllerTypes)
//        {
//            _umbCtx = umbCtx;
//            _pcr = pcr;
//            _umbControllerTypes = umbControllerTypes;
//        }

//        public int Order => 0;
        
//        public bool Accept(ActionConstraintContext context)
//        {
//            //Initialize the context, this will be called a few times but the initialize logic
//            // only executes once. There might be a nicer way to do this but the RouteContext and 
//            // other request scoped instances are not available yet.
//            _umbCtx.Initialize(_pcr);
            
//            //Prepare the request if it hasn't already been done
//            if (_pcr.IsPrepared == false)
//            {
//                //TODO: See https://github.com/aspnet/Mvc/issues/3412
//                // we want this method to be async
//                //NOTE: We are using the GetAwaiter() syntax in order to get a real exception instead of an Aggregate exception
//                if (_pcr.PrepareAsync(context.RouteContext.RouteData).GetAwaiter().GetResult())
//                {
//                    if (_umbCtx.HasContent == false) return false;
//                }
//            }

//            //don't match ever if there is no content
//            if (_umbCtx.HasContent == false) return false;

//            ////Is this a POST
//            //if (context.RouteContext.HttpContext.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
//            //{
//            //    if (((ControllerActionDescriptor)context.CurrentCandidate.Action).ControllerName == "TestSurface")
//            //    {
//            //        return true;
//            //    }
//            //}

//            //string actionNameRequest =
//            //    context.RouteContext.HttpContext.Request.Query["actionName"] ??
//            //"Index";

//            var isMatch = false;
//            //Let's match controller names:        
//            //check if there are actually any controllers that match the content type
//            if (_umbControllerTypes.ContainsControllerName(_umbCtx.PublishedContent.ContentType))
//            {
//                //ok, so there is actually a controller type registered with this content type, so we must make sure 
//                // that we only match that controller.
//                if (((ControllerActionDescriptor) context.CurrentCandidate.Action).ControllerName.InvariantEquals(_umbCtx.PublishedContent.ContentType))
//                {
//                    isMatch = true;
//                }
//            }
//            else
//            {
//                //There is no controller type registerd for this content type, so we'll match if this is the default
//                if (((ControllerActionDescriptor)context.CurrentCandidate.Action).ControllerName == typeof(UmbracoController).Name.TrimEnd("Controller"))
//                {
//                    isMatch = true;
//                }
//            }

//            if (isMatch)
//            {                
//                return true;
//            }
            
//            //Another way to do that:
//            //object controllerNameFound;
//            //if (context.CurrentCandidate.Action.Properties.TryGetValue("controllerName", out controllerNameFound))
//            //{
//            //    if ((string)controllerNameFound == controllerNameRequest)
//            //    {
//            //        return true;
//            //    }
//            //}

//            return false;
//        }
//    }
//}