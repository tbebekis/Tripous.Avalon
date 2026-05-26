/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

public class DbOpContext
{
    // ● construction
    public DbOpContext(string ModuleName, SqlStore Store, DbTransaction Transaction, MemTable TopTable, bool CascadeDeletes = true, bool GenerateSql = false)
    {
        this.ModuleName = ModuleName;
        this.Store = Store;
        this.Transaction = Transaction;
        this.TopTable = TopTable;

        // flat table list
        this.FlatList = TopTable.GetTreeAsFlatList();
        
        // generate sql
        if (GenerateSql)
        {
            foreach (MemTable Table in this.FlatList)
                SqlStatementBuilder.BuildSql(ModuleName, Table, Store, Table == TopTable);
        }
        
        // max detail level
        MaxDetailLevel = 0;
        foreach (MemTable Table in this.FlatList)
            MaxDetailLevel = Math.Max(MaxDetailLevel, Table.Level);
 
        // cascade deletes
        this.CascadeDeletes = CascadeDeletes;
    }
    
    // ● properties
    /// <summary>
    /// <para><b>WARNING:</b> The <see cref="ModuleName"/> and a TableName are used in constructing a unique StatementName.</para>
    /// <para>The StatementName is used with the <see cref="SqlStore.GetNativeSchemaFromTableName"/>
    /// so the <c>ModuleName.TableName</c> must construct a unique name because schema DataTables are stored in the <see cref="SqlCache"/> under that unique name. </para>
    /// </summary>
    public string ModuleName { get; }
    public SqlStore Store { get;  }
    public DbTransaction Transaction { get; }
    public MemTable TopTable { get;  }
    public List<MemTable> FlatList { get; } 

    public int MaxDetailLevel { get;  }
    public bool CascadeDeletes { get;  }
    
    /// <summary>
    /// Returns true if Oids are needed before commiting a row to the database
    /// </summary>
    public bool OidIsBefore => Store.Provider.OidMode == OidMode.Generator;
    /// <summary>
    /// Returns true if Oids are needed after commiting a row to the database
    /// </summary>
    public bool OidIsAfter => !OidIsBefore;
}