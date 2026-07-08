namespace Tripous.Data;

/// <summary>
/// Creates locator mapping plans.
/// </summary>
public class LocatorMapper
{
    // ● protected methods
    /// <summary>
    /// Sets a target row value.
    /// </summary>
    protected virtual void SetTargetRowValue(DataRow TargetRow, string TargetField, object Value)
    {
        if (TargetRow == null || string.IsNullOrWhiteSpace(TargetField))
            return;

        DataColumn Column = TargetRow.Table.FindColumn(TargetField);
        if (Column == null || Column.ReadOnly)
            return;

        object NewValue = Sys.IsNull(Value) ? DBNull.Value : Value;
        if (Sys.IsNull(NewValue) && !Column.AllowDBNull)
            return;

        TargetRow[Column] = NewValue;
    }
    /// <summary>
    /// Returns a source row value.
    /// </summary>
    protected virtual object GetSourceRowValue(DataRow SourceRow, string SourceField)
    {
        if (SourceRow == null || string.IsNullOrWhiteSpace(SourceField))
            return DBNull.Value;

        DataColumn Column = SourceRow.Table.FindColumn(SourceField);
        return Column != null ? SourceRow[Column] : DBNull.Value;
    }
    /// <summary>
    /// Finds a snapshot field for a join field.
    /// </summary>
    protected virtual FieldDef FindSnapshotField(TableDef TargetTable, TableDef JoinTable, FieldDef JoinField)
    {
        if (TargetTable == null || JoinTable == null || JoinField == null)
            return null;

        foreach (FieldDef Field in TargetTable.Fields)
        {
            if (string.IsNullOrWhiteSpace(Field.SnapshotOf))
                continue;

            string[] Parts = Field.SnapshotOf.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (Parts.Length == 2 && Parts[0].IsSameText(JoinTable.Alias) && Parts[1].IsSameText(JoinField.Name))
                return Field;
        }

        return null;
    }
    /// <summary>
    /// Finds a target field for a locator result field.
    /// </summary>
    protected virtual FieldDef FindTargetField(TableDef TargetTable, FieldDef ReferenceField, string SourceField)
    {
        if (TargetTable == null || ReferenceField == null || string.IsNullOrWhiteSpace(SourceField))
            return null;

        TableDef JoinTable = TargetTable.FindJoinTableByMasterKeyField(ReferenceField.Name);
        FieldDef JoinField = JoinTable?.Fields.FirstOrDefault(item => item.Name.IsSameText(SourceField) || item.Alias.IsSameText(SourceField));
        if (JoinField != null)
            return FindSnapshotField(TargetTable, JoinTable, JoinField) ?? JoinField;

        return TargetTable.Fields.FirstOrDefault(item => item.Name.IsSameText(SourceField) || item.Alias.IsSameText(SourceField));
    }

    // ● public
    /// <summary>
    /// Creates a locator mapping plan.
    /// </summary>
    public virtual LocatorMapPlan CreatePlan(LocatorDef LocatorDef, TableDef TargetTable, FieldDef ReferenceField)
    {
        LocatorMapPlan Result = new()
        {
            LocatorName = LocatorDef?.Name,
            ReferenceField = ReferenceField?.Name,
        };

        if (LocatorDef == null || TargetTable == null || ReferenceField == null)
            return Result;

        Result.Add(LocatorDef.KeyField, ReferenceField.Name);

        foreach (string SourceField in LocatorDef.GetResultFields())
        {
            if (SourceField.IsSameText(LocatorDef.KeyField))
                continue;

            FieldDef TargetField = FindTargetField(TargetTable, ReferenceField, SourceField);
            if (TargetField != null)
                Result.Add(SourceField, TargetField.Alias);
        }

        return Result;
    }
    /// <summary>
    /// Applies a locator mapping plan to a target row.
    /// </summary>
    public virtual void Apply(LocatorMapPlan Plan, DataRow SourceRow, DataRow TargetRow)
    {
        if (Plan == null || TargetRow == null)
            return;

        foreach (LocatorMapItem Item in Plan.Items)
        {
            object Value = GetSourceRowValue(SourceRow, Item.SourceField);
            SetTargetRowValue(TargetRow, Item.TargetField, Value);
        }
    }
}
