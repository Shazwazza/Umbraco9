using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Umbraco.Core;
using Umbraco.Core.Plugins;

namespace Umbraco.Web.Plugins
{
    /// <summary>
    /// This will return candidate assemblies based on the assemblies found in App_Plugins
    /// </summary>
    public class PluginAssemblyProvider : IUmbracoAssemblyProvider
    {
        private readonly IFileProvider _fileProvider;
        private readonly IAssemblyLoadContextAccessor _loadContextAccessor;
        private readonly IAssemblyLoaderContainer _assemblyLoaderContainer;
        private readonly Lazy<IEnumerable<Assembly>> _candidates;
        private readonly ILogger _logger;

        public PluginAssemblyProvider(IFileProvider fileProvider, ILoggerFactory loggerFactory,
            IAssemblyLoadContextAccessor loadContextAccessor, IAssemblyLoaderContainer assemblyLoaderContainer)
        {
            _logger = loggerFactory.CreateLogger<PluginAssemblyProvider>();
            _fileProvider = fileProvider;
            _loadContextAccessor = loadContextAccessor;
            _assemblyLoaderContainer = assemblyLoaderContainer;
            _candidates = new Lazy<IEnumerable<Assembly>>(FindPluginAssemblies);
        }

        public IEnumerable<Assembly> CandidateAssemblies => _candidates.Value;

        private IEnumerable<Assembly> FindPluginAssemblies()
        {
            //TODO: Do not include any assemblies that have already been loaded or are referenced as normal reference
            // assemblies, this could be done if a developer included library/framework assemblies with their package.

            var content = _fileProvider.GetDirectoryContents("/App_Plugins");
            if (!content.Exists) yield break;
            foreach (var pluginDir in content.Where(x => x.IsDirectory))
            {
                var binDir = new DirectoryInfo(Path.Combine(pluginDir.PhysicalPath, "bin"));
                if (!binDir.Exists) continue;
                foreach (var assembly in GetAssembliesInFolder(binDir))
                {
                    yield return assembly;
                }
            }
        }

        /// <summary>
        /// Returns assemblies loaded from /bin folders inside of App_Plugins
        /// </summary>
        /// <param name="binPath"></param>
        /// <returns></returns>
        private IEnumerable<Assembly> GetAssembliesInFolder(DirectoryInfo binPath)
        {
            // Use the default load context
            var loadContext = _loadContextAccessor.Default;

            // Add the loader to the container so that any call to Assembly.Load will call the load context back (if it's not already loaded)
            using (_assemblyLoaderContainer.AddLoader(new DirectoryLoader(binPath, loadContext)))
            {
                foreach (var fileSystemInfo in binPath.GetFileSystemInfos("*.dll"))
                {
                    //// You should be able to use Assembly.Load()
                    //var assembly1 = Assembly.Load(AssemblyName.GetAssemblyName(fileSystemInfo.FullName));
                    var assembly2 = loadContext.Load(AssemblyName.GetAssemblyName(fileSystemInfo.FullName));

                    yield return assembly2;
                }
            }
        }
    }
}
