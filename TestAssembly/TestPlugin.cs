using TestInterfaces;

namespace TestAssembly
{
    public class TestPlugin : ITestInterface
    {
        public string GetName()
        {
            return "TestPlugin";
        }
    }
}
