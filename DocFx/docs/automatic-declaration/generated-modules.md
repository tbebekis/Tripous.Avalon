# Generated Modules

Generated modules are written to `RegistryVersionN.Modules.cs`.

This is where the Registration Builder turns schema metadata into Tripous module descriptors.

The generated code registers:

- `ModuleDef`
- top `TableDef`
- detail and subdetail `TableDef` objects
- list `SelectDef`
- joins
- fields
- filters
- field groups
- field flags

## Module Source

A generated module starts from top table metadata.

Example schema header:

```sql
Table: DocumentType
Module: DocumentType DocumentTypeDataModule
Group: Documents
Form: DocumentType
FilterFields: Code, Name, ModuleName, NumberSeries__Code
FieldGroups: Posting, Cancellation, Output, Appearance, Notes
```

The Registration Builder uses this metadata together with the table fields and foreign keys.

## Generated Method

Each module is generated as a private method.

Example shape:

```csharp
static void RegisterModule_DocumentType()
{
    ModuleDef Module;
    TableDef tblTop;
    SelectDef SelectDef;
    string SqlText;

    SqlText = @"select ...";
    Module = DataRegistry.AddOrUpdateModule("DocumentType", ClassName: "DocumentTypeDataModule", ListSelectSql: SqlText);

    if (Module.Table.Fields.Count > 0)
        return;

    tblTop = Module.Table;
    tblTop.Name = "DocumentType";
    tblTop.KeyField = "Id";
}
```

The guard prevents duplicate field registration if the module has already been initialized.

## RegisterModules

The generated file overrides `RegisterModules()`.

It calls all generated module methods for the version.

```csharp
public override void RegisterModules()
{
    RegisterModule_Account();
    RegisterModule_Asset();
    RegisterModule_DocumentType();
}
```

The application calls `RegisterModules()` from the handwritten registry coordinator.

## List Select SQL

Each generated module includes list SQL.

The list SQL is used by the module's list view and by filters.

It includes:

- top table fields
- enum display columns
- joined lookup display fields
- generated join aliases
- optional `ListWhere` conditions

Generated join aliases use this convention:

```text
JOIN_ALIAS__FIELD_NAME
```

Example:

```sql
COALESCE(NumberSeries.Code, '') as NumberSeries__Code,
COALESCE(NumberSeries.Name, '') as NumberSeries__Name
```

## Top Table Descriptor

The top table becomes the module's main `TableDef`.

Generated code sets:

- table name
- key field
- field groups
- fields
- defaults
- nullability
- flags
- lookup metadata
- locator metadata
- code provider metadata

Example:

```csharp
tblTop.Name = "DocumentType";
tblTop.KeyField = "Id";
tblTop.FieldGroups.AddRange(["Posting", "Cancellation", "Output", "Appearance", "Notes"]);
tblTop.AddId("Id").SetNullable(false);
tblTop.AddString("Code", MaxLength: 40, Flags: FieldFlags.Required | FieldFlags.ReadOnlyEdit).SetNullable(false);
tblTop.AddString("Name", MaxLength: 96, Flags: FieldFlags.Required).SetNullable(false);
```

Field metadata in `Schema.sql` becomes chained `FieldDef` configuration.

Examples:

```csharp
tblTop.AddBoolean("AffectsStock", Flags: FieldFlags.Required).SetNullable(false).SetDefaultValue("0").SetGroup("Posting");
tblTop.AddTextBlob("Remarks", Flags: FieldFlags.None).SetNullable(true).SetLargeMemo().SetGroup("Notes");
```

## Joins

Foreign keys and lookup/locator metadata can generate joins.

Example:

```csharp
TableDef tblSupplier = tblTop.AddJoin("SupplierId", "ProductSupplier", "Supplier", "Id");
tblSupplier.AddString("SupplierCode", MaxLength: 96, Flags: FieldFlags.None).SetNullable(true).SetTitleKey("Supplier Product Code");
```

Join fields become non-persistent display fields in the module descriptor.

They are also used by list select aliases and filters.

## Details And Subdetails

Tables connected with `Master` metadata become detail tables.

Example generated shape:

```csharp
TableDef tblAssetDepreciationLine = tblTop.AddDetail("AssetDepreciationLine", "Id", "AssetId");
tblAssetDepreciationLine.AddString("AssetId", MaxLength: 40, Flags: FieldFlags.Required).SetNullable(false);
tblAssetDepreciationLine.AddDate("DepreciationDate", Flags: FieldFlags.Required).SetNullable(false);
```

Nested detail tables become subdetail descriptors under their parent table.

`DetailOrder` metadata can control the display order of direct child details.

Example:

```csharp
Module.DetailOrder["Payment"] = ["PaymentSettlement"];
```

## Filters

Generated modules configure filters on the list `SelectDef`.

Example:

```csharp
SelectDef = Module.SelectList[0];
SelectDef.AddFilter("Name", FieldName: "Name", FilterDataType: DataFieldType.String);
SelectDef.AddFilter("NumberSeries__Code", FieldName: "NumberSeries__Code", FilterDataType: DataFieldType.String);
```

If `FilterFields` is declared in metadata, only those final list columns are used, in the declared order.

Without `FilterFields`, automatic filter generation is used.

## Column Types

The generated list `SelectDef` stores column type information.

Example:

```csharp
SelectDef.ColumnTypes["Code"] = DataColumnType.Text;
SelectDef.ColumnTypes["IsActive"] = DataColumnType.Boolean;
```

This helps the UI and filtering system understand result columns.

## Generated Baseline

Generated modules are a baseline descriptor.

They should not contain business behavior.

Custom behavior belongs in:

- data module classes
- services
- form classes
- handwritten registry update methods

If the generated module structure is wrong, change the schema metadata and regenerate.

Do not edit `RegistryVersionN.Modules.cs` by hand.
