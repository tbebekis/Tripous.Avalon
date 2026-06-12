/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;


/// <summary>
/// Database operation context.
/// </summary>
public class DbOpContext
{
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
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
    /// <summary>
    /// The <see cref="SqlStore"/> this context operates with.
    /// </summary>
    public SqlStore Store { get;  }
    /// <summary>
    /// The database transaction this context operates under.
    /// </summary>
    public DbTransaction Transaction { get; }
    /// <summary>
    /// The top-level <see cref="MemTable"/> of the data tree this context operates on.
    /// </summary>
    public MemTable TopTable { get;  }
    /// <summary>
    /// The <see cref="TopTable"/> and all its descendant tables, as a flat list.
    /// </summary>
    public List<MemTable> FlatList { get; } 

    /// <summary>
    /// The maximum detail (nesting) level among the tables in <see cref="FlatList"/>.
    /// </summary>
    public int MaxDetailLevel { get;  }
    /// <summary>
    /// Indicates whether deletes should be cascaded to detail tables.
    /// </summary>
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