namespace tERP.Common;

/// <summary>
/// Represents this library.
/// </summary>
static public class CommonLib
{
    
    /// <summary>
    /// We need to call this first of all in order for .Net to load the assembly.
    /// <para>Otherwise is not "visible" to <see cref="TypeRegistry.RegisterLoadedAssemblies()"/> which registers types marked with the <see cref="TypeStoreAttribute"/>.</para>
    /// </summary>
    static public void Load()
    {
        // fake, must be called for the assembly to be loaded in the domain.
    }
    /// <summary>
    /// Initializes this library.
    /// </summary>
    static public void Initialize()
    {
        // nothing yet
    }
}