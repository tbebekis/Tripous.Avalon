namespace Tripous;

/// <summary>
/// Helper for getting application assemblies, excluding system assemblies, or assemblies defined by the user.
/// </summary>
static public class AppAssemblies
{
    static List<string> StartList = [];
    static List<string> ContainingList = [];
 
    // ● construction
    static AppAssemblies()
    {
        StartList.AddRange(["System", "Microsoft", "Avalonia", "mscorlib", "mscorlib", "netstandard"]);
        
        ContainingList.AddRange(["FirebirdSql", "Npgsql", "MySql", "Oracle"]);               // Data
        ContainingList.AddRange(["SkiaSharp", "Tmds.DBus.Protocol", "HarfBuzzSharp"]);       // Avalonia
    }
    
    // ● public
    static public void AddExcludeStart(string Name)
    {
        if (!StartList.ContainsText(Name))
            StartList.Add(Name);
    }
    static public void AddExcludeStart(IEnumerable<string> Names)
    {
        foreach (string Name in Names)
            AddExcludeStart(Name);
    }
    static public void AddExcludeContaining(string Name)
    {
        if (!ContainingList.ContainsText(Name))
            ContainingList.Add(Name);
    }
    static public void AddExcludeContaining(IEnumerable<string> Names)
    {
        foreach (string Name in Names)
            AddExcludeContaining(Name);
    }
    
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
    /// Returns a list of non-system assemblies
    /// </summary>
    static public List<Assembly> GetApplicationAssemblies(string[] ExcludeAssempliesContaining = null)
    {
        if (ExcludeAssempliesContaining != null)
            AddExcludeContaining(ExcludeAssempliesContaining);
 
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
    /// Returns a class <see cref="Type"/> after searching all application assemblies for a type specified by name, if any, else null.
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