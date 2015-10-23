using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace Umbraco.Web.Controllers
{
    //NOTE: This is used to ensures that the VDD is merged over to our proxied controller when a surface
    // controller is executed. 
    public class UmbracoControllerActivator : DefaultControllerActivator
    {
        /// <summary>
        /// Creates a new <see cref="T:Microsoft.AspNet.Mvc.Controllers.DefaultControllerActivator"/>.
        /// </summary>
        /// <param name="typeActivatorCache">The <see cref="T:Microsoft.AspNet.Mvc.Infrastructure.ITypeActivatorCache"/>.</param>
        public UmbracoControllerActivator(ITypeActivatorCache typeActivatorCache) : base(typeActivatorCache)
        {
        }
        
        //protected override IReadOnlyDictionary<Type, Func<ActionContext, object>> CreateValueAccessorLookup()
        //{
        //    var baseLookup = base.CreateValueAccessorLookup();

        //    var newLookup = new Dictionary<Type, Func<ActionContext, object>>();
        //    foreach (var i in baseLookup)
        //    {
        //        newLookup.Add(i.Key, i.Value);
        //    }

        //    newLookup[typeof(ViewDataDictionary)] = context =>
        //    {
        //        if (context.HttpContext.Items["vdd"] != null)
        //        {
        //            return context.HttpContext.Items["vdd"];
        //        }
        //        return baseLookup[typeof(ViewDataDictionary)](context);
        //    };


        //    return newLookup;
        //}

    }
}