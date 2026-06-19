# Data Extensions

Tripous.Data adds extension methods to common ADO.NET data types.

These helpers reduce repetitive `DBNull` checks, column lookup code, row copying code and blob conversion code.

## Extension Classes

- `DataRowExtensions`
- `DataRowViewExtensions`
- `DataTableExtensions`
- `DataColumnExtensions`

## Typed Row Reads

`DataRow` and `DataRowView` provide typed `As...()` methods by column name or column index.

```csharp
DataRow Row = Table.Rows[0];

string Name = Row.AsString("Name");
int Quantity = Row.AsInteger("Quantity", 0);
decimal Amount = Row.AsDecimal("Amount", 0);
bool IsActive = Row.AsBoolean("IsActive", false);
DateTime CreatedAt = Row.AsDateTime("CreatedAt", DateTime.MinValue);
```

These methods return a default value when the column value is `DBNull`, cannot be converted, or is not available.

The same pattern exists for `DataRowView`.

```csharp
DataRowView RowView = View[0];

string Code = RowView.AsString("Code");
int LineNo = RowView.AsInteger("LineNo");
```

## Safe Set And Try Get

`SetValue()` assigns only when the row exists, is not deleted and contains the field.

```csharp
Row.SetValue("Name", "Customer A");
```

`TryGetValue()` is useful when code works with dynamic table shapes.

```csharp
if (Row.TryGetValue("TotalAmount", out object Value))
{
    decimal Amount = Sys.AsDecimal(Value);
}
```

## Copying Rows And Tables

`CopyTo()` assumes identical schemas. `SafeCopyTo()` copies only columns that exist in both source and destination.

```csharp
SourceRow.SafeCopyTo(DestRow);
SourceTable.SafeCopyTo(DestTable);
```

For appending rows:

```csharp
SourceRow.AppendTo(DestTable);
SourceTable.SafeAppendTo(DestTable);
```

For preserving row states, use `CopyExactState()`.

```csharp
DataTable Copy = SourceTable.CopyExactState();
DataSet DataSetCopy = SourceDataSet.CopyExactState();
```

This is useful when code must preserve `Added`, `Modified`, `Deleted` and `Unchanged` row states.

## Structure Helpers

`DataTableExtensions` includes helpers for table structure work.

```csharp
bool HasName = Table.ContainsColumn("Name");
DataColumn Column = Table.FindColumn("Name");
DataTable EmptyCopy = Table.CopyStructure();

TargetTable.ClearSchemaAndData();
TargetTable.CopyStructureAndRowsFrom(SourceTable);
```

`DataColumnExtensions` can clone columns and copy column structure.

```csharp
DataColumn Clone = SourceColumn.CloneColumn();
SourceColumn.CopyStructureTo(TargetTable);
```

## Column Metadata

Several column properties are stored in `DataColumn.ExtendedProperties`.

```csharp
DataColumn Column = Table.GetColumn("Description");

Column.IsVisible(true);
Column.IsMemo(true);
Column.SetTitleKey("Description");
Column.SetWidth(240);
```

Common pseudo-properties:

- `Visible`
- `IsDateTime`
- `IsDate`
- `IsTime`
- `IsCheckBox`
- `IsMemo`
- `IsImage`
- `TitleKey`
- `Width`

These metadata flags are useful for UI and serialization code.

## Blob Helpers

Rows and row views can read and write blob fields using streams or strings.

```csharp
using MemoryStream Stream = new MemoryStream(Buffer);

Row.StreamToBlob("Content", Stream);
Row.BlobToStream("Content", Stream);

Row.StringToBlob("JsonText", JsonText);
string Text = Row.BlobToString("JsonText");
```

The blob helpers support columns of type `byte[]`, `object`, and in string conversion cases, `string`.

## Locating Rows

`Locate()` searches rows by one or more fields.

```csharp
DataRow Found = Table.Locate(
    "Code",
    "CUST-001",
    LocateOptions.CaseInsensitive);
```

For multiple fields:

```csharp
DataRow Found = Table.Locate(
    ["CompanyId", "Code"],
    [CompanyId, "CUST-001"],
    LocateOptions.CaseInsensitive);
```

`LocateOptions.PartialKey` allows prefix matching for string values.

## SQL IN Lists

`GetKeyValuesList()` creates chunks for SQL `IN (...)` clauses.

```csharp
List<string> Chunks = Table.GetKeyValuesList("Id", 100);

foreach (string Chunk in Chunks)
{
    string SqlText = $"select * from Customer where Id in ({Chunk})";
}
```

This avoids creating a single huge `IN` list when an RDBMS has a practical parameter or expression limit.
