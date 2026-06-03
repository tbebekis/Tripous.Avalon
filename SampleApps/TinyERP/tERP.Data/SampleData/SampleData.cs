/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class SampleData
{
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
    
    static protected void AddCodeProviderPatterns()
    {
        string TableName = DbConfig.SysNumberSeriesTableName;
        if (Store.TableExists(TableName) && Store.TableIsEmpty(TableName))
        {
            Dictionary<string, string> CodeProviderPatters = Registry.GetCodeProviderPatterns();
            CodeProviderEntries.SeedPatterns(CodeProviderPatters);
        }
    }

    protected virtual void AddSampleDataInternal()
    {
    }
    
    // ● construction
    public SampleData()
    {
    }

    // ● public
    static public async Task AddSampleDataAsync()
    {
        bool Flag = Db.Settings.LogSqlStatements;
        Db.Settings.LogSqlStatements = false;
        try
        {
            await Task.Run(() =>
            {
                AddCodeProviderPatterns();
                
                List<SampleData> SampleDataList = [];
                SampleDataList.AddRange([
                    new SampleData1(),
                    new SampleData2(),
                ]);
                
                foreach (SampleData SampleData in SampleDataList)
                    SampleData.AddSampleDataInternal();
            });
        }
        finally
        {
            Db.Settings.LogSqlStatements = Flag;
        }
    }
}