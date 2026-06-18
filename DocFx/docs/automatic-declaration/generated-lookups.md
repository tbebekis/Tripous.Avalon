# Generated Lookups

Generated lookups are written to `RegistryVersionN.Lookups.cs`.

They register lookup sources in `DataRegistry`.

Lookups are intended for small reference sets that can be shown as simple selectors.

## Generated Method

The generated file overrides `RegisterLookups()`.

Example:

```csharp
public override void RegisterLookups()
{
    DataRegistry.AddOrUpdateLookupWithTableName("Currency", "Currency", FormName: "Currency");
    DataRegistry.AddOrUpdateLookupWithTableName("DocumentType", "DocumentType", FormName: "DocumentType");
    DataRegistry.AddOrUpdateLookupWithClassName("ModuleName", "DocumentModuleLookupSource");
}
```

Each call registers a lookup source by name.

## Lookup Sources

The Registration Builder can generate three lookup source kinds:

- table-backed lookup sources
- enum-backed lookup sources
- class-backed lookup sources

The generated method depends on metadata.

## Table-Backed Lookups

Table-backed lookups are generated with:

```csharp
DataRegistry.AddOrUpdateLookupWithTableName("Currency", "Currency", FormName: "Currency");
```

The first argument is the lookup source name.

The second argument is the table name.

`FormName` is added when the lookup table has a form.

Table-backed lookups can come from:

- tables marked with `IsLookup`
- tables detected by lookup shape heuristic
- fields declaring `-- Lookup`
- fields declaring `-- Lookup LOOKUP_NAME TableName:TABLE_NAME`

## Lookup Table Detection

A table is treated as a lookup table when:

- the header declares `IsLookup`, or
- the table is a top table whose native fields match a known lookup shape

Known lookup shapes are:

```text
Id, Name
Id, Code, Name
Id, Name, IsActive
Id, Code, Name, IsActive
```

Explicit `IsLookup` overrides the heuristic.

## Field Lookup Metadata

A field can request lookup behavior.

Examples:

```sql
CurrencyId @NVARCHAR(40) @NOT_NULL, -- Lookup
CreatedBy @NVARCHAR(40) @NOT_NULL, -- Lookup SYS_APP_USER
PersonId @NVARCHAR(40) @NOT_NULL, -- Lookup Customer TableName:Person
```

If the lookup name is omitted, the builder resolves it from the foreign key referenced table or from the field name without the `Id` suffix.

If `TableName:` is used, the lookup name is required.

## Enum-Backed Lookups

Enum-backed lookups are generated with:

```csharp
DataRegistry.AddOrUpdateLookupSource("TradeType", TypeStore.Get("TradeType"));
```

Schema metadata may use:

```sql
TradeTypeId int @NOT_NULL, -- Lookup TradeType EnumName:TradeType
```

The parser also accepts `EnumType:` as an alias for `EnumName:`.

Enum-backed lookup sources need the enum type to be discoverable by `TypeStore`.

That is why `RegBuilderConsole` can build and load configured assemblies before generation.

## Class-Backed Lookups

Class-backed lookups are generated with:

```csharp
DataRegistry.AddOrUpdateLookupWithClassName("ModuleName", "DocumentModuleLookupSource");
```

Schema metadata:

```sql
ModuleName @NVARCHAR(96) @NOT_NULL, -- Lookup ModuleName ClassName:DocumentModuleLookupSource
```

Use class-backed lookup sources when lookup values are not simply read from one table or enum.

The class implementation is handwritten application code.

## Registering Lookup Fields

Lookup source registration and field registration are related but separate.

`RegistryVersionN.Lookups.cs` registers the lookup source.

`RegistryVersionN.Modules.cs` registers fields that use lookup sources.

Example field descriptor:

```csharp
tblTop.AddStringLookupId("CurrencyId", "Currency", Flags: FieldFlags.Required).SetNullable(false);
```

The field points to the lookup source by name.

## Generated Baseline

Generated lookup registrations are structural declarations.

They do not define business rules.

If a generated lookup is wrong, change the schema metadata and regenerate.

Do not edit `RegistryVersionN.Lookups.cs` manually.

Use handwritten registry update methods only for deliberate application-specific adjustments.
