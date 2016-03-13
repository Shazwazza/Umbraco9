using System.Collections.Generic;
using System.Reflection;

namespace Umbraco.Core.Plugins
{
    public class StaticAssemblyProvider : IUmbracoAssemblyProvider
    {
        public StaticAssemblyProvider(IEnumerable<Assembly> assemblies)
        {
            CandidateAssemblies = assemblies;
        }

        public IEnumerable<Assembly> CandidateAssemblies { get; }
    }
}