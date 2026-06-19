# DataModule

`DataModule` is the base class for Tripous.Data business/data modules.
It represents one registered module: a browser list, a single item table, and any detail/subdetail tables under that item.

It is a data-layer class.
It must not show UI, modal dialogs, message boxes, or wait for user interaction.
UI feedback belongs to forms and other UI-layer classes.

## Role

A `DataModule` is built from a `ModuleDef`.
During initialization it creates:

- A `SqlStore` for the module connection.
- A `DataSet`.
- `tblList`, used by list/browser views.
- `tblItem`, the top table of the edit tree.
- Detail `MemTable` tables under `tblItem`.
- Stock lookup tables.
- A `TableSet`, which performs load/save/delete operations.

```csharp
ModuleDef ModuleDef = DataRegistry.Modules.Get("Customer");

DataModule Module = new();
Module.Initialize(ModuleDef);
```

Applications usually create data modules through registry and UI infrastructure, but the workflow is the same.

## Tables

The important table properties are:

- `tblList`, the list result table.
- `tblItem`, the current item table.
- `ItemTables`, all item/detail tables.
- `Stocks`, lookup tables used by the module.
- `Tables`, all module tables including `tblList`.
- `RowProviderHost`, usually `tblItem`.

```csharp
MemTable Customer = Module.tblItem;
MemTable Address = Module["CustomerAddress"];

DataRow Row = Module.CurrentRow;
object Id = Module.Id;
```

The indexer resolves a table by name.

```csharp
MemTable Lines = Module["SalesInvoiceLine"];
```

## List Workflow

The list workflow fills `tblList`.

```csharp
Module.ListSelect(@"
select
    Id,
    Code,
    Name
from Customer
order by Name");
```

`ListSelect(SelectDef)` is used when the SELECT comes from module registration.

```csharp
SelectDef SelectDef = Module.ModuleDef.SelectList[0];

Module.ListSelect(SelectDef);
```

List changes can also be saved or cancelled.

```csharp
Module.ListSave();
Module.ListCancel();
```

## Item Workflow

The item workflow is the normal insert/edit/delete flow.

```csharp
Module.Insert();

Module.tblItem.CurrentRow["Name"] = "Acme";

object Id = Module.Commit();
```

Editing loads the item and its details.

```csharp
Module.Edit(Id);

Module.tblItem.CurrentRow["Name"] = "Acme Ltd";

Module.Commit(Reselect: true);
```

Deleting also goes through the table tree and transaction pipeline.

```csharp
Module.Delete(Id);
```

Cancelling rejects pending item/detail changes.

```csharp
if (Module.HasChanges())
    Module.Cancel();
```

`State` reports the current data mode.

```csharp
DataMode State = Module.State;
```

After `Insert()`, the state is `Insert`.
After `Edit()` or successful `Commit()`, the state is `Edit`.

## Default Values

Data defaults belong in the `DataModule`, not in UI code.
Override `SetDefaultValues()` when a module needs application-specific values.

```csharp
protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
{
    base.SetDefaultValues(Table, Row, TableDef);

    if (TableDef.Name == "Note")
    {
        if (Sys.IsNull(Row["CreatedAt"]))
            Row["CreatedAt"] = DateTime.Now;

        Row["UpdatedAt"] = DateTime.Now;

        if (Sys.IsNull(Row["IsPinned"]))
            Row["IsPinned"] = 0;
    }
}
```

The base implementation also applies descriptor defaults and `SqlValueProviders`.
For the top item table it fills audit fields such as `CreatedBy`, `CreatedAt`, `ModifiedBy`, and `ModifiedAt`, when those columns exist.

## Code Providers

If the module has a `Code` field with a code provider, `DataModule` can assign the next code during commit.
The assignment happens inside the same database transaction.

```csharp
protected override void TableSet_TransactionStageCommit(object Sender, TransactionEventArgs Args)
{
    base.TableSet_TransactionStageCommit(Sender, Args);

    if (Args.Stage == TransactionStage.Post && Args.ExecTime == ExecTime.Before)
    {
        DbTransaction Transaction = Args.Transaction;
    }
}
```

The base handler uses this same `Post/Before` point to assign code values.
Derived modules can use it for validation or related updates that must be atomic with the item save.

## Lifecycle Hooks

`DataModule` exposes protected hooks around the main workflow.

- `Inserting()` and `Inserted()`.
- `Editing(RowId)` and `Edited(RowId)`.
- `Deleting(RowId)` and `Deleted(RowId)`.
- `Commiting(Reselect)` and `Commited(Reselect, RowId)`.
- `ColumnChanging()` and `ColumnChanged()`.
- `NewRowAdding()` and `NewRowAdded()`.

Example: decrypting a loaded row after edit.

```csharp
protected override void Edited(object RowId)
{
    base.Edited(RowId);

    DataRow Row = tblItem.Rows.Count > 0 ? tblItem.Rows[0] : null;

    if (Row != null)
        DecryptRow(Row);
}
```

Example: setting values when a detail row is added.

```csharp
protected override void NewRowAdded(MemTable Table, DataTableNewRowEventArgs Args)
{
    base.NewRowAdded(Table, Args);

    if (Table.TableName == "SalesInvoiceLine")
        Args.Row["DisplayOrder"] = Table.Rows.Count;
}
```

## Transaction Hooks

`TableSet_TransactionStageCommit()` and `TableSet_TransactionStageDelete()` are the hooks for transaction-aware work.

Use them when the module must:

- Validate persisted state inside the transaction.
- Reserve or release stock.
- Post finance movements.
- Update linked documents.
- Write related rows using the same `DbTransaction`.

```csharp
protected override void TableSet_TransactionStageCommit(object Sender, TransactionEventArgs Args)
{
    base.TableSet_TransactionStageCommit(Sender, Args);

    if (Args.Stage == TransactionStage.Post && Args.ExecTime == ExecTime.Before)
        ValidateDocumentBeforeCommit(Args.Transaction);
}
```

This pattern is used heavily by tERP document, stock, payment, and finance modules.

## Batch Work

`CommitBatch()` delegates to `TableSet.CommitBatch()`.
It is used when many rows must be posted in chunks.

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

Module.CommitBatch(Args);
```

Use `BeforeFunc` to prepare one item and return whether it should be posted.
Use `AfterFunc` to decide whether the loop continues.

## Common Extension Points

Typical derived modules override a small set of methods.

- `SetDefaultValues()` for row defaults.
- `Edited()` for post-load transformations.
- `Commit()` when data must be transformed before save and restored after save.
- `NewRowAdded()` for detail row defaults.
- `TableSet_TransactionStageCommit()` for validation and related posting.

Keep these overrides data-only.
Do not ask the user questions, show messages, or depend on a visible form from inside a `DataModule`.
