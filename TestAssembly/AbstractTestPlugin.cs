using TestInterfaces;

namespace TestAssembly
{
    public abstract class AbstractTestPlugin : TestInterface
    {
        public string GetName()
        {
            return nameof(AbstractTestPlugin);
        }
    }
}
