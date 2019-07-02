using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddTransientPlugins<T>(this IServiceCollection serviceCollection, params string[] plugins) where T : class
        {
            IEnumerable<System.Type> types = LoadTypes<T>(plugins);
            foreach (var type in types)
            {
                serviceCollection.AddTransient(type);
                serviceCollection.AddTransient(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }

        private static IEnumerable<System.Type> LoadTypes<T>(IEnumerable<string> plugins) where T : class
        {
            var loadPlugins = new Mef2ServiceLoader.PluginLoader(plugins);
            var types = loadPlugins.GetExports<T>().Where(t => !(t.IsAbstract || t.IsInterface));
            return types;
        }

        public static IServiceCollection AddSingletonPlugins<T>(this IServiceCollection serviceCollection, params string[] plugins) where T : class
        {
            IEnumerable<System.Type> types = LoadTypes<T>(plugins);
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
            IEnumerable<System.Type> types = LoadTypes<T>(plugins);
            foreach (var type in types)
            {
                serviceCollection.AddScoped(type);
                serviceCollection.AddScoped(p => (T)p.GetService(type));
            }
            return serviceCollection;
        }
    }
}
