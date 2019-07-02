using Microsoft.Extensions.DependencyInjection;
using TestAssembly;
using TestAssembly2;
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
            serviceCollection.AddTransientPlugins<TestInterfaces.ITestInterface>("TestAssemblies/**/*.dll");
            System.Collections.Generic.IEnumerable<TestInterfaces.ITestInterface> services = serviceCollection.BuildServiceProvider().GetServices<TestInterfaces.ITestInterface>();

            AssertCollection.CollectionHasAny(services, output,
                t => Assert.Equal(typeof(MultiAssemblyPlugin), t.GetType()),
                t => Assert.Equal(typeof(TestPlugin), t.GetType()),
                t => Assert.Equal(typeof(AnotherTestPlugin), t.GetType()),
                t => Assert.Equal(typeof(TestPlugin2), t.GetType()));
        }

        [Fact]
        public void TestLoadingFromSeperateFolders()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddTransientPlugins<TestInterfaces.ITestInterface>("TestAssemblies/TestAssembly1/**/*.dll", );
            System.Collections.Generic.IEnumerable<TestInterfaces.ITestInterface> services = serviceCollection.BuildServiceProvider().GetServices<TestInterfaces.ITestInterface>();

            AssertCollection.CollectionHasAny(services, output,
                t => Assert.Equal(typeof(MultiAssemblyPlugin), t.GetType()),
                t => Assert.Equal(typeof(TestPlugin), t.GetType()),
                t => Assert.Equal(typeof(AnotherTestPlugin), t.GetType()),
                t => Assert.Equal(typeof(TestPlugin2), t.GetType()));
        }
    }
}
