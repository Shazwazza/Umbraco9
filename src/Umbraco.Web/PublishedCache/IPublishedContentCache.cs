using System;
using System.Threading.Tasks;
using Umbraco.Web.Models;

namespace Umbraco.Web.PublishedCache
{
    public interface IPublishedContentCache : IPublishedCache
    {
        Task<IPublishedContent> GetByRouteAsync(bool preview, string route);

        Task<string> GetRouteByIdAsync(bool preview, Guid contentId);
    }
}