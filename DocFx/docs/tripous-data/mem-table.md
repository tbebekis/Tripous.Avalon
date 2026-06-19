# MemTable

`MemTable` is the main in-memory table type of Tripous.Data.
It extends `DataTable` and adds the behavior Tripous needs for SQL-first data modules: current row tracking, master-detail relations, row providers, table SQL statements, and change helpers.

It is not an ORM entity.
It remains a `DataTable`, so it works naturally with `DataRow`, `DataView`, binding, and ADO.NET.

## Loading Data

Most code gets a `MemTable` from `SqlStore`.

```csharp
SqlStore Store = Db.DefaultStore;

MemTable Table = Store.Select(@"
select *
from Customer
where IsActive = :IsActive",
    true);
```

Tripous SQL parameters use the global `:` prefix.
At execution time the provider translates them to the native prefix of the selected RDBMS.

A `MemTable` can also be created manually.

```csharp
MemTable Table = new("Customer");

Table.Columns.Add("Id", typeof(string));
Table.Columns.Add("Name", typeof(string));

Table.KeyField = "Id";
```

When `AutoGenerateGuidKeys` is true, and the key is a single string column, `MemTable` can fill a new Guid key automatically.

```csharp
DataRow Row = Table.AddNewRow();

Row["Name"] = "Acme";
```

## Current Row

`MemTable` tracks a `CurrentRow`.
This is important for forms, locators, master-detail tables, and any component that needs to know the selected logical row.

```csharp
Table.CurrentRowChanged += (Sender, Args) =>
{
    DataRow Row = Table.CurrentRow;
};

Table.UpdateCurrentRow();
```

`CurrentRowView` gives the matching `DataRowView` from the table `DataView`.

```csharp
DataRowView RowView = Table.CurrentRowView;
```

## Filtering And Binding

`MemTable.DataView` is the view used for binding to grids and simple controls.

`MemTable` keeps two filters:

- `UserRowFilter`, set by user or application code.
- `DetailRowFilter`, set by master-detail logic.

The final `DataView.RowFilter` combines both.

```csharp
Table.UserRowFilter = "Name like 'A%'";
```

This lets UI filtering and master-detail filtering work together without one overwriting the other.

## Master-Detail Tables

`MemTable` can form a table tree.
A master table owns detail tables through `Details`.

```csharp
DataSet DataSet = new();

MemTable Customer = new("Customer");
MemTable Order = new("SalesOrder");

MemTable.AddToDataSet(DataSet, Customer);
MemTable.AddToDataSet(DataSet, Order);

Customer.Columns.Add("Id", typeof(string));
Order.Columns.Add("Id", typeof(string));
Order.Columns.Add("CustomerId", typeof(string));

Customer.KeyField = "Id";
Order.MasterField = "Id";
Order.DetailField = "CustomerId";

Customer.AddDetail(Order);
Customer.DetailsActive = true;
```

When `Customer.CurrentRow` changes, the detail table receives a `DetailRowFilter`.
New detail rows also get the master field values automatically.

```csharp
DataRow CustomerRow = Customer.AddNewRow();
CustomerRow["Id"] = MemTable.GenId();

DataRow OrderRow = Order.AddNewRow();

string CustomerId = OrderRow["CustomerId"].ToString();
```

`RefreshDetails()` updates the current row and refreshes all detail filters.

```csharp
Customer.RefreshDetails();
```

## Row Providers

`MemTable` implements `IRowProvider`.
It exposes:

- `TableName`.
- `CurrentRow`.
- `CurrentRowChanged`.
- `UpdateCurrentRow()`.

It also implements `IRowProviderHost`.
That allows a top table to expose its detail tables by name.

```csharp
IRowProvider Provider = Customer.GetRowProvider("SalesOrder");
DataRow Row = Provider.CurrentRow;
```

This pattern is used by data modules, locators, and UI code that need a row from a known table without depending on the whole object that owns it.

## Table SQL

Each `MemTable` has a `Sqls` property of type `TableSqls`.
It stores the SQL statements used for table operations.

```csharp
Table.Sqls.SelectSql = "select * from Customer";
Table.Sqls.InsertRowSql = "insert into Customer (Id, Name) values (:Id, :Name)";
Table.Sqls.UpdateRowSql = "update Customer set Name = :Name where Id = :Id";
Table.Sqls.DeleteRowSql = "delete from Customer where Id = :Id";
```

`DbOps` uses these statements when it posts changes for a table or a whole table tree.

## Changes

Because `MemTable` is still a `DataTable`, normal row states are used.
Tripous adds helpers that work on the whole table tree.

```csharp
bool HasChanges = Customer.HasChangesAll();

Customer.AcceptChangesAll();
Customer.RejectChangesAll();
```

There are also helpers for clearing or deleting a full tree.

```csharp
Customer.ClearAll();
Customer.DeleteAll();
```

`ClearAll()` removes data without setting deleted row states.
`DeleteAll()` marks rows as deleted first.

## Where It Is Used

`MemTable` is central in Tripous.Data.

- `SqlStore` returns `MemTable` instances from SELECT statements.
- `DataModule` uses them for list, item, and detail tables.
- `TableSet` loads and posts item/detail table trees.
- `DbOps` posts inserts, updates, and deletes using `TableSqls`.
- `DetailList` keeps master-detail relations active.
- `Locator` uses row providers to read and assign values from current rows.

In practice, `MemTable` is the bridge between raw SQL and the higher-level Tripous data workflow.
