using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.Extensions.PlatformAbstractions;
using Umbraco.Core.Plugins;

namespace Umbraco.Web.Plugins
{

    //public class CompositeAssemblyProvider : DefaultAssemblyProvider
    //{
    //    private readonly IUmbracoAssemblyProvider[] _additionalProviders;
    //    private readonly string[] _referenceAssemblies;

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="libraryManager"></param>
    //    /// <param name="additionalProviders">
    //    /// If passed in will concat the assemblies returned from these 
    //    /// providers with the default assemblies referenced
    //    /// </param>
    //    /// <param name="referenceAssemblies">
    //    /// If passed in it will filter the candidate libraries to ones
    //    /// that reference the assembly names passed in. 
    //    /// (i.e. "MyProduct.Web", "MyProduct.Core" )
    //    /// </param>
    //    public CompositeAssemblyProvider(
    //        ILibraryManager libraryManager,
    //        IUmbracoAssemblyProvider[] additionalProviders = null,
    //        string[] referenceAssemblies = null) : base(libraryManager)
    //    {
    //        _additionalProviders = additionalProviders;
    //        _referenceAssemblies = referenceAssemblies;
    //    }

    //    /// <summary>
    //    /// Uses the default filter if a custom list of reference
    //    /// assemblies has not been provided
    //    /// </summary>
    //    protected override HashSet<string> ReferenceAssemblies
    //        => _referenceAssemblies == null
    //            ? base.ReferenceAssemblies
    //            : new HashSet<string>(_referenceAssemblies);
        
    //    /// <summary>
    //    /// Returns the base Libraries referenced along with any DLLs/Libraries
    //    /// returned from the custom IAssemblyProvider passed in
    //    /// </summary>
    //    /// <returns></returns>
    //    protected override IEnumerable<Library> GetCandidateLibraries()
    //    {
    //        var baseCandidates = base.GetCandidateLibraries();
    //        if (_additionalProviders == null) return baseCandidates;
    //        return baseCandidates               
    //            .Concat(
    //            _additionalProviders.SelectMany(provider => provider.CandidateAssemblies.Select(
    //                x => new Library(x.FullName, null, Path.GetDirectoryName(x.Location), null, Enumerable.Empty<string>(),
    //                    new[] { new AssemblyName(x.FullName) }))));
    //    }
    //}

    /// <summary>
    /// This is used for MVC to determine which assemblies to query for MVC specific details like Controllers
    /// </summary>
    public class MvcPluginAssemblyProvider : DefaultAssemblyProvider
    {
        private readonly IUmbracoAssemblyProvider _umbracoAssemblyProvider;

        public MvcPluginAssemblyProvider(ILibraryManager libraryManager, IUmbracoAssemblyProvider umbracoAssemblyProvider) : base(libraryManager)
        {
            _umbracoAssemblyProvider = umbracoAssemblyProvider;
        }

        /// <summary>
        /// We override this method to also include plugin 'libraries' so they are included when the base class determines candidates
        /// </summary>        
        protected override IEnumerable<Library> GetCandidateLibraries()
        {
            return base.GetCandidateLibraries().Concat(
                _umbracoAssemblyProvider.CandidateAssemblies.Select(
                    x => new Library(x.FullName, null, Path.GetDirectoryName(x.Location), null, Enumerable.Empty<string>(),
                        new[] {new AssemblyName(x.FullName)})));

        }
    }
}