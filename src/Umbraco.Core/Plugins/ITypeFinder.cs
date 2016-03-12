using System;
using System.Collections.Generic;
using System.Reflection;

namespace Umbraco.Core.Plugins
{
    public interface ITypeFinder
    {
        IEnumerable<Type> FindClassesOfType<T>();
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies);
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses);
        IEnumerable<Type> FindClassesOfTypeWithAttribute<TAttribute>(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses) where TAttribute : Attribute;
        IEnumerable<Type> FindClassesOfTypeWithAttribute<T, TAttribute>() where TAttribute : Attribute;
        IEnumerable<Type> FindClassesOfTypeWithAttribute<T, TAttribute>(IEnumerable<Assembly> assemblies) where TAttribute : Attribute;
        IEnumerable<Type> FindClassesOfTypeWithAttribute<T, TAttribute>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses) where TAttribute : Attribute;
        IEnumerable<Type> FindClassesWithAttribute(Type attributeType, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses);
        IEnumerable<Type> FindClassesWithAttribute<T>() where T : Attribute;
        IEnumerable<Type> FindClassesWithAttribute<T>(IEnumerable<Assembly> assemblies) where T : Attribute;
        IEnumerable<Type> FindClassesWithAttribute<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses) where T : Attribute;
    }
}