using FasterReflection;
using GlobExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mef2ServiceLoader
{
    public class PluginLoader
    {
        List<CompositionHost> containers = new List<CompositionHost>();
        private ReflectionMetadataResult metaData;

        public PluginLoader(string pattern, ILogger logger = null, string rootDir = ".") : this(logger ?? NullLogger.Instance, rootDir, new string[] { pattern })
        {

        }

        /// <summary>
        /// Creates a new plugin loader
        /// </summary>
        /// <param name="root">root folder, default is relative to executing assemlby</param>
        /// <param name="patterns"></param>
        public PluginLoader(ILogger logger = null, string rootDir = ".", IEnumerable<string> patterns = null)
        {
            logger.LogDebug("Getting files from {relpath}", rootDir);
            var files = new List<string>();
            if (patterns == null)
                files.AddRange(Glob.Files(rootDir, "**/*.dll"));
            else
                foreach (var pattern in patterns)
                    files.AddRange(Glob.Files(rootDir, pattern.TrimStart("./".ToCharArray())));

            logger.LogDebug("Found {fct} files:\n {@files}", files.Count, files);

            var builder = new ReflectionMetadataBuilder();

            foreach (var file in files)
            {
                try
                {
                    builder.AddAssembly(Path.Combine(rootDir, file));
                }
                catch (Exception)
                {

                }
            }

            builder.AddReferenceOnlyAssemblyByType<object>(); // adds the corlib
            try
            {
                metaData = builder.Build(true);
            }
            catch (AggregateException aex)
            {
                throw;
            }
            catch(Exception ex)
            {
                
            }
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
