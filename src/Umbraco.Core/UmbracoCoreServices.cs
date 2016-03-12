using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Core.Plugins;
using Umbraco.Core.Services;

namespace Umbraco.Core
{
    public static class UmbracoCoreServices
    {
        public static IServiceCollection AddUmbracoCore(this IServiceCollection services)
        {
            services.AddSingleton<IUmbracoAssemblyProvider, ReferencedAssemblyProvider>();
            services.AddSingleton<ITypeFinder, TypeFinder>();

            return services;
        }
    }
}
