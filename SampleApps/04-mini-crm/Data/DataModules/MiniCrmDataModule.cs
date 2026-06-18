namespace MiniCrm.Data;

/// <summary>
/// Data module for Mini CRM records.
/// </summary>
public class MiniCrmDataModule : DataModule
{
    // ● protected
    /// <summary>
    /// Sets default values for CRM rows.
    /// </summary>
    /// <param name="Table">The table.</param>
    /// <param name="Row">The data row.</param>
    /// <param name="TableDef">The table definition.</param>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (TableDef.Name == "Customer")
        {
            if (Sys.IsNull(Row["IsActive"]))
                Row["IsActive"] = 1;
            if (Sys.IsNull(Row["CreatedAt"]))
                Row["CreatedAt"] = DateTime.Now;
            Row["UpdatedAt"] = DateTime.Now;
        }
        else if (TableDef.Name == "Contact")
        {
            if (Sys.IsNull(Row["IsPrimaryContact"]))
                Row["IsPrimaryContact"] = 0;
        }
        else if (TableDef.Name == "Activity")
        {
            if (Sys.IsNull(Row["ActivityDate"]))
                Row["ActivityDate"] = DateTime.Now;
            if (Sys.IsNull(Row["ActivityTypeId"]))
                Row["ActivityTypeId"] = Convert.ToInt32(Config.GetValue("MiniCrm.DefaultActivityTypeId"), CultureInfo.InvariantCulture);
            if (Sys.IsNull(Row["IsClosed"]))
                Row["IsClosed"] = 0;
        }
    }
}
