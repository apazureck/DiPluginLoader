using FasterReflection;
using GlobExpressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mef2ServiceLoader
{
    public class PluginLoader
    {
        List<CompositionHost> containers = new List<CompositionHost>();
        List<AssemblyEntry> reflectionOnlyAssemblies = new List<AssemblyEntry>();

        /// <summary>
        /// Creates a new plugin loader
        /// </summary>
        /// <param name="root">root folder, default is relative to executing assemlby</param>
        /// <param name="pattern"></param>
        public PluginLoader(string pattern = null)
        {
            IEnumerable<string> files = Glob.Files(".", pattern ?? "**/*.dll");
            foreach (var file in files)
            {
                try
                {
                    var builder = new ReflectionMetadataBuilder();
                    builder.AddAssembly(file);
                    builder.AddReferenceOnlyAssemblyByType<object>(); // adds the corlib
                    reflectionOnlyAssemblies.Add(new AssemblyEntry(builder, file));
                }
                catch (Exception ex)
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
                        return t.Interfaces.Any(i =>
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
