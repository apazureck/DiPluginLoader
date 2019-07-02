using TestInterfaces;

namespace TestAssembly2
{
    public class TestPlugin2 : ITestInterface
    {
        public string GetName() => nameof(TestPlugin2);

    }
}
