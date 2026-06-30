/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Web;

/// <summary>
/// Represents this library.
/// </summary>
static public class WebLib
{
    // ● private
    static List<RegistryVersion> GetRegistryVersions()
    {
        Type BaseType = typeof(RegistryVersion);
        return typeof(WebLib).Assembly.GetTypesSafe()
            .Where(Type => Type.IsClass && !Type.IsAbstract && BaseType.IsAssignableFrom(Type))
            .Where(Type => Type.Name.StartsWith("WebRegistryVersion", StringComparison.Ordinal))
            .Select(Type => Activator.CreateInstance(Type) as RegistryVersion)
            .Where(Version => Version != null)
            .OrderBy(Version => Version.VersionNumber)
            .ToList();
    }

    // ● static public
    /// <summary>
    /// We need to call this first of all in order for .Net to load the assembly.
    /// <para>Otherwise is not "visible" to <see cref="TypeStore.RegisterLoadedAssemblies()"/> which registers types marked with the <see cref="TypeStoreAttribute"/>.</para>
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
    /// <summary>
    /// Registers web descriptors.
    /// </summary>
    static public void RegisterDescriptors()
    {
        foreach (RegistryVersion Version in GetRegistryVersions())
            Version.RegisterForms();

        UpdateForms();
    }
    /// <summary>
    /// Definitions added by the registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateForms()
    {
        WebFormDef AppUserForm = WebDeskRegistry.Forms.Find("AppUser");
        if (AppUserForm != null)
            AppUserForm.SecurityLevel = UserLevel.Admin;
    }
}
