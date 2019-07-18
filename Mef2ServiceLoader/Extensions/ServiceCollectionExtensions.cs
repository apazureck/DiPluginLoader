using Mef2ServiceLoader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddTransientPlugins<T>(this IServiceCollection serviceCollection, params string[] plugins) where T : class
        {
            return AddTransientPlugins<T>(serviceCollection, NullLogger.Instance, plugins);
        }

        public static IServiceCollection AddTransientPlugins<T>(this IServiceCollection serviceCollection, ILogger logger, params string[] plugins) where T : class
        {
            IEnumerable<System.Type> types = LoadTypes<T>(plugins, logger);
            foreach (var type in types)
            {
                serviceCollection.AddTransient(type);
                serviceCollection.AddTransient(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }

        private static IEnumerable<System.Type> LoadTypes<T>(IEnumerable<string> plugins, ILogger logger) where T : class
        {
            IEnumerable<System.Type> types = new Mef2ServiceLoader.PluginLoader(logger, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), plugins).GetExports<T>().Where(t => !(t.IsAbstract || t.IsInterface));
            logger.LogDebug("Found {tct} implementations for {typename}: {@types}", types.Count(), typeof(T).Name, types);
            return types;
        }

        public static IServiceCollection AddSingletonPlugins<T>(this IServiceCollection serviceCollection, params string[] plugins) where T : class
        {
            return AddSingletonPlugins<T>(serviceCollection, NullLogger.Instance, plugins);
        }

        public static IServiceCollection AddSingletonPlugins<T>(this IServiceCollection serviceCollection, ILogger logger, params string[] plugins) where T : class
        {
            IEnumerable<System.Type> types = LoadTypes<T>(plugins, logger);
            foreach (var type in types)
            {
                serviceCollection.AddSingleton(type);
                serviceCollection.AddSingleton(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }

        public static IServiceCollection AddScopedPlugins<T>(this IServiceCollection serviceCollection, params string[] plugins) where T : class
        {
            return AddScopedPlugins<T>(serviceCollection, NullLogger.Instance, plugins);
        }

        public static IServiceCollection AddScopedPlugins<T>(this IServiceCollection serviceCollection, PluginLoader plugins) where T : class
        {
            foreach (System.Type type in plugins.GetExports<T>().Where(t => !(t.IsAbstract || t.IsInterface)))
            {
                serviceCollection.AddScoped(type);
                serviceCollection.AddScoped(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }

        public static IServiceCollection AddTransientPlugins<T>(this IServiceCollection serviceCollection, PluginLoader plugins) where T : class
        {
            foreach (System.Type type in plugins.GetExports<T>().Where(t => !(t.IsAbstract || t.IsInterface)))
            {
                serviceCollection.AddTransient(type);
                serviceCollection.AddTransient(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }

        public static IServiceCollection AddSingletonPlugins<T>(this IServiceCollection serviceCollection, PluginLoader plugins) where T : class
        {
            foreach (System.Type type in plugins.GetExports<T>().Where(t => !(t.IsAbstract || t.IsInterface)))
            {
                serviceCollection.AddSingleton(type);
                serviceCollection.AddSingleton(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }

        public static IServiceCollection AddScopedPlugins<T>(this IServiceCollection serviceCollection, ILogger logger, params string[] plugins) where T : class
        {
            IEnumerable<System.Type> types = LoadTypes<T>(plugins, logger);
            foreach (var type in types)
            {
                serviceCollection.AddScoped(type);
                serviceCollection.AddScoped(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }
    }
}
