using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Umbraco.Core.Plugins;
using Umbraco.Web.Routing;
using Xunit;

namespace Umbraco.Tests.Plugins
{
    /// <summary>
    /// Tests for typefinder
    /// </summary>
    public class TypeFinderTests
    {
        /// <summary>
        /// List of assemblies to scan
        /// </summary>
        private Assembly[] _assemblies;

        private ITypeFinder _typeFinder;

        public TypeFinderTests()
        {
            _assemblies = new[]
                {
                    GetType().Assembly,                    
                    typeof(System.Guid).Assembly,
                    typeof(Xunit.Assert).Assembly,
                    typeof(Microsoft.CSharp.CSharpCodeProvider).Assembly,
                    typeof(System.Xml.NameTable).Assembly,
                    typeof(ITypeFinder).Assembly,
                    typeof(UmbracoRouter).Assembly
                };
            _typeFinder = new TypeFinder(
                Mock.Of<ILoggerFactory>(), 
                new TypeHelper(), 
                new[] {new StaticAssemblyProvider(_assemblies)});
        }

        [Fact]
        public void Find_Class_Of_Type_With_Attribute()
        {

            var typesFound = _typeFinder.FindClassesOfTypeWithAttribute<TestEditor, MyTestAttribute>(_assemblies);
            Assert.Equal(2, typesFound.Count());
        }

        [Fact]
        public void Find_Classes_Of_Type()
        {
            var typesFound = _typeFinder.FindClassesOfType<IUmbracoAssemblyProvider>(_assemblies);
            
            Assert.Equal(5, typesFound.Count());
            
        }

        [Fact]
        public void Find_Classes_With_Attribute()
        {
            var typesFound = _typeFinder.FindClassesWithAttribute<MyTestAttribute>(_assemblies);
            Assert.Equal(2, typesFound.Count());
        }

        public class MyAssemblyProvider : IUmbracoAssemblyProvider
        {
            public IEnumerable<Assembly> CandidateAssemblies
            {
                get { throw new NotImplementedException(); }
            }
        }

        public class MySuperAssemblyProvider : MyAssemblyProvider
        {

        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class MyTestAttribute : Attribute
        {

        }

        public abstract class TestEditor
        {

        }

        [MyTest]
        public class BenchmarkTestEditor : TestEditor
        {

        }

        [MyTest]
        public class MyOtherTestEditor : TestEditor
        {

        }

    }
}
