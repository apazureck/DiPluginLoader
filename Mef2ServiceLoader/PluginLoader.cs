using FasterReflection;
using GlobExpressions;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;

namespace Mef2ServiceLoader
{
    public class PluginLoader
    {
        List<CompositionHost> containers = new List<CompositionHost>();
        List<AssemblyEntry> reflectionOnlyAssemblies = new List<AssemblyEntry>();

        public PluginLoader(string pattern) : this(new string[] { pattern })
        {

        }

        /// <summary>
        /// Creates a new plugin loader
        /// </summary>
        /// <param name="root">root folder, default is relative to executing assemlby</param>
        /// <param name="patterns"></param>
        public PluginLoader(IEnumerable<string> patterns = null)
        {
            var files = new List<string>();
            if (patterns == null)
                files.AddRange(Glob.Files(".", "**/*.dll"));
            else
                foreach (var pattern in patterns)
                    files.AddRange(Glob.Files(".", pattern));

            foreach (var file in files)
            {
                try
                {
                    var builder = new ReflectionMetadataBuilder();
                    builder.AddAssembly(file);
                    builder.AddReferenceOnlyAssemblyByType<object>(); // adds the corlib
                    reflectionOnlyAssemblies.Add(new AssemblyEntry(builder, file));
                }
                catch (Exception)
                {
                    
                }
            }
        }

        public List<Type> GetExports<T>() where T : class
        {
            IEnumerable<TypeDefinition> foundTypes;
            foreach(AssemblyEntry a in reflectionOnlyAssemblies)
            {
                a.AddAssemblyByType<T>();
            }
            if (typeof(T).IsInterface)
            {
                foundTypes = reflectionOnlyAssemblies.SelectMany(x =>
                {
                    return x.ReflectionOnlyAssembly.TypeDefinitions.Where(t =>
                    {
                        return t.AllInterfaces.Any(i =>
                        {
                            return i.FullName == typeof(T).FullName;
                        });
                    });
                });
            }
            else
                foundTypes = reflectionOnlyAssemblies.SelectMany(x => x.ReflectionOnlyAssembly.TypeDefinitions.Where(t => t.FullName == typeof(T).FullName || t.HasBaseType<T>()));
            var ret = new List<Type>();

            foreach(var foundType in foundTypes)
            {
                var fassembly = reflectionOnlyAssemblies.First(x => x.AssemblyName == foundType.AssemblyName);
                ret.Add(fassembly.Assembly.GetType(foundType.FullName));
            }
            return ret;
        }
    }
}
