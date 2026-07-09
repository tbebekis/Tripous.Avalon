/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Provides item module structure information for a FactBox.
/// </summary>
public class ItemStructureFactBoxProvider: ItemFactBoxProvider
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemStructureFactBoxProvider()
    {
    }

    // ● private
    /// <summary>
    /// Creates field information.
    /// </summary>
    /// <param name="Field">The field definition.</param>
    /// <returns>The created field information.</returns>
    static ItemStructureFieldInfo CreateFieldInfo(FieldDef Field)
    {
        return new()
        {
            Name = Field.Name,
            Title = Field.Title,
            DataType = Field.DataType.ToString(),
            Group = Field.Group,
            MaxLength = Field.MaxLength,
            DisplayWidth = Field.DisplayWidth,
            Decimals = Field.Decimals,
            IsNullable = Field.IsNullable,
            IsVisible = Field.IsVisible,
            IsRequired = Field.IsRequired,
            IsReadOnly = Field.IsReadOnly || Field.IsReadOnlyUI || Field.IsReadOnlyEdit,
            LookupSource = Field.LookupSource,
            Locator = Field.Locator,
            DefaultValue = Field.DefaultValue,
            Expression = Field.Expression,
            CodeProvider = Field.CodeProvider,
            SnapshotOf = Field.SnapshotOf,
            Flags = Field.Flags.ToString()
        };
    }
    /// <summary>
    /// Creates table information.
    /// </summary>
    /// <param name="Table">The table definition.</param>
    /// <returns>The created table information.</returns>
    static ItemStructureTableInfo CreateTableInfo(TableDef Table)
    {
        ItemStructureTableInfo Result = new()
        {
            Name = Table.Name,
            Title = Table.Title,
            Alias = Table.Alias,
            KeyField = Table.KeyField,
            MasterField = Table.MasterField,
            DetailField = Table.DetailField,
            MasterName = Table.Master?.Name,
            DetailNames = Table.Details.Select(Detail => Detail.Name).ToList(),
            IsDetail = Table.IsDetail,
            IsUiVisible = Table.IsUiVisible,
            IsOneToOne = Table.IsOneToOne,
            FieldCount = Table.Fields.Count,
            VisibleFieldCount = Table.Fields.Count(Field => Field.IsVisible),
            JoinCount = Table.Joins.Count,
            StockCount = Table.Stocks.Count
        };

        foreach (FieldDef Field in Table.Fields)
            Result.Fields.Add(CreateFieldInfo(Field));
        foreach (TableDef Detail in Table.Details)
            Result.Details.Add(CreateTableInfo(Detail));

        return Result;
    }
    /// <summary>
    /// Counts tables recursively.
    /// </summary>
    /// <param name="Table">The table definition.</param>
    /// <param name="VisibleOnly">True to count only UI visible tables.</param>
    /// <returns>The table count.</returns>
    static int CountTables(TableDef Table, bool VisibleOnly)
    {
        if (Table == null)
            return 0;

        int Result = !VisibleOnly || Table.IsUiVisible ? 1 : 0;
        foreach (TableDef Detail in Table.Details)
            Result += CountTables(Detail, VisibleOnly);
        return Result;
    }

    // ● public
    /// <summary>
    /// Creates serializable data for a FactBox.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <returns>The serializable FactBox data.</returns>
    public override object GetData(ItemFactBoxContext Context)
    {
        ModuleDef Module = Context?.ModuleDef;
        return new ItemStructureFactBoxData
        {
            ModuleName = Module?.Name,
            ModuleTitle = Module?.Title,
            ModuleGroup = Module?.Group,
            ModuleClassName = Module?.ClassName,
            ModuleJsClassName = "tp.DataModule",
            FormClassName = Context?.FormClassName,
            FormJsClassName = Context?.FormJsClassName,
            ItemPageClassName = Context?.ItemPageClassName,
            ItemPageJsClassName = Context?.ItemPageJsClassName,
            TableCount = CountTables(Module?.Table, false),
            VisibleTableCount = CountTables(Module?.Table, true),
            Table = Module != null ? CreateTableInfo(Module.Table) : null
        };
    }
}
