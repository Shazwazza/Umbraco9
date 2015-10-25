using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Umbraco.Core.Services;

namespace Umbraco.Core
{
    public static class UmbracoCoreServices
    {
        public static IServiceCollection AddUmbracoCore(this IServiceCollection services)
        {
            services.AddSingleton<ITemplateService, TemplateService>();
            return services;
        }
    }
}
