using Microsoft.Extensions.DependencyInjection;
using TestAssembly;
using Xunit;
using Xunit.Abstractions;

namespace TestModuleLoader
{
    public class UnitTest1
    {
        private readonly IServiceCollection serviceCollection;
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestLoading()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var loadPlugins = new Mef2ServiceLoader.PluginLoader("TestAssemblies/**/*.dll");
            var types = loadPlugins.GetExports<TestInterfaces.ITestInterface>();
            serviceCollection.AddTransient(types[0]);
            serviceCollection.AddTransient(p => p.GetService(types[0]) as TestInterfaces.ITestInterface);
            var prov = serviceCollection.BuildServiceProvider();
            var instances = prov.GetService<TestInterfaces.ITestInterface>();
        }

        [Fact]
        public void TestLoadingMultiple()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddTransientPlugins<TestInterfaces.ITestInterface>("TestAssemblies/**/*.dll");
            System.Collections.Generic.IEnumerable<TestInterfaces.ITestInterface> services = serviceCollection.BuildServiceProvider().GetServices<TestInterfaces.ITestInterface>();

            AssertCollection.CollectionHasAny(services, output,
                t => Assert.Equal(typeof(MultiAssemblyPlugin), t.GetType()),
                t => Assert.Equal(typeof(TestPlugin), t.GetType()),
                t => Assert.Equal(typeof(AnotherTestPlugin), t.GetType()));
        }
    }
}
