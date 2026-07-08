namespace MiniCrm.Data;

/// <summary>
/// Registers schemas and descriptors for this sample.
/// </summary>
static public partial class Registry
{
    // ● private fields
    static readonly List<SchemaVersionDef> fSchemaVersionList = [];
    static readonly List<RegistryVersion> fRegistryVersionList = [];

    // ● constructor
    /// <summary>
    /// Initializes registry versions.
    /// </summary>
    static Registry()
    {
        fSchemaVersionList.AddRange([
            new SchemaVersion1()
        ]);
        fRegistryVersionList.AddRange([
            new RegistryVersion1()
        ]);
    }

    // ● static public
    /// <summary>
    /// Registers database schema versions.
    /// </summary>
    static public void RegisterSchemas()
    {
        foreach (SchemaVersionDef Version in fSchemaVersionList)
            Version.Register();
    }
    /// <summary>
    /// Registers descriptors, such as modules and forms.
    /// </summary>
    static public void RegisterDescriptors()
    {
        foreach (RegistryVersion Version in fRegistryVersionList)
        {
            Version.RegisterLookupSources();
            Version.RegisterLocators();
            Version.RegisterModules();
            Version.RegisterForms();
            Version.RegisterConfigProperties();
        }

        DataRegistry.UpdateLocator2References();
        DataRegistry.Modules.UpdateReferences();
        DesktopRegistry.Forms.UpdateReferences();
    }
}
