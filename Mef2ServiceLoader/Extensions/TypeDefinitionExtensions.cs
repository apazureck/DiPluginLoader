namespace FasterReflection
{
    public static class TypeDefinitionExtensions
    {
        /// <summary>
        /// Extension method which chechs the type definition, if it has any base type that is of the given type
        /// </summary>
        /// <typeparam name="Tbase">The base type to check for</typeparam>
        /// <param name="t">The <see cref="TypeDefinition"/> to be checked</param>
        /// <returns></returns>
        public static bool HasBaseType<Tbase>(this TypeDefinition t) where Tbase : class
        {
            if (t.BaseType != null)
            {
                if (t.BaseType.FullName == typeof(Tbase).FullName)
                    return true;
                else
                    return t.BaseType.HasBaseType<Tbase>();
            }
            return false;
        }
    }
}
