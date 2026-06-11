/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Provides helper methods for locating application assemblies and types.
///
/// Application assemblies are all currently loaded assemblies excluding
/// system, framework and third-party assemblies based on configurable
/// exclusion rules.
/// </summary>
static public class AppAssemblies
{
    static List<string> StartList = [];
    static List<string> ContainingList = [];
 
    // ● construction
    /// <summary>
    /// Initializes the exclusion lists used to identify
    /// application assemblies.
    /// </summary>
    static AppAssemblies()
    {
        StartList.AddRange(["System", "Microsoft", "Avalonia", "mscorlib", "mscorlib", "netstandard"]);
        
        ContainingList.AddRange(["FirebirdSql", "Npgsql", "MySql", "Oracle"]);               // Data
        ContainingList.AddRange(["SkiaSharp", "Tmds.DBus.Protocol", "HarfBuzzSharp"]);       // Avalonia
    }
    
    // ● public
    /// <summary>
    /// Adds an assembly name prefix to the exclusion list.
    /// Any assembly whose name starts with the specified value
    /// is ignored when locating application assemblies.
    /// </summary>
    static public void AddExcludeStart(string Name)
    {
        if (!StartList.ContainsText(Name))
            StartList.Add(Name);
    }
    /// <summary>
    /// Adds multiple assembly name prefixes to the exclusion list.
    /// </summary>
    static public void AddExcludeStart(IEnumerable<string> Names)
    {
        foreach (string Name in Names)
            AddExcludeStart(Name);
    }
    /// <summary>
    /// Adds a text fragment to the exclusion list.
    /// Any assembly whose name contains the specified value
    /// is ignored when locating application assemblies.
    /// </summary>
    static public void AddExcludeContaining(string Name)
    {
        if (!ContainingList.ContainsText(Name))
            ContainingList.Add(Name);
    }
    /// <summary>
    /// Adds multiple text fragments to the exclusion list.
    /// </summary>
    static public void AddExcludeContaining(IEnumerable<string> Names)
    {
        foreach (string Name in Names)
            AddExcludeContaining(Name);
    }
    
    /// <summary>
    /// Returns true when an assembly name is considered
    /// an application assembly name.
    /// </summary>
    static bool IsApplicationAssembly(string Name)
    {
        foreach (string sName in StartList)
            if (Name.StartsWithText(sName))
                return false;
        
        foreach (string sName in ContainingList)
            if (Name.ContainsText(sName))
                return false;

        return true;
    }
    
    /// <summary>
    /// Returns all currently loaded application assemblies.
    /// Optionally extends the exclusion list with additional
    /// assembly name fragments.
    /// </summary>
    static public List<Assembly> GetApplicationAssemblies(string[] ExcludeAssembliesContaining = null)
    {
        if (ExcludeAssembliesContaining != null)
            AddExcludeContaining(ExcludeAssembliesContaining);
 
        List<Assembly> Result = new List<Assembly>();
        Assembly[] LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly Item in LoadedAssemblies)
        {
            string Name = Item.GetName().Name;
            bool Flag = IsApplicationAssembly(Name);
            if (Flag)
                Result.Add(Item);
        }

        return Result;
    }
    /// <summary>
    /// Searches all application assemblies for a class type.
    ///
    /// The search is performed first using the fully qualified type name.
    /// If not found, a second search is performed using only the class name.
    ///
    /// When <paramref name="BaseType"/> is specified, only types assignable
    /// to that base type are considered.
    /// </summary>
    static public Type FindApplicationClassType(string ClassName, Type BaseType = null)
    {
        // ------------------------------------------------
        bool IsOk(Type T) => BaseType == null ? true : BaseType.IsAssignableFrom(T);
        // ------------------------------------------------
        
        Type Result = null;
        List<Assembly> Assemblies = GetApplicationAssemblies();
        
        Type[] Types;
        foreach (Assembly A in Assemblies)
        {
            Types = A.GetTypesSafe();
            foreach (Type T in Types)
            {
                if (T.IsClass && Equals(T.FullName, ClassName) && IsOk(T))
                {
                    Result = T;
                    break;
                }
            }
        }

        if (Result == null)
        {
            foreach (Assembly A in Assemblies)
            {
                Types = A.GetTypesSafe();
                foreach (Type T in Types)
                {
                    if (T.IsClass && T.FullName.EndsWith("." + ClassName) && IsOk(T))
                    {
                        Result = T;
                        break;
                    }
                }
            }
        }

        return Result;
    }
}