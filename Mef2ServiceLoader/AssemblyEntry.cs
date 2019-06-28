using FasterReflection;
using System.Reflection;

namespace Mef2ServiceLoader
{
    internal class AssemblyEntry
    {
        private readonly ReflectionMetadataBuilder builder;

        public AssemblyEntry(ReflectionMetadataBuilder builder, string assemblyPath)
        {
            this.builder = builder;
            AssemblyPath = assemblyPath;
            ReflectionOnlyAssembly = builder.Build();
            foreach(var baseAssembly in ReflectionOnlyAssembly.AssemblyDefinitions)
                ;
        }

        public ReflectionMetadataResult ReflectionOnlyAssembly { get; private set; }
        public void AddAssemblyByType<T>()
        {
            try
            {
                builder.AddAssemblyByType<T>();
                ReflectionOnlyAssembly = builder.Build();
            }
            catch (System.Exception)
            {

            }
        }
        public string AssemblyName => ReflectionOnlyAssembly.AssemblyDefinitions[0].Name;
        public string AssemblyPath { get; }
        private Assembly assembly;
        public Assembly Assembly
        {
            get
            {
                if (assembly == null)
                {
                    assembly = Assembly.LoadFrom(AssemblyPath);
                }
                return assembly;
            }
        }
    }
}
