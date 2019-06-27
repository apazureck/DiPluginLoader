using TestInterfaces;

namespace TestAssembly
{
    public class AnotherTestPlugin : AbstractTestPlugin
    {
        public string GetName()
        {
            return nameof(AnotherTestPlugin);
        }
    }
}
