using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.Dnx.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.OptionsModel;
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

            services.AddCaching();
            services.AddSession();
            services.AddMvc();
            //services.AddAuthentication();
            //services.AddAuthorization();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "umbraco-backoffice",
                    builder => builder
                        .AddAuthenticationSchemes("umbraco-backoffice")
                        .RequireAuthenticatedUser()
                        .RequireClaim("umbraco-backoffice")
                    );
            });

            services.Configure<MvcOptions>(options =>
            {
                options.ModelBinders.Insert(0, new PublishedContentModelBinder());  
            });

            //services.AddIdentity<BackOfficeUser, IdentityRole>();

            services.AddSingleton<IControllerActivator, UmbracoControllerActivator>();
            //services.AddSingleton<UmbracoAssemblyProvider>();
            services.AddSingleton<IUmbracoConfig, UmbracoConfig>();
            services.AddSingleton<UmbracoControllerTypeCollection>();
            services.AddSingleton<SurfaceFormHelper>();
            services.AddSingleton<IControllerPropertyActivator, ProxiedViewDataDictionaryPropertyActivator>();

            services.AddScoped<UmbracoContext>();
            services.AddScoped<RoutingContext>();
            services.AddScoped<PublishedContentRequest>();            
            
            //TODO: default is no last chance finder (for now)
            services.AddScoped<ILastChanceContentFinder>(provider => (ILastChanceContentFinder) null);
            services.AddScoped<UrlProvider>(provider => new UrlProvider(
                provider.GetRequiredService<UmbracoContext>(),
                provider.GetServices<IUrlProvider>(),
                UrlProviderMode.Auto));            
            

            return services;
        }

        public static void UseUmbraco(this IApplicationBuilder app)
        {
            app.UseSession();

            //TODO: This is currently here to authenticate every back office requests until we implement logins/etc...
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/Umbraco"))
                {
                    if (!context.User.Identities.Any(identity => identity.IsAuthenticated))
                    {
                        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, "admin"),
                            new Claim("umbraco-backoffice", "yes")
                        }, "umbraco-backoffice"));

                        await context.Authentication.SignInAsync("umbraco-backoffice", user);
                    }                    
                }
                await next();
            });

            app.UseCookieAuthentication(options =>
            {
                options.CookiePath = "Umbraco";
                options.CookieName = "UMB_CONTEXT";
                options.LoginPath = "/Umbraco/Login";
                options.AutomaticAuthenticate = true;
                options.AuthenticationScheme = "umbraco-backoffice";
            });

            app.UseMvc(routes =>
            {
                //Creates the Umbraco catch all route with the Umbraco router
                routes.DefaultHandler = new UmbracoRouter(routes.DefaultHandler);
                routes.MapRoute("Umbraco", "{*_umbracoRoute:Required}");
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
