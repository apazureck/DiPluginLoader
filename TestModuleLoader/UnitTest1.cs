using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TestModuleLoader
{
    public class UnitTest1
    {
        private readonly IServiceCollection serviceCollection;

        public UnitTest1()
        {
            
        }
        [Fact]
        public void TestLoading()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var loadPlugins = new Mef2ServiceLoader.PluginLoader("TestAssemblies/**/*.dll");
            var types = loadPlugins.GetExports<TestInterfaces.TestInterface>();
            serviceCollection.AddTransient(types[0]);
            serviceCollection.AddTransient(p => p.GetService(types[0]) as TestInterfaces.TestInterface);
            var prov = serviceCollection.BuildServiceProvider();
            var instances = prov.GetService<TestInterfaces.TestInterface>();
        }
    }
}
