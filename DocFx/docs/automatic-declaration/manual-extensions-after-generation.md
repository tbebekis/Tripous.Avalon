# Manual Extensions After Generation

Automatic registration is the starting point, not the end of application registration.

The Registration Builder writes the declarations that can be inferred safely from `Schema.sql` files and metadata comments.

After those generated declarations have been registered, the application may execute handwritten code that modifies, replaces or extends them.

This is an important part of the Tripous model:

- generated code provides the first complete registration pass
- handwritten code performs the application-specific final pass
- generated files remain disposable and can be recreated at any time
- handwritten extension files remain under developer control

The practical rule is simple:

- do not edit generated registry files manually
- add extension code after generated registration has completed

## Registration Order

A typical generated registry version registers the descriptors first.

The application registry coordinator then runs handwritten extension methods.

Example from tERP:

```csharp
static public void RegisterDescriptors()
{
    foreach (RegistryVersion Version in RegistryVersionList)
    {
        Version.RegisterLookups();
        Version.RegisterLookupSources();
        Version.RegisterLocators();
        Version.RegisterCodeProviders();
        Version.RegisterModules();
        Version.RegisterForms();
    }

    RegisterDocumentHandlers();

    UpdateLookups();
    UpdateLocators();
    UpdateForms();
    UpdateModules();

    RegisterSycConfigProperties();
}
```

This order is intentional.

The generated registration creates the baseline descriptors.

The handwritten registration then gets full access to those descriptors and can alter almost anything they contain.

## What Can Be Extended

Handwritten extension code may modify or add:

- lookups
- lookup sources
- locators
- locator fields
- modules
- tables and fields
- forms
- select definitions
- code providers
- document handlers
- configuration properties
- custom data module classes
- custom form classes
- custom locator classes

This does not create a second registration system.

It uses the same registries and the same descriptor types as manual registration.

## Why Extensions Are Needed

Metadata comments are intentionally compact.

They describe the normal case well, but some applications need behavior that is too specific for schema comments.

Examples:

- a generated locator must be replaced by a SQL-based locator
- a field must use a role-specific locator
- a form must use a custom class
- a document module must be connected to a document handler
- a configuration property must apply runtime behavior
- a lookup must display a field different from the generated default

These cases belong in handwritten extension code.

## Updating Lookups

Generated lookup definitions may need small corrections.

In tERP, the application user lookup is generated from the system user table, but the display field is changed manually.

```csharp
static public void UpdateLookups()
{
    LookupDef LookupDef = DataRegistry.Lookups.Find(DbConfig.SysAppUserTableName);
    if (LookupDef != null)
        LookupDef.DisplayField = "FullName";
}
```

This is a small extension, but it shows the pattern:

- find the generated descriptor
- modify the required property
- leave the generated file unchanged

## Replacing Or Extending Locators

Locators are often extended manually because application lookup behavior can be domain-specific.

In tERP, the `Person` locator is replaced with a SQL locator.

```csharp
string SqlText = @"
select
     P.Id
    ,P.Code
    ,P.Name
from Person P
where
        P.IsActive = 1
";

LocatorDef LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Person", SqlText, "Id", FormName: "Person");
```

The same base idea is then reused for role-specific person locators such as `Customer`, `Supplier`, `Employee`, `Manager` and `Carrier`.

```csharp
string WhereSql = @"  and PRT.Code = 'CUS' ";
LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Customer", SqlText + WhereSql, "Id", FormName: "Person");
```

This is not a RegBuilder limitation.

It is a deliberate split:

- the schema declares the tables and ordinary metadata
- handwritten code expresses business-specific locator logic

## Custom Locator Fields

After a locator is created or replaced, its visible and searchable fields may be customized.

tERP clears and rebuilds locator fields when it needs exact control.

```csharp
void AddFields(LocatorDef LocatorDef, string[] FieldNames)
{
    LocatorDef.Fields.Clear();

    foreach (string FieldName in FieldNames)
    {
        LocatorFieldDef FieldDef = LocatorDef.Add(FieldName);
        if (FieldName.IsSameText("Id"))
        {
            FieldDef.IsVisible = false;
            FieldDef.IsSearchable = false;
        }
    }
}
```

The product locator is a good example.

It returns product identity fields and extra values needed by document lines.

```csharp
LocatorDef = DataRegistry.AddOrUpdateLocatorWithSql("Product", SqlText, "Id", FormName: "Product");
AddFields(LocatorDef, [
    "Id",
    "Code",
    "Name",
    "UnitOfMeasureId",
    "UnitOfMeasureName",
    "UnitRatio",
    "TaxProductGroupId"
]);

LocatorDef.Fields.Find("UnitOfMeasureId").IsVisible = false;
```

