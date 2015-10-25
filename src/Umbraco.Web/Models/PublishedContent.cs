using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Umbraco.Web.Models
{   

    public class PublishedContent : IPublishedContent
    {
        public Guid Id { get; set; }
        public Guid TemplateId { get; set; }
        public string Name { get; set; }
        public string View { get; set; }
        public string ContentType { get; set; }
    }
}
