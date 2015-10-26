//using System;
//using Microsoft.AspNet.Mvc.ActionConstraints;
//using Microsoft.Framework.DependencyInjection;
//using Umbraco.Web.Controllers;

//namespace Umbraco.Web.Routing
//{
//    public class UmbracoActionConstraintAttribute : Attribute, IActionConstraintFactory
//        //, IControllerModelConvention
//        //,IActionModelConvention
//    {
//        //public void Apply(ActionModel model)
//        //{            
//        //    model.RouteConstraints.Clear();
//        //    model.RouteConstraints.Add(new UmbracoRouteConstraintAttribute());
//        //}

//        //public void Apply(ControllerModel model)
//        //{
//        //    model.RouteConstraints.Clear();
//        //    model.RouteConstraints.Add(new UmbracoRouteConstraintAttribute());
//        //}

//        public IActionConstraint CreateInstance(IServiceProvider services)
//        {
//            var umbCtx = services.GetRequiredService<UmbracoContext>();
//            var pcr = services.GetRequiredService<PublishedContentRequest>();
//            var umbCtrlTypes = services.GetRequiredService<UmbracoControllerTypeCollection>();
//            return new UmbracoActionConstraint(umbCtx, pcr, umbCtrlTypes);
//        }
//    }
//}