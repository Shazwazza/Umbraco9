using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace Umbraco.Core.Plugins
{    
    public class DirectoryLoader : IAssemblyLoader
    {
        private readonly IAssemblyLoadContext _context;
        private readonly DirectoryInfo _path;

        public DirectoryLoader(DirectoryInfo path, IAssemblyLoadContext context)
        {
            _path = path;
            _context = context;
        }

        public Assembly Load(AssemblyName assemblyName)
        {
            return _context.LoadFile(Path.Combine(_path.FullName, assemblyName.Name + ".dll"));
        }

        public IntPtr LoadUnmanagedLibrary(string name)
        {
            throw new NotImplementedException();
        }
    }
}
