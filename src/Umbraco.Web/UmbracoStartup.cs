using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Umbraco.Web.Controllers;

namespace Umbraco.Web
{
    public static class UmbracoStartup
    {
        public static IServiceCollection AddUmbraco(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new SurfaceControllerConvention());
            });

            services.AddSingleton<IControllerActivator, UmbracoControllerActivator>();
            //services.AddSingleton<UmbracoAssemblyProvider>();
            services.AddSingleton<IUmbracoConfig, UmbracoConfig>();
            services.AddScoped<UmbracoContext>();

            return services;
        }

        public static void UseUmbraco(this IApplicationBuilder app)
        {
            //app.UseMvc();
            //app.UseMvc(routes =>
            //{
            //    //routes.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");

            //    //routes.MapRoute("Umbraco",
            //    //    //catch all
            //    //    "{*_umbracoRoute:Required}",
            //    //    //route constraint value
            //    //    new { umbraco = "yes", action = "Index" }
            //    //    );
            //});
        }
    }

    //public class UmbracoActionDescriptorProvider : IActionDescriptorProvider
    //{
    //    public int Order
    //    {
    //        get
    //        {
    //            //return DefaultOrder.DefaultFrameworkSortOrder;
    //            return 0;
    //        }
    //    }

    //    public void Invoke(ActionDescriptorProviderContext context, Action callNext)
    //    {
    //        foreach(var result in context.Results.OfType< ControllerActionDescriptor>())
    //        {
    //            if (typeof(UmbracoController).GetTypeInfo().IsAssignableFrom(
    //                    result.ControllerDescriptor.ControllerTypeInfo
    //                ))
    //            {
    //                var found = result.RouteConstraints.Where(x => x.RouteKey != "umbraco").ToArray();
    //                var controllerName = found.First(x => x.RouteKey == "controller").RouteValue;
    //                foreach(var f in found)
    //                {
    //                    result.RouteConstraints.Remove(f);
    //                }
    //                //result.RouteConstraints.Clear();
    //                result.RouteConstraints.Add(new UmbracoRouteConstraintAttribute());

    //                //you could do this instead of casting if you wanted
    //                result.Properties.Add("controllerName", controllerName);
    //            }
    //        }

    //        callNext();
    //    }
    //}



    //public class UmbracoAssemblyProvider : IAssemblyProvider
    //{
    //    // List of Mvc assemblies that we'll use as roots for controller discovery.
    //    private static readonly HashSet<string> _mvcAssemblyList = new HashSet<string>(StringComparer.Ordinal)
    //    {
    //        //TODO: Put Umbraco.Core and Umbraco.Web here
    //        //TODO: If we have a single assembly that just contains plugin types/interfaces,
    //        // the scan will be much quicker because it's a single reference
    //        // and then the recursive scan of ref'd assemblies don't get included in the load.

    //        "Microsoft.AspNet.Mvc",
    //        "Microsoft.AspNet.Mvc.Core",
    //        "Microsoft.AspNet.Mvc.ModelBinding",
    //        "Microsoft.AspNet.Mvc.Razor",
    //        "Microsoft.AspNet.Mvc.Razor.Host",
    //        "Microsoft.AspNet.Mvc.Rendering",
    //    };

    //    private readonly ILibraryManager _libraryManager;

    //    public UmbracoAssemblyProvider(ILibraryManager libraryManager)
    //    {

    //        _libraryManager = libraryManager;
    //    }

    //    public IEnumerable<Assembly> CandidateAssemblies
    //    {
    //        get
    //        {
    //            return GetCandidateLibraries().SelectMany(l => l.LoadableAssemblies).Select(Load);
    //        }
    //    }

    //    internal IEnumerable<ILibraryInformation> GetCandidateLibraries()
    //    {
    //        // GetReferencingLibraries returns the transitive closure of referencing assemblies
    //        // for a given assembly. In our case, we'll gather all assemblies that reference
    //        // any of the primary Mvc assemblies while ignoring Mvc assemblies.
    //        return _mvcAssemblyList.SelectMany(_libraryManager.GetReferencingLibraries)
    //                               .Distinct()
    //                               .Where(IsCandidateLibrary);
    //    }

    //    private static Assembly Load(AssemblyName assemblyName)
    //    {
    //        var ass = Assembly.Load(assemblyName);

    //        //NOTE: This will tell us if the assembly is the same as before/changed/etc...
    //        //var id = ass.ManifestModule.ModuleVersionId;

    //        return ass;
    //    }

    //    private static bool IsCandidateLibrary(ILibraryInformation library)
    //    {
    //        return !_mvcAssemblyList.Contains(library.Name);
    //    }
    //}
}
