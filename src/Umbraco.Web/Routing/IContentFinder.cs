using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Umbraco.Web.Models;

namespace Umbraco.Web.Routing
{
    public interface IContentFinder
    {
        Task<bool> TryFindContentAsync(RouteData routeData);
    }
}