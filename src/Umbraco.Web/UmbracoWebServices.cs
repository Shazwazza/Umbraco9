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
using Microsoft.Framework.DependencyInjection.Extensions;
using Microsoft.Framework.OptionsModel;
using Umbraco.Core;
using Umbraco.Web.Controllers;
using Umbraco.Web.Routing;

namespace Umbraco.Web
{
    public static class UmbracoWebServices
    {
        public static IServiceCollection AddUmbraco(this IServiceCollection services)
        {
            services.AddUmbracoCore();

            services.Configure<MvcOptions>(options =>
            {
                options.ModelBinders.Insert(0, new PublishedContentModelBinder());  
            });

            services.AddSingleton<IControllerActivator, UmbracoControllerActivator>();
            //services.AddSingleton<UmbracoAssemblyProvider>();
            services.AddSingleton<IUmbracoConfig, UmbracoConfig>();
            services.AddSingleton<UmbracoControllerTypeCollection>();
            services.AddSingleton<SurfaceFormHelper>();
            services.AddSingleton<IControllerPropertyActivator, ProxiedViewDataDictionaryPropertyActivator>();

            services.AddScoped<UmbracoContext>();
            services.AddScoped<RoutingContext>();
            services.AddScoped<PublishedContentRequest>();            
            services.AddScoped<IContentFinder, PathContentFinder>();
            //TODO: default is no last chance finder (for now)
            services.AddScoped<ILastChanceContentFinder>(provider => (ILastChanceContentFinder) null);
            services.AddScoped<UrlProvider>(provider => new UrlProvider(
                provider.GetRequiredService<UmbracoContext>(),
                provider.GetServices<IUrlProvider>(),
                UrlProviderMode.Auto));            
            services.AddScoped<IUrlProvider, DefaultUrlProvider>();

            return services;
        }

        public static void UseUmbraco(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                //Creates the Umbraco catch all route with the Umbraco router
                routes.DefaultHandler = new UmbracoRouter(routes.DefaultHandler);
                routes.MapRoute("Umbraco", "{*_umbracoRoute:Required}");
                routes.MapRoute("UmbracoSurface", "{*_surfaceRoute:Required}");
            });
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
