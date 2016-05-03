using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using Umbraco.Core;

namespace Umbraco.Web.Controllers
{
    /// <summary>
    /// Used to resolve all controler types of type UmbracoController
    /// </summary>
    public sealed class UmbracoControllerTypeCollection
    {
        private readonly IAssemblyProvider _assProvider;

        public UmbracoControllerTypeCollection(IActionDescriptorsCollectionProvider actionDescriptorProviderContext)
        {
            if (actionDescriptorProviderContext == null) throw new ArgumentNullException(nameof(actionDescriptorProviderContext));
         
            _umbracoControllerDescriptors = new Lazy<ControllerActionDescriptor[]>(() =>
            {
                return actionDescriptorProviderContext.ActionDescriptors.Items
                    .OfType<ControllerActionDescriptor>()
                    .Where(x => typeof (IUmbracoController).GetTypeInfo().IsAssignableFrom(x.ControllerTypeInfo))
                    //.Where(x => typeof (UmbracoController).GetTypeInfo().IsAssignableFrom(x.ControllerTypeInfo))
                    .ToArray();
            });
        }

        private readonly Lazy<ControllerActionDescriptor[]> _umbracoControllerDescriptors;

        public IEnumerable<TypeInfo> UmbracoControllerTypes => _umbracoControllerDescriptors.Value.Select(x => x.ControllerTypeInfo);

        public string GetControllerName(string name)
        {
            var found = (_umbracoControllerDescriptors.Value.FirstOrDefault(t => t.ControllerName.InvariantEquals(name)));
            return found?.ControllerName;
        }

        public bool ContainsControllerName(string name)
        {
            return (_umbracoControllerDescriptors.Value.Any(t => t.ControllerName.InvariantEquals(name)));
        }

        public string GetControllerActionName(string controllerName, string templateAlias)
        {
            var found = (_umbracoControllerDescriptors.Value
                .FirstOrDefault(t => t.ControllerName.InvariantEquals(controllerName) && t.Name.InvariantEquals(templateAlias)));
            return found == null ? "Index" : found.Name;
        }
    }
}