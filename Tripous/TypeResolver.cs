namespace Tripous;

static public class TypeResolver
{
    // ● private
    static readonly Dictionary<string, Type> fCache = new(StringComparer.OrdinalIgnoreCase);
    static readonly object fLock = new();

    // ● private methods
    static Type ResolveCore(string ClassName)
    {
        Type Result = Type.GetType(ClassName, false, true);
        if (Result != null)
            return Result;

        foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                Result = Assembly.GetType(ClassName, false, true);
                if (Result != null)
                    return Result;
            }
            catch
            {
            }
        }

        return null;
    }

    // ● public methods
    static public Type Resolve(string ClassName, Type BaseType = null, bool ThrowIfNotFound = true)
    {
        if (string.IsNullOrWhiteSpace(ClassName))
        {
            if (ThrowIfNotFound)
                throw new ArgumentNullException(nameof(ClassName));

            return null;
        }

        Type Result;

        lock (fLock)
        {
            if (!fCache.TryGetValue(ClassName, out Result))
            {
                Result = ResolveCore(ClassName);
                fCache[ClassName] = Result;
            }
        }

        if (Result == null)
        {
            if (ThrowIfNotFound)
                throw new ApplicationException($"Type '{ClassName}' not found.");

            return null;
        }

        if (BaseType != null && !BaseType.IsAssignableFrom(Result))
            throw new ApplicationException($"Type '{ClassName}' is not assignable to '{BaseType.FullName}'.");

        return Result;
    }
    static public T CreateInstance<T>(string ClassName) where T : class
    {
        Type ClassType = Resolve(ClassName, typeof(T));
        return Activator.CreateInstance(ClassType) as T;
    }
    static public object CreateInstance(string ClassName, Type BaseType = null)
    {
        Type ClassType = Resolve(ClassName, BaseType);
        return Activator.CreateInstance(ClassType);
    }
    static public void ClearCache()
    {
        lock (fLock)
            fCache.Clear();
    }
}