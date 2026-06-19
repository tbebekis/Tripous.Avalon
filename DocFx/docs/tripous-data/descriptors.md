# Descriptors

Tripous.Data descriptors are metadata objects.
They describe modules, tables, fields, SELECT statements, lookups, locators, code providers, document handlers, and configuration properties.

Descriptors are registered in `DataRegistry`.
At runtime they are used to create `DataModule` instances, `MemTable` tables, SQL statements, filters, lookups, locators, and generated UI.

## Main Descriptor Types

The most common descriptor types are:

- `ModuleDef`, a registered data module.
- `TableDef`, a database table inside a module.
- `FieldDef`, a table field.
- `SelectDef`, a named SELECT used by a list/browser view or stock table.
- `LookupDef`, a value list source.
- `LocatorDef` and `LocatorFieldDef`, a searchable selector and its fields.
- `CodeProviderDef`, an automatic numbering definition.
- `DocumentHandlerDef`, a document behavior handler.
- `ConfigPropertyDef`, an application configuration setting.

Most descriptors inherit from `BaseDef`, so they have common identity and title behavior such as `Name`, `TitleKey`, and `Title`.

## ModuleDef

`ModuleDef` is the top descriptor for a data module.
It connects the module name with its data module class, connection, list SELECTs, top table, security level, and delete behavior.

```csharp
string SqlText = @"
select
    Id,
    Code,
    Name
from Customer
order by Name";

ModuleDef Module = DataRegistry.AddModule(
    "Customer",
    TitleKey: "Customers",
    ClassName: typeof(CustomerDataModule).FullName,
    ListSelectSql: SqlText,
    IsSingleSelect: true);
```

`ModuleDef.Create()` creates the associated `DataModule`.

```csharp
DataModule ModuleInstance = Module.Create();
```

`DataRegistry.CreateModule()` is the usual shortcut.

```csharp
DataModule ModuleInstance = DataRegistry.CreateModule("Customer");
```

## TableDef

`TableDef` describes one table in the module table tree.
The top table is `ModuleDef.Table`.

```csharp
TableDef Table = Module.Table;

Table.Name = "Customer";
Table.KeyField = "Id";
Table.AddId();
Table.AddString("Code", 40, Flags: FieldFlags.Required | FieldFlags.Searchable);
Table.AddString("Name", 96, Flags: FieldFlags.Required | FieldFlags.Searchable);
Table.AddBoolean("IsActive");
```

`TableDef` is used to:

- Create the in-memory `MemTable`.
- Create `DataColumn` objects.
- Build INSERT, UPDATE, DELETE, and SELECT SQL.
- Define UI editors and grid columns.
- Define table joins and detail tables.

## Detail Tables

A detail table is another `TableDef` under the top table or under another detail.

```csharp
TableDef Address = Table.AddDetail(
    "CustomerAddress",
    MasterField: "Id",
    DetailField: "CustomerId");

Address.AddId();
Address.AddString("CustomerId", 40, Flags: FieldFlags.Required | FieldFlags.Hidden);
Address.AddString("Street", 128);
Address.AddString("City", 64);
```

`DataModule.Initialize()` turns this descriptor tree into a `MemTable` tree.
`TableSet` then loads and posts the table tree.

## FieldDef

`FieldDef` describes a table field.
It carries data type, length, decimal count, title, group, flags, lookup source, locator, default value, expression, and code provider metadata.

```csharp
FieldDef Field = Table.AddString(
    "Name",
    MaxLength: 96,
    Group: "General",
    TitleKey: "Name",
    Flags: FieldFlags.Required | FieldFlags.Searchable);
```

Fields can be configured fluently.

```csharp
Table.AddString("Notes", 4000)
    .SetMemo()
    .SetTitleKey("Notes");
```

Lookup fields point to a registered lookup source.

```csharp
Table.AddIntegerLookupId(
    "CountryId",
    LookupSource: "Country",
    TitleKey: "Country");
```

Code fields can point to a code provider.

```csharp
Table.AddString("Code", 40, Flags: FieldFlags.Required)
    .SetCodeProviderName("Customer");
```

`DataModule` uses field defaults and code provider metadata during insert and commit.

## SelectDef

`SelectDef` describes a named SELECT statement and its filters.
It is most often used for the browser/list part of a module.

```csharp
SelectDef SelectDef = Module.SelectList[0];

SelectDef.DisplayLabels["Code"] = "Code";
SelectDef.DisplayLabels["Name"] = "Name";

SelectDef.AddFilter(
    "Name",
    FilterDataType: DataFieldType.String,
    ConditionOp: ConditionOp.Contains);
```

The SELECT text is executed by `DataModule.ListSelect()` and loaded into `tblList`.
Filter definitions are used by the UI filter panel and SQL filter helpers.

## Lookups

`LookupDef` describes a value list.
The source may be:

- An enum type.
- A table name.
- A SELECT statement.
- A custom `LookupSource` class.

```csharp
LookupDef Lookup = DataRegistry.AddLookupWithTableName(
    "Country",
    "Country",
    FormName: "Country");

Lookup.ValueField = "Id";
Lookup.DisplayField = "Name";
```

Fields reference lookups by name.

```csharp
Table.AddIntegerLookupId("CountryId", "Country");
```

## Locators

`LocatorDef` describes a searchable selector for large reference tables.
It identifies the source table or source SQL, the key field, the form name, and the fields shown to the user.

```csharp
LocatorDef Locator = DataRegistry.AddLocator(
    "Customer",
    SourceTableName: "Customer",
    KeyField: "Id",
    FormName: "Customer");

Locator.Add("Code", DataFieldType.String, TargetField: "CustomerCode");
Locator.Add("Name", DataFieldType.String, TargetField: "CustomerName");
```

A field can reference a locator by name.

```csharp
Table.AddString("CustomerId", 40)
    .SetTitleKey("Customer")
    .SetFlags(FieldFlags.Required);

Table.GetField("CustomerId").Locator = "Customer";
```

Locators are used by data and UI code to search, select, and assign reference values.

## Descriptor References

Many descriptors contain child descriptors or references to other descriptors.
For example:

- `ModuleDef.Table` owns the top `TableDef`.
- `TableDef.Fields` owns `FieldDef` objects.
- `TableDef.Details` owns child `TableDef` objects.
- `SelectDef.Owner` points to its owner.
- `FieldDef.TableDef` points to its table.
- `LocatorFieldDef.LocatorDef` points to its locator.

`UpdateReferences()` reconnects those owner references.

```csharp
DataRegistry.Modules.UpdateReferences();
DataRegistry.UpdateLocatorReferences();
```

`DataModule.Initialize()` also updates references before it builds runtime tables.

## Runtime Use

Descriptors drive the runtime model.

- `ModuleDef.Create()` creates a `DataModule`.
- `TableDef.CreateDescriptorTable()` creates a `MemTable`.
- `TableDef.BuildSql()` creates table SQL statements.
- `SelectDef` defines list SELECTs and filters.
- `LookupDef.Create()` creates a `LookupSource`.
- `LocatorDef.Create()` creates a `Locator`.
- `FieldDef` controls data type, defaults, lookup, locator, and UI behavior.

This is why the same descriptor tree can serve data access, generated SQL, filtering, lookup loading, locators, and desktop UI generation.
