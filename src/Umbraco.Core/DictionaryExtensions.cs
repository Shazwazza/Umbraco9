using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.WebEncoders;

namespace Umbraco.Core
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Converts a dictionary object to a query string representation such as:
        /// firstname=shannon&lastname=deminick
        /// </summary>
        /// <param name="d"></param>
        /// <param name="urlEncoder"></param>
        /// <returns></returns>
        public static string ToQueryString(this IDictionary<string, object> d, IUrlEncoder urlEncoder)
        {
            if (!d.Any()) return "";

            var builder = new StringBuilder();
            foreach (var i in d)
            {
                builder.Append($"{urlEncoder.UrlEncode(i.Key)}={(i.Value == null ? string.Empty : urlEncoder.UrlEncode(i.Value.ToString()))}&");
            }
            return builder.ToString().TrimEnd('&');
        }
    }
}