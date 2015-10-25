using System;

namespace Umbraco.Core.Models
{
    public interface ITemplate
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Alias { get; set; }
    }
}