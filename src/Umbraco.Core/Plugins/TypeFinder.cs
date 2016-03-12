using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Core.Collections;

namespace Umbraco.Core.Plugins
{
    /// <summary>
    /// A utility class to find all classes of a certain type by reflection in the current bin folder
    /// of the web application.
    /// </summary>
    public class TypeFinder : ITypeFinder
    {
        private readonly Lazy<HashSet<Assembly>> _localFilteredAssemblyCache = null;
        private readonly IEnumerable<IUmbracoAssemblyProvider> _assemblyProvider;
        private readonly ILogger _logger;

        public TypeFinder(ILoggerFactory loggerFactory, IEnumerable<IUmbracoAssemblyProvider> assemblyProvider, IEnumerable<Assembly> excludeFromResults = null)
        {
            _assemblyProvider = assemblyProvider;
            _logger = loggerFactory.CreateLogger<TypeFinder>();

            _localFilteredAssemblyCache = new Lazy<HashSet<Assembly>>(() =>
            {
                var localFilteredAssemblyCache = new HashSet<Assembly>();
                var assemblies = GetFilteredAssemblies(excludeFromResults, KnownAssemblyExclusionFilter);
                foreach (var a in assemblies)
                {
                    localFilteredAssemblyCache.Add(a);
                }
                return localFilteredAssemblyCache;
            });
        }

        /// <summary>
        /// Return a list of found local Assemblies excluding the known assemblies we don't want to scan
        /// and exluding the ones passed in and excluding the exclusion list filter, the results of this are
        /// cached for perforance reasons.
        /// </summary>
        /// <returns></returns>
        internal HashSet<Assembly> GetAssembliesWithKnownExclusions()
        {
            return _localFilteredAssemblyCache.Value;
        }

        /// <summary>
        /// Return a distinct list of found local Assemblies and exluding the ones passed in and excluding the exclusion list filter
        /// </summary>
        /// <param name="excludeFromResults"></param>
        /// <param name="exclusionFilter"></param>
        /// <returns></returns>
        private IEnumerable<Assembly> GetFilteredAssemblies(
            IEnumerable<Assembly> excludeFromResults = null,
            string[] exclusionFilter = null)
        {
            if (excludeFromResults == null)
                excludeFromResults = new HashSet<Assembly>();
            if (exclusionFilter == null)
                exclusionFilter = new string[] {};

            return _assemblyProvider.SelectMany(x => x.CandidateAssemblies)
                .Where(x => !excludeFromResults.Contains(x)
                            && !x.GlobalAssemblyCache
                            && !exclusionFilter.Any(f => x.FullName.StartsWith(f)));
        }

        /// <summary>
        /// this is our assembly filter to filter out known types that def dont contain types we'd like to find or plugins
        /// </summary>
        /// <remarks>
        /// NOTE the comma vs period... comma delimits the name in an Assembly FullName property so if it ends with comma then its an exact name match
        /// NOTE this means that "foo." will NOT exclude "foo.dll" but only "foo.*.dll"
        /// </remarks>
        internal static readonly string[] KnownAssemblyExclusionFilter = new[]
        {
            "mscorlib,",
            "System.",
            "Antlr3.",
            "Autofac.",
            "Autofac,",           
            "Dynamic,",            
            "Microsoft.",
            "Newtonsoft.",            
            "NuGet.",            
            "Lucene.",
            "Examine,",
            "Examine.",            
            "AutoMapper,",
            "AutoMapper.",            
            "Moq,"            
        };

        /// <summary>
        /// Finds any classes derived from the type T that contain the attribute TAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfTypeWithAttribute<T, TAttribute>()
            where TAttribute : Attribute
        {
            return FindClassesOfTypeWithAttribute<T, TAttribute>(GetAssembliesWithKnownExclusions(), true);
        }

        /// <summary>
        /// Finds any classes derived from the type T that contain the attribute TAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfTypeWithAttribute<T, TAttribute>(IEnumerable<Assembly> assemblies)
            where TAttribute : Attribute
        {
            return FindClassesOfTypeWithAttribute<T, TAttribute>(assemblies, true);
        }

