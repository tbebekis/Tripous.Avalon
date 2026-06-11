/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Global store of discoverable application types.
/// </summary>
static public class TypeStore
{
    // ● private fields
    static readonly Dictionary<string, Type> fItems = new(StringComparer.OrdinalIgnoreCase);
    static readonly object fLock = new();

    // ● private methods
    /// <summary>
    /// Returns true if a type is marked with the <see cref="TypeStoreAttribute"/>.
    /// </summary>
    static bool IsStoredType(Type Type)
    {
        return Type.GetCustomAttributes(typeof(TypeStoreAttribute), true).Length > 0;
    }
    /// <summary>
    /// Resolves a type using all available mechanisms.
    /// </summary>
    static Type ResolveCore(string TypeName, Type BaseType = null)
    {
        // Type.GetType()
        Type Result = Type.GetType(TypeName, false, true);
        if (Result != null)
        {
            Register(Result);
            return Result;
        }

        // FullName lookup
        lock (fLock)
        {
            if (fItems.TryGetValue(TypeName, out Result))
                return Result;
        }

        // Simple class name lookup
        string Suffix = "." + TypeName;

        lock (fLock)
        {
            List<Type> MatchingTypes = fItems.Values
                .Where(T => !string.IsNullOrWhiteSpace(T.FullName) &&
                            T.FullName.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (MatchingTypes.Count > 1)
            {
                StringBuilder SB = new();

                SB.AppendLine($"There are more than one types with this name: {TypeName}");

                foreach (Type T in MatchingTypes)
                    SB.AppendLine(T.FullName);

                throw new TripousException(SB.ToString());
            }

            if (MatchingTypes.Count == 1)
                Result = MatchingTypes[0];
        }

        // Application assemblies fallback
        if (Result == null)
            Result = AppAssemblies.FindApplicationClassType(TypeName, BaseType);

        return Result;
    }

    // ● static public
    /// <summary>
    /// Registers a single type.
    /// </summary>
    static public void Register(Type Type)
    {
        if (Type == null)
            throw new ArgumentNullException(nameof(Type));

        if (string.IsNullOrWhiteSpace(Type.FullName))
            throw new TripousException("Type.FullName is null or empty.");

        lock (fLock)
        {
            if (!fItems.ContainsKey(Type.FullName))
                fItems[Type.FullName] = Type;
        }
    }
    /// <summary>
    /// Registers all discoverable types of an assembly.
    /// </summary>
    static public void Register(Assembly Assembly)
    {
        if (Assembly == null)
            throw new ArgumentNullException(nameof(Assembly));

        Type[] Types = Assembly.GetTypesSafe();

        foreach (Type Type in Types)
        {
            if (!IsStoredType(Type))
                continue;

            Register(Type);
        }
    }
    /// <summary>
    /// Loads and registers all discoverable types of all assemblies in the specified folders.
    /// </summary>
    static public void LoadAndRegisterAssemblies(params string[] FolderPaths)
    {
        foreach (string FolderPath in FolderPaths)
        {
            if (string.IsNullOrWhiteSpace(FolderPath) || !Directory.Exists(FolderPath))
                continue;

            string[] FilePaths = Directory.GetFiles(FolderPath, "*.dll", SearchOption.AllDirectories);

            foreach (string FilePath in FilePaths)
            {
                try
                {
                    Assembly Assembly = Assembly.LoadFrom(FilePath);
                    TypeStore.Register(Assembly);
                }
                catch
                {
                    // Ignore files that are not valid .NET assemblies or cannot be loaded.
                }
            }
        }
    }
    /// <summary>
    /// Registers all discoverable types of all loaded assemblies.
    /// </summary>
    static public void RegisterLoadedAssemblies()
    {
        Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly Assembly in Assemblies)
            Register(Assembly);
    }
    /// <summary>
    /// Registers all discoverable types of application assemblies.
    /// </summary>
    static public void RegisterApplicationAssemblies()
    {
        List<Assembly> Assemblies = AppAssemblies.GetApplicationAssemblies();

        foreach (Assembly Assembly in Assemblies)
            Register(Assembly);
    }
    
    /// <summary>
    /// Finds and returns a type, if any, else null.
    /// </summary>
    static public Type Find(string TypeName)
    {
        return Resolve(TypeName, null, false);
    }
    /// <summary>
    /// Finds and returns a type, if any, else exception.
    /// </summary>
    static public Type Get(string TypeName)
    {
        Type Result = Resolve(TypeName);

        if (Result == null)
            throw new TripousException($"Type '{TypeName}' not found.");

        return Result;
    }
    /// <summary>
    /// Returns true if a type exists in the store.
    /// </summary>
    static public bool Contains(string TypeName)
    {
        return Find(TypeName) != null;
    }
    
    /// <summary>
    /// Resolves and returns a type.
    /// </summary>
    static public Type Resolve(string TypeName, Type BaseType = null, bool ThrowIfNotFound = true)
    {
        if (string.IsNullOrWhiteSpace(TypeName))
        {
            if (ThrowIfNotFound)
                throw new TripousArgumentNullException(nameof(TypeName));

            return null;
        }

        Type Result = ResolveCore(TypeName, BaseType);

        if (Result == null)
        {
            if (ThrowIfNotFound)
                throw new TripousException($"Type '{TypeName}' not found.");

            return null;
        }

        if (BaseType != null && !BaseType.IsAssignableFrom(Result))
            throw new TripousException($"Type '{TypeName}' is not assignable to '{BaseType.FullName}'.");

        Register(Result);

        return Result;
    }
    /// <summary>
    /// Creates and returns an instance of a type.
    /// </summary>
    static public object CreateInstance(string TypeName, Type BaseType = null)
    {
        Type ClassType = Resolve(TypeName, BaseType);
        return Activator.CreateInstance(ClassType);
    }
    /// <summary>
    /// Creates and returns an instance of a type.
    /// </summary>
    static public T CreateInstance<T>(string TypeName) where T : class
    {
        Type ClassType = Resolve(TypeName, typeof(T));
        return Activator.CreateInstance(ClassType) as T;
    }
    
    /// <summary>
    /// Clears the internal store.
    /// </summary>
    static public void Clear()
    {
        lock (fLock)
            fItems.Clear();
    }
}