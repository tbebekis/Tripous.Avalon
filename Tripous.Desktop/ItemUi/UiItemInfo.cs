/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Creates and populates the metadata objects used while building an item page UI.
/// </summary>
static public class UiItemInfo
{
    /// <summary>
    /// Creates the root UI information object for the top table and its visible details.
    /// </summary>
    static public UiTableInfo CreateTopTableUiInfo(DataModule Module)
    {
        UiTableInfo Result = CreateUiTableInfo(Module.ModuleDef.Table, Module);
        AddDetailUiInfo(Result, Module.ModuleDef.Table, Module);
        return Result;
    }
    /// <summary>
    /// Creates UI information for a table.
    /// </summary>
    static public UiTableInfo CreateUiTableInfo(TableDef TableDef, DataModule Module)
    {
        UiTableInfo Result = new();
        Result.TableDef = TableDef;
        Result.Table = Module.GetTable(TableDef.Name);
        return Result;
    }
    /// <summary>
    /// Adds visible detail table information to the root UI information object.
    /// <para>One-to-one details and multi-row details are collected separately while the table tree is traversed recursively.</para>
    /// </summary>
    static public void AddDetailUiInfo(UiTableInfo RootUiInfo, TableDef ParentTableDef, DataModule Module)
    {
        foreach (TableDef Detail in ParentTableDef.Details)
        {
            if (!Detail.IsUiVisible)
                continue;
            if (Detail.IsOneToOne)
                RootUiInfo.OneToOneList.Add(CreateUiTableInfo(Detail, Module));
            else
                RootUiInfo.DetailList.Add(CreateDetailTableUiInfo(ParentTableDef, Detail, Module));
            AddDetailUiInfo(RootUiInfo, Detail, Module);
        }
    }
    /// <summary>
    /// Creates detail table UI information.
    /// </summary>
    static public UiDetailTableInfo CreateDetailTableUiInfo(TableDef ParentTableDef, TableDef TableDef, DataModule Module)
    {
        UiDetailTableInfo Result = new();
        Result.ParentTableDef = ParentTableDef;
        Result.TableDef = TableDef;
        Result.Table = Module.GetTable(TableDef.Name);
        return Result;
    }
    /// <summary>
    /// Adds the runtime association between a field and its generated control.
    /// </summary>
    static public void AddFieldUiInfo(UiTableInfo TableUiInfo, FieldDef Field, Control Control)
    {
        TableUiInfo.FieldList.Add(new UiFieldInfo
        {
            TableDef = TableUiInfo.TableDef,
            FieldDef = Field,
            FieldName = Field.Name,
            Control = Control,
            Table = TableUiInfo.Table
        });
    }
}
