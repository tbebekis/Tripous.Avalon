# Generated Forms

Generated forms are written to `RegistryVersionN.Forms.cs`.

They register form descriptors in `DesktopRegistry`.

The generated code maps modules to UI forms, form classes, groups, item page classes and read-only flags.

## Generated Method

The generated file overrides `RegisterForms()`.

Example:

```csharp
public override void RegisterForms()
{
    DesktopRegistry.AddOrUpdateForm("DocumentType", TitleKey: "DocumentType", Module: "DocumentType", Group: "Documents");
    DesktopRegistry.AddOrUpdateForm("SalesInvoice", TitleKey: "SalesInvoice", Module: "SalesInvoice", ClassName: "SalesInvoiceForm", Group: "Sales", ItemClassName: "TradeItemPage");
    DesktopRegistry.AddOrUpdateForm("FinanceMovement", TitleKey: "FinanceMovement", Module: "FinanceMovement", Group: "Finance", IsReadOnly: true);
}
```

Each call registers or updates a `FormDef`.

## Source Metadata

Generated form declarations come mainly from top table module metadata.

Example:

```sql
Module: SalesInvoice SalesInvoiceDataModule
Group: Sales
Form: SalesInvoice SalesInvoiceForm
ItemPage: TradeItemPage
```

This generates a form registration like:

```csharp
DesktopRegistry.AddOrUpdateForm("SalesInvoice", TitleKey: "SalesInvoice", Module: "SalesInvoice", ClassName: "SalesInvoiceForm", Group: "Sales", ItemClassName: "TradeItemPage");
```

## Form Name

The form name is the key in `DesktopRegistry`.

It is also the first argument of `AddOrUpdateForm()`.

If `Form` is omitted in metadata, the form name defaults to the module name.

Example:

```sql
Module: DocumentType DocumentTypeDataModule
Group: Documents
```

Generated form:

```csharp
DesktopRegistry.AddOrUpdateForm("DocumentType", TitleKey: "DocumentType", Module: "DocumentType", Group: "Documents");
```

## Form Class

`ClassName` points to a custom form class.

Example:

```sql
Form: SalesInvoice SalesInvoiceForm
```

Generated argument:

```csharp
ClassName: "SalesInvoiceForm"
```

If no custom form class is declared, the default `DataForm` is used by the desktop layer.

The generated registration only names the class.

The class implementation is handwritten application code.

## Module And Group

`Module` links the form to the corresponding `ModuleDef`.

`Group` controls navigation grouping.

Example:

```csharp
DesktopRegistry.AddOrUpdateForm("StockTrade", TitleKey: "StockTrade", Module: "StockTrade", ClassName: "StockTradeForm", Group: "Inventory");
```

The form descriptor does not create the module.

It points to an already registered module.

## Item Page Class

`ItemPage` metadata generates `ItemClassName`.

Example:

```sql
ItemPage: TradeItemPage
```

Generated argument:

```csharp
ItemClassName: "TradeItemPage"
```

At runtime, `DataForm` creates the item page from `FormDef.ItemClassName`.

This is how a module can use a custom item layout while still keeping generated field and table descriptors.

## Read-Only Forms

The table header flag `IsReadOnly` generates a read-only form declaration.

Example generated form:

```csharp
DesktopRegistry.AddOrUpdateForm("FinanceMovement", TitleKey: "FinanceMovement", Module: "FinanceMovement", Group: "Finance", IsReadOnly: true);
```

At runtime, `DataForm.IsReadOnlyForm` reads `FormDef.IsReadOnly`.

Read-only forms are used for modules where insert, edit and delete should not be available through the form.

## Declaration Only

Generated forms are declarations.

They do not generate form behavior.

Custom behavior belongs in:

- custom form classes
- custom item page classes
- data modules
- services
- handwritten registry update methods

## Manual Edits

Do not edit `RegistryVersionN.Forms.cs` manually.

When a generated form registration needs to change:

- change `Form` metadata
- change `ItemPage` metadata
- change table-level flags such as `IsReadOnly`
- run the Registration Builder again
- review the generated diff
