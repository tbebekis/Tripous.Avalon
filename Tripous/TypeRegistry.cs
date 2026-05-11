namespace Tripous;

/// <summary>
/// Global registry of discoverable types.
/// </summary>
static public class TypeRegistry
{
    // ● private fields
    static readonly Dictionary<string, Type> fItems = new(StringComparer.OrdinalIgnoreCase);

    // ● private methods
    /// <summary>
    /// Returns true if a type is marked with the <see cref="RegistryTypeAttribute"/>.
    /// </summary>
    static bool IsRegisteredType(Type Type)
    {
        return Type.GetCustomAttributes(typeof(RegistryTypeAttribute), false).Length > 0;
    }

    // ● static public
    /// <summary>
    /// Registers all discoverable types of an assembly.
    /// </summary>
    static public void Register(Assembly Assembly)
    {
        foreach (Type Type in Assembly.GetTypes())
        {
            if (!IsRegisteredType(Type))
                continue;

            if (!fItems.ContainsKey(Type.FullName))
                fItems[Type.FullName] = Type;
        }
    }
    /// <summary>
    /// Registers all discoverable types of all loaded assemblies.
    /// </summary>
    static public void RegisterLoadedAssemblies()
    {
        foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies())
            Register(Assembly);
    }
    /// <summary>
    /// Registers a single type.
    /// </summary>
    static public void Register(Type Type)
    {
        if (Type == null)
            throw new ArgumentNullException(nameof(Type));

        if (string.IsNullOrWhiteSpace(Type.FullName))
            throw new Exception("Type.FullName is null or empty.");

        if (!fItems.ContainsKey(Type.FullName))
            fItems[Type.FullName] = Type;
    }
    /// <summary>
    /// Finds and returns a registered type, if any, else null.
    /// </summary>
    static public Type Find(string TypeName)
    {
        if (string.IsNullOrWhiteSpace(TypeName))
           throw new TripousArgumentNullException($"{TypeName} is null or empty.");

        if (fItems.TryGetValue(TypeName, out Type Result))
            return Result;

        Result = Type.GetType(TypeName);

        if (Result != null)
            Register(Result);

        return Result;
    }
    /// <summary>
    /// Finds and returns a registered type, if any, else exception.
    /// </summary>
    static public Type Get(string TypeName)
    {
        Type Result = Find(TypeName);
        if (Result == null)
            throw new TripousException($"{TypeName} not found.");
        return Result;
    }
    /// <summary>
    /// Returns true if a type is registered.
    /// </summary>
    static public bool Contains(string TypeName)
    {
        return Find(TypeName) != null;
    }
}