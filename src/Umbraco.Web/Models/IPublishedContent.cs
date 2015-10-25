using System;

namespace Umbraco.Web.Models
{
    public interface IPublishedContent
    {
        Guid Id { get; set; }
        string ContentType { get; set; }
        string Name { get; set; }
        string View { get; set; }
        Guid TemplateId { get; set; }
    }
}