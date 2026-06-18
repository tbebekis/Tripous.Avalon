namespace Notes.Data;

/// <summary>
/// Registers version 1 descriptors.
/// </summary>
public partial class RegistryVersion1 : RegistryVersion
{
    // ● public
    /// <summary>
    /// Registers the Note module definition.
    /// </summary>
    public override void RegisterModules()
    {
        // ● A ModuleDef describes a data module. It is the bridge between the database table,
        // ● the DataModule class and the list SELECTs used by DataForm.
        // ● This sample uses a custom NotesDataModule class, but it still uses the default Tripous.Desktop DataForm and ItemPage classes.
        // ● Larger applications may register custom DataModule, DataForm and ItemPage classes here.
        string SqlText = """
                         select
                             Id
                            ,Title
                            ,CreatedAt
                            ,UpdatedAt
                            ,IsPinned
                         from
                             Note
                         order by
                             IsPinned desc,
                             UpdatedAt desc,
                             Title
                         """;
        ModuleDef Module = DataRegistry.AddModule("Note", TitleKey: "Notes", ClassName: typeof(NotesDataModule).FullName, ListSelectSql: SqlText, IsSingleSelect: true);
        SelectDef SelectDef = Module.SelectList[0];
        // ● A ModuleDef may have multiple list SELECT definitions. Each SelectDef may have its own SQL text,
        // ● display labels and filter definitions. The DataForm list toolbar lets the user run the selected list.
        SelectDef.AddFilter("Title", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
        SelectDef.AddFilter("CreatedAt", FilterDataType: DataFieldType.DateTime, ConditionOp: ConditionOp.Between);

        // ● TableDef describes the editable item table. Tripous uses it to create the in-memory MemTable,
        // ● create UI editors, build INSERT/UPDATE/DELETE SQL and bind controls to fields.
        TableDef Table = Module.Table;
        Table.Name = "Note";
        Table.AddId();
        // ● AddString(), AddDateTime(), AddBoolean() and the other AddXXXX helpers create FieldDef objects.
        // ● FieldDef flags affect both data behavior and UI behavior. For example Required affects validation,
        // ● Searchable makes a field a good filter candidate, Memo creates a multiline editor and Boolean creates a checkbox.
        // ● Tripous boolean FieldDef fields are integer-backed 0/1 fields.
        Table.AddString("Title", 128, Flags: FieldFlags.Required | FieldFlags.Searchable);
        Table.AddString("Body", 4000, Flags: FieldFlags.Memo);
        Table.AddDateTime("CreatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
        Table.AddDateTime("UpdatedAt", Flags: FieldFlags.Required | FieldFlags.ReadOnlyUI | FieldFlags.ReadOnlyEdit);
        Table.AddBoolean("IsPinned");
    }
    /// <summary>
    /// Registers the Note form definition.
    /// </summary>
    public override void RegisterForms()
    {
        // ● FormDef connects a UI form registration name with a ModuleDef.
        // ● No custom form class is supplied here, so Tripous.Desktop.DataForm is used.
        // ● No custom item page class is supplied either, so Tripous.Desktop.ItemPage is used.
        DesktopRegistry.AddForm("Note", TitleKey: "Notes", Module: "Note", Group: "Samples");
    }

    // ● properties
    /// <summary>
    /// Gets the registry version number.
    /// </summary>
    public override int VersionNumber => 1;
}
