namespace TemplateApp.Data;

/// <summary>
/// Registers schema versions and descriptors for TemplateApp.
/// </summary>
static public class Registry
{
    // ● private methods
    /// <summary>
    /// Registers lookup sources.
    /// </summary>
    static void RegisterLookupSources()
    {
    }
    /// <summary>
    /// Registers locators.
    /// </summary>
    static void RegisterLocators()
    {
    }
    /// <summary>
    /// Registers data modules, tables, fields, selects and related data descriptors.
    /// </summary>
    static void RegisterModules()
    {
    }
    /// <summary>
    /// Registers desktop forms.
    /// </summary>
    static void RegisterForms()
    {
    }
    /// <summary>
    /// Registers configuration properties.
    /// </summary>
    static void RegisterConfigProperties()
    {
    }

    // ● static public methods
    /// <summary>
    /// Registers database schema versions.
    /// </summary>
    static public void RegisterSchemas()
    {
    }
    /// <summary>
    /// Registers application descriptors.
    /// </summary>
    static public void RegisterDescriptors()
    {
        RegisterLookupSources();
        RegisterLocators();
        RegisterModules();
        RegisterForms();
        RegisterConfigProperties();

        DataRegistry.Modules.UpdateReferences();
        DesktopRegistry.Forms.UpdateReferences();
    }
}
