# Registration Best Practices

Manual registration is ordinary C# code, but it should follow stable conventions.

The goal is not only to make the application work.

The goal is to make the declaration readable, predictable and close to what the Registration Builder would generate.

## Keep Schema And Descriptors Separate

Keep database schema registration separate from descriptor registration.

Schema registration creates or upgrades database objects.

Descriptor registration tells Tripous how to work with those objects.

The normal startup order is:

- Register schemas.
- Execute schemas.
- Create the SQL store.
- Register descriptors.
- Resolve descriptor references.
- Register application commands.

Do not register descriptors before the database schema exists.

## Use A Stable Descriptor Order

Use the same order everywhere.

Recommended order:

- Lookups.
- Lookup sources.
- Locators.
- Code providers.
- Modules.
- Forms.
- Configuration properties.
- Reference resolution.

This keeps name-based references predictable.

Fields can reference lookup names, locator names and code provider names because those descriptors already exist.

Forms can reference modules because modules already exist.

## Group By Registry Version

Group manual declarations in registry version classes.

This is an application convention, not a Tripous core requirement.

```csharp
public class RegistryVersion1 : RegistryVersion
{
    public override void RegisterLookupSources()
    {
    }

    public override void RegisterLocators()
    {
    }

    public override void RegisterCodeProviders()
    {
    }

    public override void RegisterModules()
    {
    }

    public override void RegisterForms()
    {
    }
}
```

This keeps each version inspectable.

It also keeps manual registration close to generated registration.

## Keep Names Stable

Names are the main links between descriptors.

Use names consistently.

Examples:

- Module name: `Customer`.
- Table name: `Customer`.
- Form name: `Customer`.
- Locator name: `Customer`.
- Code provider name: `Customer`.

Different names are allowed, but they should express a real difference.

Do not invent alternate names without a reason.

## Keep Primary Keys And Business Codes Separate

Use `Id` as the primary key.

Use `Code` as a business value.

When a `Code` field is generated, connect it to a code provider.

```csharp
Table.AddString("Code", 40, Flags: FieldFlags.Required | FieldFlags.Searchable | FieldFlags.ReadOnlyEdit | FieldFlags.ReadOnlyUI).SetCodeProviderName("Customer");
```

Do not use a generated business code as the primary key.

## Prefer Clear List SQL

A module list select should be explicit.

Avoid relying on `select *` for real modules.

Good list SQL:

- Includes the key field.
- Uses clear aliases.
- Joins lookup and reference display fields.
- Orders rows intentionally.
- Keeps calculated columns obvious.

The list SQL is a projection.

The table definition is still the editable structure.

## Match Filters To SQL

Filter names should match visible list columns when possible.

`FieldName` should point to the real SQL field or expression.

```csharp
SelectDef.AddFilter("Customer", FieldName: "c.Name", FilterDataType: DataFieldType.String, ConditionOp: ConditionOp.Contains);
```

This matters when the visible column is an alias.

Use:

- `Contains` for text search.
- `Between` for dates and date-times.
- Boolean filters only for integer-backed boolean fields.

## Use Joins For Display Fields

A reference field stores the selected key.

A join exposes related display fields.

```csharp
FieldDef Field = Table.AddString("CustomerId", 40, TitleKey: "Customer", Flags: FieldFlags.Required);
Field.Locator = "Customer";

TableDef CustomerJoin = Table.AddJoin("CustomerId", "Customer", "Customer", "Customer", "Id");
CustomerJoin.AddString("Code", 40);
CustomerJoin.AddString("Name", 128);
```

Use the join alias convention consistently.

Examples:

- `Customer__Code`.
- `Customer__Name`.
- `Product__Code`.
- `Product__Name`.

## Use Snapshots When Values Must Be Stored

A join display field is not the same as a stored snapshot.

Use snapshots when the target table must keep the selected value.

```csharp
Table.AddString("ProductCode", 40).SetSnapshotOf("Product.Code");
Table.AddString("ProductName", 128).SetSnapshotOf("Product.Name");
```

This is useful for document lines, where product name or price-related display values may need to remain as they were at selection time.

## Keep Locators Predictable

A locator should:

- Return the key field.
- Return at least one visible searchable field.
- Map the key to the target reference field.
- Use aliases that match join aliases.
- Use `TargetField` when the target field is not implied by aliases or snapshots.

For example:

```csharp
LocatorDef Locator = DataRegistry.AddLocator("Customer", "Customer", "Id", FormName: "Customer");
Locator.Add("Id", DataFieldType.String, TargetField: "CustomerId", Alias: "Customer__Id", TitleKey: null, IsVisible: false, IsSearchable: false);
Locator.Add("Code", DataFieldType.String, TargetField: null, Alias: "Customer__Code", TitleKey: null, IsVisible: true, IsSearchable: true);
Locator.Add("Name", DataFieldType.String, TargetField: null, Alias: "Customer__Name", TitleKey: null, IsVisible: true, IsSearchable: true);
```

## Keep Lookup Usage Small

Use lookups for small stable value lists.

Use locators for searchable entity tables.

Good lookup candidates:

- Status.
- Type.
- Category.
- Enum values.

Good locator candidates:

- Customer.
- Product.
- Contact.
- Document.

Do not turn large entity tables into simple lookup lists unless the UI really needs a small dropdown.

## Use Field Flags Deliberately

Field flags communicate behavior to the runtime and presentation layers.

Typical rules:

- Use `Required` for values that must exist.
- Use `Searchable` for useful search fields.
- Use `ReadOnlyUI` for fields users should not edit.
- Use `ReadOnlyEdit` for fields that should not change after insert.
- Use memo flags only for long text fields.

Do not add flags just because another field has them.

Each flag should describe real behavior.

## Keep Manual And Generated Registration Equivalent

Manual registration and generated registration should describe the same model.

The Registration Builder is not a separate architecture.

It writes the declarations that can also be written by hand.

Use this as a quality check:

- Could this manual declaration be generated from schema metadata?
- Are names, aliases and provider names consistent?
- Are joins and snapshots explicit?
- Are filters connected to real SQL fields?
- Are forms pointing to real modules?

If the answer is no, the manual declaration may be too clever or too implicit.

## Practical Checklist

Before considering a manual registration complete, check:

- Schema exists before descriptors are registered.
- Descriptor order is stable.
- References are resolved after registration.
- Names are consistent across modules, forms, lookups, locators and code providers.
- Primary keys are separate from business codes.
- Code provider fields have matching registered providers and number series rows.
- List SQL includes the key field.
- Filters use correct `FieldName` values.
- Locator aliases match join aliases.
- Snapshot fields are marked with `SetSnapshotOf()`.
- Field flags describe actual behavior.
- Manual declarations remain close to generated declarations.

## Related Articles

- [Overview](overview.md)
- [Declaration Order](declaration-order.md)
- [Application Host](application-host.md)
- [Modules](modules.md)
- [Tables and Fields](tables-and-fields.md)
- [Forms](forms.md)
- [Lookups](lookups.md)
- [Locators](locators.md)
- [Select Definitions](select-definitions.md)
- [Code Providers](code-providers.md)
