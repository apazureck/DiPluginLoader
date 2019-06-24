using TestInterfaces;

namespace TestAssembly
{
    public class TestPlugin : TestInterface
    {
        public string GetName()
        {
            return "TestPlugin";
        }
    }
}
