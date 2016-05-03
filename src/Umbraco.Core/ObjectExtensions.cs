using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Umbraco.Core
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Turns object into dictionary
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ignoreProperties">Properties to ignore</param>
        /// <returns></returns>
        public static IDictionary<string, TVal> ToDictionary<TVal>(this object o, params string[] ignoreProperties)
        {
            if (o != null)
            {
#if DNX46
                var props = TypeDescriptor.GetProperties(o).Cast<PropertyDescriptor>();
#else
                var props = o.GetType().GetTypeInfo().DeclaredProperties;
#endif
                var d = new Dictionary<string, TVal>();
                foreach (var prop in props.Where(x => !ignoreProperties.Contains(x.Name)))
                {
                    var val = prop.GetValue(o);
                    if (val != null)
                    {
                        d.Add(prop.Name, (TVal)val);
                    }
                }
                return d;
            }
            return new Dictionary<string, TVal>();
        }
    }
}