/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;


/// <summary>
/// Represents this library.
/// </summary>
static public partial class DataLib
{
    static DbLogListener_tERP LogListener;
    
    // ● public
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
        LogListener = new();
    }
    
    // ● properties
#if DEBUG
    static public string DebugUserName => "teo";
#else
    static public string DebugUserName => string.Empty;
#endif
    static public string[] SupportedCultures { get; } =  ["en-US", "el-GR"];
}