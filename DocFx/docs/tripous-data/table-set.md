# TableSet

`TableSet` coordinates the tables of a data module.
It owns the list table, the item table, the item/detail table tree, and the stock lookup tables used by the module.

Application code normally does not create or call `TableSet` directly.
`DataModule` creates it during initialization and exposes higher-level methods such as `Insert()`, `Edit()`, `Delete()`, `Commit()`, and `ListSelect()`.

## Role

A `TableSet` connects four things:

- A `SqlStore`, used for database access.
- A list `MemTable`, used by browser/list views.
- An item `MemTable`, used by edit forms.
- The item detail tree, built with `MemTable.Details`.

`DataModule` creates it like this.

```csharp
TableSetFlags Flags = TableSetFlags.None;

if (!ModuleDef.CascadeDeletes)
    Flags |= TableSetFlags.NoCascadeDeletes;

TableSet = new TableSet(Name, Store, tblList, tblItem, Stocks, Flags);
```

The constructor checks the item table tree, generates missing SQL statements when requested, and loads stock tables.

## List Operations

The list table is used by browser views.
`ListSelect()` executes a SELECT and fills the list `MemTable`.

```csharp
TableSet.ListSelect(@"
select
    Id,
    Code,
    Name
from Customer
order by Name");
```

`DataModule.ListSelect()` delegates to `TableSet`.

```csharp
public virtual void ListSelect(string SqlText)
{
    if (!string.IsNullOrWhiteSpace(SqlText))
        TableSet.ListSelect(tblList, SqlText);
}
```

The list table can also be saved or cancelled.

```csharp
TableSet.ListSave();
TableSet.ListCancel();
```

This is useful for simple list-style editing.
Most item forms use the item operations instead.

## Item Operations

The item table is the top table of the edit tree.
Details and subdetails hang under it.

For insert, `TableSet` clears the tree and creates a new top row.

```csharp
TableSet.ProcessInsert();
```

For edit, it loads the top row and then loads all detail tables.

```csharp
TableSet.Load(RowId);
```

For delete, it loads the full table tree, marks rows as deleted, and posts the deletes in a transaction.

```csharp
TableSet.Delete(RowId);
```

For save, it posts inserts, updates, and deletes for the whole table tree.

```csharp
object Id = TableSet.Commit(Reselect: true);
```

When `Reselect` is true, the committed row is loaded again after the transaction.
This is useful when database triggers, default values, or calculated fields may have changed persisted data.

## Detail Loading

Detail tables are loaded from their own `Table.Sqls.SelectSql`.
`TableSet` limits detail SELECTs by using the master key values.

Conceptually, a detail SELECT becomes:

```sql
select *
from SalesOrderLine
where SalesOrderLine.SalesOrderId in (...)
```

The loaded details are appended into the detail `MemTable`.
Then `MemTable.RefreshDetails()` keeps the visible detail rows synchronized with the current master row.

## SQL Generation

`TableSetFlags.GenerateSql` tells `TableSet` to generate missing table operation SQL through `SqlStatementBuilder`.

```csharp
TableSet TableSet = new(
    "Customer",
    Store,
    ListTable,
    ItemTable,
    Stocks,
    TableSetFlags.GenerateSql);
```

The generated statements are stored in `MemTable.Sqls`.

```csharp
string SelectSql = ItemTable.Sqls.SelectSql;
string InsertSql = ItemTable.Sqls.InsertRowSql;
string UpdateSql = ItemTable.Sqls.UpdateRowSql;
string DeleteSql = ItemTable.Sqls.DeleteRowSql;
```

This is how descriptor-built modules can post changes without handwritten CRUD SQL for every table.

## Transactions

`Commit()` and `Delete()` use `SqlTransactionContext`.
The context owns both the connection and the transaction and disposes them together.

During commit, the sequence is:

- `Start`.
- `Post`.
- `Commit`.
- `Rollback`, only on failure.

Each stage is raised before and after the action.

```csharp
TableSet.TransactionStageCommit += (Sender, Args) =>
{
    if (Args.Stage == TransactionStage.Post && Args.ExecTime == ExecTime.Before)
    {
        DbTransaction Transaction = Args.Transaction;
    }
};
```

Data modules override these hooks when they must validate or update related data inside the same transaction.

```csharp
protected override void TableSet_TransactionStageCommit(object Sender, TransactionEventArgs Args)
{
    base.TableSet_TransactionStageCommit(Sender, Args);

    if (Args.Stage == TransactionStage.Post && Args.ExecTime == ExecTime.Before)
        ValidateDocumentBeforeCommit(Args.Transaction);
}
```

This pattern is used by document, stock, payment, and finance modules.

## Deletes

Delete behavior depends on `TableSetFlags.NoCascadeDeletes`.

When cascade deletes are enabled, `DbOps` can post deletes in an order suitable for a detail tree.
When `NoCascadeDeletes` is set, deletes happen top to bottom and the database is allowed to reject the operation through foreign key constraints.

```csharp
TableSetFlags Flags = TableSetFlags.NoCascadeDeletes;
```

`DataModule` maps this from `ModuleDef.CascadeDeletes`.

## Change Tracking

`TableSet` delegates change tracking to the item `MemTable` tree.

```csharp
bool HasChanges = TableSet.HasChanges();

TableSet.AcceptChanges();
TableSet.RejectChanges();
```

`PostChanges()` posts the current changes but does not call `AcceptChangesAll()`.
`Commit()` wraps `PostChanges()` in a transaction and accepts changes after successful commit.

## Batch Commit

`CommitBatch()` is used for repeated insert/edit operations.
It keeps a transaction open and commits every `TransLimit` posted rows.

```csharp
BatchCommitArgs Args = new(
    BeforeFunc: () =>
    {
        Module.Insert();
        Module.tblItem.CurrentRow["Name"] = "Customer";
        return true;
    },
    AfterFunc: LastId => false,
    TransLimit: 100);

TableSet.CommitBatch(Args);
```

`BeforeFunc` prepares one iteration and returns whether a post is needed.
`AfterFunc` decides whether the loop continues.

## Where It Is Used

`TableSet` is the persistence engine behind `DataModule`.

- `DataModule.ListSelect()` calls `TableSet.ListSelect()`.
- `DataModule.Insert()` calls `TableSet.ProcessInsert()`.
- `DataModule.Edit()` calls `TableSet.Load()`.
- `DataModule.Delete()` calls `TableSet.Delete()`.
- `DataModule.Commit()` calls `TableSet.Commit()`.
- `DataModule.HasChanges()` calls `TableSet.HasChanges()`.
- `DataModule.CommitBatch()` calls `TableSet.CommitBatch()`.

In practice, `TableSet` keeps the list/item/detail `MemTable` model consistent while `DataModule` provides the public workflow used by application code and UI forms.
