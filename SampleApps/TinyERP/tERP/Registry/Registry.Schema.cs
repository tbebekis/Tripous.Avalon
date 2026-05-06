namespace tERP;

static internal partial class Registry
{
    // ● schemas
    static internal void RegisterSchemas()
    {
        List<SchemaVersionDef> SchemaVersionList = [];
        SchemaVersionList.AddRange([new SchemaVersion1()]);
        
        foreach (SchemaVersionDef Version in SchemaVersionList)
            Version.Register();
    }
}