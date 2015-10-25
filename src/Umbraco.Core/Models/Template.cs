using System;

namespace Umbraco.Core.Models
{
    public class Template : ITemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}