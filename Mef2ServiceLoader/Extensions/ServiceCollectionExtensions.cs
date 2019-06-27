using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientPlugins<T>(this IServiceCollection serviceCollection, string pluginsFolder) where T : class => AddTransientPlugins<T>(serviceCollection, new string[] { pluginsFolder });

        public static IServiceCollection AddTransientPlugins<T>(this IServiceCollection serviceCollection, IEnumerable<string> plugins) where T : class
        {
            var loadPlugins = new Mef2ServiceLoader.PluginLoader(plugins);
            var types = loadPlugins.GetExports<T>();
            foreach(var type in types)
            {
                serviceCollection.AddTransient(type);
                serviceCollection.AddTransient(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }

        public static IServiceCollection AddSingletonPlugins<T>(this IServiceCollection serviceCollection, string pluginsFolder) where T : class => AddSingletonPlugins<T>(serviceCollection, new string[] { pluginsFolder });

        public static IServiceCollection AddSingletonPlugins<T>(this IServiceCollection serviceCollection, IEnumerable<string> plugins) where T : class
        {
            var loadPlugins = new Mef2ServiceLoader.PluginLoader(plugins);
            var types = loadPlugins.GetExports<T>();
            foreach (var type in types)
            {
                serviceCollection.AddSingleton(type);
                serviceCollection.AddSingleton(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }

        public static IServiceCollection AddScopedPlugins<T>(this IServiceCollection serviceCollection, string pluginsFolder) where T : class => AddScopedPlugins<T>(serviceCollection, new string[] { pluginsFolder });

        public static IServiceCollection AddScopedPlugins<T>(this IServiceCollection serviceCollection, IEnumerable<string> plugins) where T : class
        {
            var loadPlugins = new Mef2ServiceLoader.PluginLoader(plugins);
            var types = loadPlugins.GetExports<T>();
            foreach (var type in types)
            {
                serviceCollection.AddScoped(type);
                serviceCollection.AddScoped(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }
    }
}
