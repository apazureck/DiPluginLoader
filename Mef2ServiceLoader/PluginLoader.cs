using FasterReflection;
using GlobExpressions;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace Mef2ServiceLoader
{
    public class PluginLoader
    {
        List<CompositionHost> containers = new List<CompositionHost>();
        private ReflectionMetadataResult metaData;

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

            var builder = new ReflectionMetadataBuilder();

            foreach (var file in files)
            {
                try
                {
                    builder.AddAssembly(file);
                }
                catch (Exception)
                {

                }
            }

            builder.AddReferenceOnlyAssemblyByType<object>(); // adds the corlib
            metaData = builder.Build();
        }

        public List<Type> GetExports<T>() where T : class
        {
            IEnumerable<TypeDefinition> foundTypes;

            if (typeof(T).IsInterface)
            {
                foundTypes = metaData.TypeDefinitions.Where(t =>
                    {
                        return t.AllInterfaces.Any(i =>
                        {
                            return i.FullName == typeof(T).FullName;
                        });
                    });
            }
            else
                foundTypes = metaData.TypeDefinitions.Where(t => t.FullName == typeof(T).FullName || t.HasBaseType<T>());
            var ret = new List<Type>();

            foreach (var foundType in foundTypes)
            {
                IEnumerable<AssemblyDefinition> assembly = metaData.AssemblyDefinitions.Where(x => x.Name == foundType.AssemblyName);
                string location = assembly.First().Location;
                ret.Add(Assembly.LoadFrom(location).GetType(foundType.FullName));
            }
            return ret;
        }
    }
}
