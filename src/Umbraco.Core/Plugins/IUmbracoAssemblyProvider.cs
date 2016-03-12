using System.Collections.Generic;
using System.Reflection;

namespace Umbraco.Core.Plugins
{
    //TODO: Copied locally since we don't want to reference MVC from Core this will change with rc2
    public interface IUmbracoAssemblyProvider
    {
        IEnumerable<Assembly> CandidateAssemblies { get; }
    }
}