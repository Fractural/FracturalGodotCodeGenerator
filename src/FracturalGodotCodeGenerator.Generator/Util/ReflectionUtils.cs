using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FracturalGodotCodeGenerator.Generator.Util
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static IEnumerable<T> GetInstances<T>()
        {
            var baseType = typeof(T);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetLoadableTypes())
                .Where
                (
                    t => baseType.IsAssignableFrom(t)                  //Derives from base
                      && !t.IsAbstract                                 //Is not abstract
                      && (t.GetConstructor(Type.EmptyTypes) != null)   //Has default constructor
                );


            foreach (var t in types)
            {
                yield return (T)Activator.CreateInstance(t);
            }
        }
    }
}
