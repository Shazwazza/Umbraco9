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
        private readonly Lazy<IEnumerable<Assembly>> _candidates;
        private readonly ILogger _logger;

        public PluginAssemblyProvider(IFileProvider fileProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PluginAssemblyProvider>();
            _fileProvider = fileProvider;
            _candidates = new Lazy<IEnumerable<Assembly>>(FindPluginAssemblies);
        }

        public IEnumerable<Assembly> CandidateAssemblies => _candidates.Value;

        private IEnumerable<Assembly> FindPluginAssemblies()
        {
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
            var loadContext = PlatformServices.Default.AssemblyLoadContextAccessor.Default;

            // Add the loader to the container so that any call to Assembly.Load will
            // call the load context back (if it's not already loaded)
            using (PlatformServices.Default.AssemblyLoaderContainer.AddLoader(
                new DirectoryLoader(binPath, loadContext)))
            {
                foreach (var fileSystemInfo in binPath.GetFileSystemInfos("*.dll"))
                {
                    //// You should be able to use Assembly.Load()
                    //var assembly1 = Assembly.Load(new AssemblyName("SomethingElse"));

                    // Or call load on the context directly
                    var assembly2 = loadContext.Load(Path.GetFileNameWithoutExtension(fileSystemInfo.Name));

                    //foreach (var definedType in assembly2.DefinedTypes)
                    //{
                    //    _logger.LogDebug("Found type {0}", definedType.FullName);
                    //}

                    yield return assembly2;
                }
            }
        }
    }
}
