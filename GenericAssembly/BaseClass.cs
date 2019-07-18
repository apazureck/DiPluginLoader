using TestInterfaces;

namespace GenericAssembly
{
    public abstract class BaseClass : ITestInterface
    {
        public string GetName() => GetType().Name;

    }
}
