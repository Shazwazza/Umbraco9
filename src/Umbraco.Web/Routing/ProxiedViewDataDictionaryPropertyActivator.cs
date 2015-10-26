using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace Umbraco.Web.Routing
{
    /// <summary>
    /// This is used to wire up the ViewDataDictionary on Umbraco controller's when they are proxied to from SurfaceController results
    /// </summary>
    internal class ProxiedViewDataDictionaryPropertyActivator : IControllerPropertyActivator
    {
        private readonly ConcurrentDictionary<Type, PropertyInfo[]> _activateActions;
        private readonly Func<Type, PropertyInfo[]> _getPropertiesToActivate;

        public ProxiedViewDataDictionaryPropertyActivator()
        {
            _activateActions = new ConcurrentDictionary<Type, PropertyInfo[]>();
            _getPropertiesToActivate = GetPropertiesToActivate;
        }

        public void Activate(ActionContext actionContext, object controller)
        {
            var vdd = GetViewDataDictionary(actionContext);
            if (vdd == null) return;

            var controllerType = controller.GetType();
            var propertiesToActivate = _activateActions.GetOrAdd(
                controllerType,
                _getPropertiesToActivate);

            for (var i = 0; i < propertiesToActivate.Length; i++)
            {
                var activateInfo = propertiesToActivate[i];
                activateInfo.SetValue(controller, vdd);
            }
        }

        private PropertyInfo[] GetPropertiesToActivate(Type type)
        {
            return type.GetProperties().Where(x => x.GetCustomAttribute<ViewDataDictionaryAttribute>() != null).ToArray();
        }

        private ViewDataDictionary GetViewDataDictionary(ActionContext context)
        {
            if (context.RouteData.DataTokens.ContainsKey("umbraco-vdd"))
            {
                return context.RouteData.DataTokens["umbraco-vdd"] as ViewDataDictionary;
            }
            return null;
        }
    }
}