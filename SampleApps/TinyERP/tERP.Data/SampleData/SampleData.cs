/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public abstract class SampleData
{
    static List<SampleData> SampleDataList = [];
    static protected SqlStore Store = SqlStores.CreateDefaultSqlStore();
    static protected readonly Dictionary<string, MemTable> SampleTables = new(StringComparer.OrdinalIgnoreCase);

    static protected bool CanAdd(string ModuleName, out DataModule Module)
    {
        bool Result = false;
        Module = null;
        ModuleDef ModuleDef = DataRegistry.Modules.Get(ModuleName);
        string TableName = ModuleDef.Table.Name;
        if (Store.TableExists(TableName) && Store.TableIsEmpty(TableName))
        {
            Module = ModuleDef.Create();
            Result = true;
        }

        return Result;
    }
    static protected DataRow AddRow(MemTable Table, params (string ColumnName, object Value)[] Values)
    {
        DataRow Row = Table.NewRow();
        foreach (var Value in Values)
            Row[Value.ColumnName] = Value.Value;
        Table.Rows.Add(Row);
        return Row;
    }

    protected abstract void AddSampleDataInternal();

    protected virtual bool GetIsAdded()
    {
        string Key = $"SampleDataAdded.{VersionNumber}";
        bool Result = Db.MainIni.ReadBool(Key, false);
        return Result;
    }

    protected virtual void SetIsAdded()
    {
        string Key = $"SampleDataAdded.{VersionNumber}";
        Db.MainIni.WriteBool(Key, true);
    }

    // ● construction
    static SampleData()
    {
        SampleDataList.AddRange([
            new SampleData1(),
            new SampleData2(),
        ]);
    }

    public SampleData()
    {
    }

    // ● public
    static public SampleData[] GetNotAdded()
    {
        SampleData[] Result = SampleDataList.Where(SD => !SD.IsAdded).ToArray();
        return Result;
    }

    static public async Task AddSampleDataAsync(SampleData[] NotAddedSampleData)
    {
        bool Flag = Db.Settings.LogSqlStatements;
        Db.Settings.LogSqlStatements = false;
        try
        {
            await Task.Run(() =>
            {
                foreach (SampleData SampleData in NotAddedSampleData)
                    SampleData.AddSampleDataInternal();
            });
        }
        finally
        {
            Db.Settings.LogSqlStatements = Flag;
        }
    }

    public bool IsAdded => GetIsAdded();
    public abstract int VersionNumber { get; }
}