        /// <summary>
        /// Finds any classes derived from the type T that contain the attribute TAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfTypeWithAttribute<T, TAttribute>(
            IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses)
            where TAttribute : Attribute
        {
            return FindClassesOfTypeWithAttribute<TAttribute>(typeof (T), assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// Finds any classes derived from the assignTypeFrom Type that contain the attribute TAttribute
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="assignTypeFrom"></param>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfTypeWithAttribute<TAttribute>(
            Type assignTypeFrom,
            IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses)
            where TAttribute : Attribute
        {
            if (assemblies == null) throw new ArgumentNullException("assemblies");

            return GetClasses(assignTypeFrom, assemblies, onlyConcreteClasses,
                //the additional filter will ensure that any found types also have the attribute applied.
                t => t.GetCustomAttributes<TAttribute>(false).Any());
        }

        /// <summary>
        /// Searches all filtered local assemblies specified for classes of the type passed in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>()
        {
            return FindClassesOfType<T>(GetAssembliesWithKnownExclusions(), true);
        }

        /// <summary>
        /// Returns all types found of in the assemblies specified of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses)
        {
            if (assemblies == null) throw new ArgumentNullException("assemblies");

            return GetClasses(typeof (T), assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// Returns all types found of in the assemblies specified of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies)
        {
            return FindClassesOfType<T>(assemblies, true);
        }

        /// <summary>
        /// Finds the classes with attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="onlyConcreteClasses">if set to <c>true</c> only concrete classes.</param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesWithAttribute<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses)
            where T : Attribute
        {
            return FindClassesWithAttribute(typeof (T), assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// Finds any classes with the attribute.
        /// </summary>
        /// <param name="attributeType">The attribute type </param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="onlyConcreteClasses">if set to <c>true</c> only concrete classes.</param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesWithAttribute(
            Type attributeType,
            IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses)
        {
            if (assemblies == null) throw new ArgumentNullException("assemblies");

            if (TypeHelper.IsTypeAssignableFrom<Attribute>(attributeType) == false)
                throw new ArgumentException("The type specified: " + attributeType + " is not an Attribute type");

            var foundAttributedTypes = new HashSet<Type>();

            var assemblyList = assemblies.ToArray();

            //find all assembly references that are referencing the attribute type's assembly since we
            //should only be scanning those assemblies because any other assembly will definitely not
            //contain a class that has this attribute.
            var referencedAssemblies = TypeHelper.GetReferencedAssemblies(attributeType, assemblyList);

            //get a list of non-referenced assemblies (we'll use this when we recurse below)
            var otherAssemblies = assemblyList.Where(x => referencedAssemblies.Contains(x) == false).ToArray();

            //loop through the referenced assemblies
            foreach (var a in referencedAssemblies)
            {
                //get all types in this assembly
                var allTypes = GetTypesWithFormattedException(a)
                    .ToArray();

                var attributedTypes = new Type[] {};
                try
                {
                    //now filter the types based on the onlyConcreteClasses flag, not interfaces, not static classes but have
                    //the specified attribute
                    attributedTypes = allTypes
                        .Where(t => (TypeHelper.IsNonStaticClass(t)
                                     && (onlyConcreteClasses == false || t.IsAbstract == false))
                            //the type must have this attribute
                                    && t.GetCustomAttributes(attributeType, false).Any())
                        .ToArray();
                }
                catch (TypeLoadException ex)
                {
                    _logger.LogError($"Could not query types on {a} assembly, this is most likely due to this assembly not being compatible with the current Umbraco version", ex);
                    continue;
                }

                //add the types to our list to return
                foreach (var t in attributedTypes)
                {
                    foundAttributedTypes.Add(t);
                }

                //get all attributes of the type being searched for
                var allAttributeTypes = allTypes.Where(attributeType.IsAssignableFrom);

                //now we need to include types that may be inheriting from sub classes of the attribute type being searched for
                //so we will search in assemblies that reference those types too.
                foreach (var subTypesInAssembly in allAttributeTypes.GroupBy(x => x.Assembly))
                {

                    //So that we are not scanning too much, we need to group the sub types:
                    // * if there is more than 1 sub type in the same assembly then we should only search on the 'lowest base' type.
                    // * We should also not search for sub types if the type is sealed since you cannot inherit from a sealed class
                    // * We should not search for sub types if the type is static since you cannot inherit from them.
                    var subTypeList = subTypesInAssembly
                        .Where(t => t.IsSealed == false && TypeHelper.IsStaticClass(t) == false)
                        .ToArray();

                    var baseClassAttempt = TypeHelper.GetLowestBaseType(subTypeList);

                    //if there's a base class amongst the types then we'll only search for that type.
                    //otherwise we'll have to search for all of them.
                    var subTypesToSearch = new HashSet<Type>();
                    if (baseClassAttempt.Success)
                    {
                        subTypesToSearch.Add(baseClassAttempt.Result);
                    }
                    else
                    {
                        foreach (var t in subTypeList)
                        {
                            subTypesToSearch.Add(t);
                        }
                    }

                    foreach (var typeToSearch in subTypesToSearch)
                    {
                        //recursively find the types inheriting from this sub type in the other non-scanned assemblies.
                        var foundTypes = FindClassesWithAttribute(typeToSearch, otherAssemblies, onlyConcreteClasses);

                        foreach (var f in foundTypes)
                        {
                            foundAttributedTypes.Add(f);
                        }
                    }

                }
            }

            return foundAttributedTypes;
        }


        /// <summary>
        /// Finds the classes with attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesWithAttribute<T>(IEnumerable<Assembly> assemblies)
            where T : Attribute
        {
            return FindClassesWithAttribute<T>(assemblies, true);
        }

        /// <summary>
        /// Finds the classes with attribute in filtered local assemblies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesWithAttribute<T>()
            where T : Attribute
        {
            return FindClassesWithAttribute<T>(GetAssembliesWithKnownExclusions());
        }


        #region Private methods

        /// <summary>
        /// Finds types that are assignable from the assignTypeFrom parameter and will scan for these types in the assembly
        /// list passed in, however we will only scan assemblies that have a reference to the assignTypeFrom Type or any type
        /// deriving from the base type.
        /// </summary>
        /// <param name="assignTypeFrom"></param>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <param name="additionalFilter">An additional filter to apply for what types will actually be included in the return value</param>
        /// <returns></returns>
        private IEnumerable<Type> GetClasses(
            Type assignTypeFrom,
            IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses,
            Func<Type, bool> additionalFilter = null)
        {
            //the default filter will always return true.
            if (additionalFilter == null)
            {
                additionalFilter = type => true;
            }

            var foundAssignableTypes = new HashSet<Type>();

            var assemblyList = assemblies.ToArray();

            //find all assembly references that are referencing the current type's assembly since we
            //should only be scanning those assemblies because any other assembly will definitely not
            //contain sub type's of the one we're currently looking for
            var referencedAssemblies = TypeHelper.GetReferencedAssemblies(assignTypeFrom, assemblyList);

            //get a list of non-referenced assemblies (we'll use this when we recurse below)
            var otherAssemblies = assemblyList.Where(x => referencedAssemblies.Contains(x) == false).ToArray();

            //loop through the referenced assemblies
            foreach (var a in referencedAssemblies)
            {
                //get all types in the assembly that are sub types of the current type
                var allSubTypes = GetTypesWithFormattedException(a)
                    .Where(assignTypeFrom.IsAssignableFrom)
                    .ToArray();

                var filteredTypes = new Type[] {};
                try
                {
                    //now filter the types based on the onlyConcreteClasses flag, not interfaces, not static classes
                    filteredTypes = allSubTypes
                        .Where(t => (TypeHelper.IsNonStaticClass(t)
                            //Do not include nested private classes - since we are in full trust now this will find those too!
                                     && t.IsNestedPrivate == false
                                     && (onlyConcreteClasses == false || t.IsAbstract == false)
                            //Do not include classes that are flagged to hide from the type finder
                                     && t.GetCustomAttribute<HideFromTypeFinderAttribute>() == null
                                     && additionalFilter(t)))
                        .ToArray();
                }
                catch (TypeLoadException ex)
                {
                    _logger.LogError($"Could not query types on {a} assembly, this is most likely due to this assembly not being compatible with the current Umbraco version", ex);
                    continue;
                }

                //add the types to our list to return
                foreach (var t in filteredTypes)
                {
                    foundAssignableTypes.Add(t);
                }

                //now we need to include types that may be inheriting from sub classes of the type being searched for
                //so we will search in assemblies that reference those types too.
                foreach (var subTypesInAssembly in allSubTypes.GroupBy(x => x.Assembly))
                {

                    //So that we are not scanning too much, we need to group the sub types:
                    // * if there is more than 1 sub type in the same assembly then we should only search on the 'lowest base' type.
                    // * We should also not search for sub types if the type is sealed since you cannot inherit from a sealed class
                    // * We should not search for sub types if the type is static since you cannot inherit from them.
                    var subTypeList = subTypesInAssembly
                        .Where(t => t.IsSealed == false && TypeHelper.IsStaticClass(t) == false)
                        .ToArray();

                    var baseClassAttempt = TypeHelper.GetLowestBaseType(subTypeList);

                    //if there's a base class amongst the types then we'll only search for that type.
                    //otherwise we'll have to search for all of them.
                    var subTypesToSearch = new HashSet<Type>();
                    if (baseClassAttempt.Success)
                    {
                        subTypesToSearch.Add(baseClassAttempt.Result);
                    }
                    else
                    {
                        foreach (var t in subTypeList)
                        {
                            subTypesToSearch.Add(t);
                        }
                    }

                    foreach (var typeToSearch in subTypesToSearch)
                    {
                        //recursively find the types inheriting from this sub type in the other non-scanned assemblies.
                        var foundTypes = GetClasses(typeToSearch, otherAssemblies, onlyConcreteClasses, additionalFilter);

                        foreach (var f in foundTypes)
                        {
                            foundAssignableTypes.Add(f);
                        }
                    }

                }

            }
            return foundAssignableTypes;
        }

        internal IEnumerable<Type> GetTypesWithFormattedException(Assembly a)
        {
            //if the assembly is dynamic, do not try to scan it
            if (a.IsDynamic)
                return Enumerable.Empty<Type>();

            var getAll = a.GetCustomAttribute<AllowPartiallyTrustedCallersAttribute>() == null;

            try
            {
                //we need to detect if an assembly is partially trusted, if so we cannot go interrogating all of it's types
                //only its exported types, otherwise we'll get exceptions.
                return getAll ? a.GetTypes() : a.GetExportedTypes();
            }
            catch (TypeLoadException ex) // GetExportedTypes *can* throw TypeLoadException!
            {
                var sb = new StringBuilder();
                AppendCouldNotLoad(sb, a, getAll);
                AppendLoaderException(sb, ex);

                // rethrow as ReflectionTypeLoadException (for consistency) with new message
                throw new ReflectionTypeLoadException(new Type[0], new Exception[] {ex}, sb.ToString());
            }
            catch (ReflectionTypeLoadException rex) // GetTypes throws ReflectionTypeLoadException
            {
                var sb = new StringBuilder();
                AppendCouldNotLoad(sb, a, getAll);
                foreach (var loaderException in rex.LoaderExceptions.WhereNotNull())
                    AppendLoaderException(sb, loaderException);

                // rethrow with new message
                throw new ReflectionTypeLoadException(rex.Types, rex.LoaderExceptions, sb.ToString());
            }
        }

        private void AppendCouldNotLoad(StringBuilder sb, Assembly a, bool getAll)
        {
            sb.Append("Could not load ");
            sb.Append(getAll ? "all" : "exported");
            sb.Append(" types from \"");
            sb.Append(a.FullName);
            sb.AppendLine("\" due to LoaderExceptions, skipping:");
        }

        private void AppendLoaderException(StringBuilder sb, Exception loaderException)
        {
            sb.Append(". ");
            sb.Append(loaderException.GetType().FullName);

            var tloadex = loaderException as TypeLoadException;
            if (tloadex != null)
            {
                sb.Append(" on ");
                sb.Append(tloadex.TypeName);
            }

            sb.Append(": ");
            sb.Append(loaderException.Message);
            sb.AppendLine();
        }

        #endregion
    }
}
