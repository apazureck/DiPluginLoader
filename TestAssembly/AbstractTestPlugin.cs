using TestInterfaces;

namespace TestAssembly
{
    public abstract class AbstractTestPlugin : ITestInterface
    {
        public string GetName()
        {
            return nameof(AbstractTestPlugin);
        }
    }
}
