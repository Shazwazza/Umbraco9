using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;

namespace Umbraco.Core.Plugins
{
    /// <summary>
    /// This will return candidate assemblies based on the currently loaded ILibraryManager (i.e. referenced assemblies)
    /// </summary>
    public class ReferencedAssemblyProvider : IUmbracoAssemblyProvider
    {
        private readonly ILibraryManager _libraryManager;
        private readonly IAssemblyLoadContextAccessor _loadContextAccessor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="libraryManager"></param>
        /// <param name="loadContextAccessor"></param>
        public ReferencedAssemblyProvider(ILibraryManager libraryManager,
            IAssemblyLoadContextAccessor loadContextAccessor)
        {
            _libraryManager = libraryManager;
            _loadContextAccessor = loadContextAccessor;
        }

        /// <summary>
        /// Gets the set of assembly names that are used as root for discovery of umbraco plugins
        /// </summary>
        protected HashSet<string> ReferenceAssemblies { get; } = new HashSet<string>(StringComparer.Ordinal)
        {
            "Umbraco.Core",
            "Umbraco.Web"
        };

        public IEnumerable<Assembly> CandidateAssemblies
        {
            get { return GetCandidateLibraries().SelectMany(l => l.Assemblies).Select(Load); }
        }

        protected IEnumerable<Library> GetCandidateLibraries()
        {
            return ReferenceAssemblies == null
                ? Enumerable.Empty<Library>()
                : ReferenceAssemblies.SelectMany(_libraryManager.GetReferencingLibraries).Distinct().Where(IsCandidateLibrary);
        }

        private Assembly Load(AssemblyName assemblyName)
        {
            return _loadContextAccessor.Default.Load(assemblyName);
        }

        private bool IsCandidateLibrary(Library library)
        {
            return !ReferenceAssemblies.Contains(library.Name);
        }
    }
}