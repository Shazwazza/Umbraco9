using System;
using Umbraco.Core.Models;

namespace Umbraco.Core.Services
{
    public class TemplateService : ITemplateService
    {
        public ITemplate GetTemplate(string alias)
        {
            return new Template
            {
                Alias = alias,
                Id = Guid.NewGuid(),
                Name = alias
            };
        }

        public ITemplate GetTemplate(Guid id)
        {
            return new Template
            {
                Alias = "Test",
                Id = id,
                Name = "Test"
            };
        }
    }
}