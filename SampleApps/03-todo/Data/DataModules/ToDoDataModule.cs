using System.Globalization;

namespace ToDo.Data;

/// <summary>
/// Data module for TodoTask records.
/// </summary>
public class ToDoDataModule : DataModule
{
    // ● protected
    /// <summary>
    /// Sets default values for a TodoTask row.
    /// </summary>
    /// <param name="Table">The table.</param>
    /// <param name="Row">The data row.</param>
    /// <param name="TableDef">The table definition.</param>
    protected override void SetDefaultValues(MemTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        // ● DataModule defaults run for both insert and commit paths.
        // ● This keeps application data rules outside the UI layer.
        if (TableDef.Name == "TodoTask")
        {
            if (Sys.IsNull(Row["TodoStatusId"]))
                Row["TodoStatusId"] = 1;
            if (Sys.IsNull(Row["Priority"]))
                Row["Priority"] = Convert.ToInt32(Config.GetValue("ToDo.DefaultPriority"), CultureInfo.InvariantCulture);
            // ● Tripous database booleans are integer-backed 0/1 values.
            if (Sys.IsNull(Row["IsDone"]))
                Row["IsDone"] = 0;
            if (Sys.IsNull(Row["CreatedAt"]))
                Row["CreatedAt"] = DateTime.Now;
            Row["UpdatedAt"] = DateTime.Now;

            bool IsDone = Convert.ToInt32(Row["IsDone"]) != 0;
            if (IsDone && Sys.IsNull(Row["CompletedAt"]))
                Row["CompletedAt"] = DateTime.Now;
            else if (!IsDone)
                Row["CompletedAt"] = DBNull.Value;
        }
    }
}