The locator may return more data than the user sees.

Those values can still be mapped to target fields when a locator result is selected.

## Custom Locator Classes

Some locator behavior requires a custom class.

tERP registers a custom locator class for payment settlement finance movements.

```csharp
LocatorDef = DataRegistry.AddOrUpdateLocator(
    "PaymentSettlementFinanceMovement",
    "FinanceMovement",
    "Id",
    ClassName: typeof(PaymentSettlementFinanceMovementLocator).FullName,
    FormName: "FinanceMovement");
```

The descriptor still lives in `DataRegistry`.

Only its runtime implementation is specialized.

## Updating Forms

Generated form descriptors may be assigned to custom form classes.

Example:

```csharp
static public void UpdateForms()
{
    DesktopRegistry.Forms.Get("SalesDeliveryNote").ClassName = "SalesDeliveryNoteForm";
}
```

The generated descriptor remains the base declaration.

The custom class supplies the behavior the application needs.

## Updating Modules

Modules can be corrected or enriched after generation.

tERP uses handwritten code to assign role-specific locators to trade modules.

```csharp
void SetTradeModulePersonLocator(string Locator, string[] ModuleNames)
{
    foreach (string ModuleName in ModuleNames)
    {
        ModuleDef ModuleDef = DataRegistry.Modules.Find(ModuleName);
        if (ModuleDef != null && ModuleDef.Table.Fields.Contains("PersonId"))
        {
            ModuleDef.Table.Fields["PersonId"].Locator = Locator;
            ModuleDef.Table.Fields["PersonId"].TitleKey = Locator;
        }
    }
}

SetTradeModulePersonLocator("Customer", SalesDocumentModules);
SetTradeModulePersonLocator("Supplier", PurchaseDocumentModules);
```

This keeps the generated module declarations simple while allowing the final application model to be precise.

## Registering Document Handlers

Document handlers are pure application behavior.

They are registered manually after generated module registration.

Example from tERP:

```csharp
static public void RegisterDocumentHandlers()
{
    DataRegistry.AddOrUpdateDocumentHandler("SalesOrder", typeof(SalesOrderDocumentHandler).FullName);
    DataRegistry.AddOrUpdateDocumentHandler("SalesInvoice", typeof(SalesInvoiceDocumentHandler).FullName);
    DataRegistry.AddOrUpdateDocumentHandler("PurchaseInvoice", typeof(PurchaseInvoiceDocumentHandler).FullName);
    DataRegistry.AddOrUpdateDocumentHandler("StockTrade", typeof(StockTradeDocumentHandler).FullName);
    DataRegistry.AddOrUpdateDocumentHandler("JournalEntry", typeof(JournalEntryDocumentHandler).FullName);
}
```

The RegBuilder can declare that a module exists.

The application decides which handler implements its business behavior.

## Configuration Properties

Application configuration properties may also be added manually.

tERP registers properties such as application defaults, data form logging and user support.

```csharp
string Name = DataLib.SAppDefaultProperties;
string TitleKey = "Application Defaults";
string GroupName = "Application";
UserLevel SecurityLevel = UserLevel.Admin;
ConfigValueKind Kind = ConfigValueKind.Object;
string DefaultValue = Json.Serialize(new AppDefaultProperties());
string TypeName = typeof(AppDefaultProperties).FullName;
string EditorClassName = "tERP.AppDefaultPropertiesEditor";

DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue, TypeName, EditorClassName);
```

These declarations are not database schema metadata.

They are application-level settings and therefore belong naturally in handwritten registration.

## Custom Classes Referenced By Generated Code

Generated declarations may reference custom class names.

For example, module and form metadata may name:

- a custom `DataModule` class
- a custom form class
- a custom item page class
- a custom locator class

The generated code only registers the class name.

The actual class is handwritten by the application.

This is another extension point: metadata connects the generated descriptor to application code, while the application owns the implementation.

## Safe Extension Rules

- Keep generated files disposable.
- Put handwritten extensions in separate partial files.
- Run extension code after all generated registry versions have registered.
- Prefer `AddOrUpdate` methods when replacing generated descriptors.
- Prefer `Find()` when an extension is optional.
- Prefer `Get()` when a missing descriptor is a programming error.
- Keep extension methods idempotent where practical.
- Keep schema-level facts in `Schema.sql`.
- Keep behavior and special cases in handwritten extension code.
- Review generated code after metadata changes, but do not edit it manually.

## Summary

Manual extensions after generation are the bridge between automatic registration and real application behavior.

The Registration Builder writes the code that can be derived from schema metadata.

The developer then has full control to adjust, replace and extend the resulting descriptors using normal Tripous registration APIs.
