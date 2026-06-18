namespace Notes.Data;

/// <summary>
/// Data module for Note records.
/// </summary>
public class NotesDataModule : DataModule
{
    // ● protected
    /// <summary>
    /// Sets default values for a Note row.
    /// </summary>
    /// <param name="Table">The table.</param>
    /// <param name="Row">The data row.</param>
    /// <param name="TableDef">The table definition.</param>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        // ● Keep data defaults inside the DataModule, not inside the UI.
        if (TableDef.Name == "Note")
        {
            if (Sys.IsNull(Row["CreatedAt"]))
                Row["CreatedAt"] = DateTime.Now;
            Row["UpdatedAt"] = DateTime.Now;
            // ● Tripous database booleans are integer-backed 0/1 values.
            if (Sys.IsNull(Row["IsPinned"]))
                Row["IsPinned"] = 0;
        }
    }
}
