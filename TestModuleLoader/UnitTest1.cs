using Microsoft.Extensions.DependencyInjection;
using System.Linq;
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

        [Fact]
        public void TestLoadingMultiple()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddTransientPlugins<TestInterfaces.TestInterface>("TestAssemblies/**/*.dll");
            System.Collections.Generic.IEnumerable<TestInterfaces.TestInterface> services = serviceCollection.BuildServiceProvider().GetServices<TestInterfaces.TestInterface>();
            Assert.Equal(2, services.Count());
        }
    }
}
