using System;
using Umbraco.Core.Models;

namespace Umbraco.Core.Services
{
    
    public interface ITemplateService
    {
        ITemplate GetTemplate(string alias);
        ITemplate GetTemplate(Guid id);
    }
}
