# TinyERP

TinyERP is the largest Tripous sample application.

It is an educational ERP-style desktop application built on Tripous, Tripous.Data and Tripous.Desktop.

The sample demonstrates how a larger application can be declared from SQL schema metadata and then extended with handwritten business logic.

TinyERP is the main sample for automatic application declaration with the Registration Builder.

## Disclaimer

- TinyERP is an educational Tripous sample.
- It is not a production ERP.
- It is not audited or certified for accounting, tax, legal, stock, finance or operational use.
- Do not use it as a real business system.
- Do not use it to store real company data.
- The implementation intentionally favors framework demonstration, readability and testability over production hardening.
- A production application based on this sample requires engineering review, security review, accounting review, tax review, operational review and legal/business validation.

## Purpose

TinyERP demonstrates:

- A larger Tripous application split into multiple projects.
- Automatic registration using the Registration Builder.
- SQL schema files with metadata comments.
- Generated registry code for modules, forms, lookups, locators, select definitions and code providers.
- Manual extensions after generation.
- Document-oriented business modules.
- Master/detail and subdetail data forms.
- Lookup fields.
- Locator fields.
- Snapshot fields.
- Generated business codes.
- Draft and final document numbering.
- Posting, cancellation and transformation workflows.
- Stock movements and balances.
- Finance movements and balances.
- Accounting journal entries.
- Customer receipts and supplier payments.
- Unit tests and UI-oriented tests for business workflows.

## Project Structure

TinyERP is a multi-project sample.

```text
SampleApps/TinyERP
    tERP
    tERP.Common
    tERP.Data
    tERP.Desktop
    tERP.Tests
    Docs
```

The projects have separate responsibilities.

- `tERP` is the executable Avalonia desktop application.
- `tERP.Common` contains shared enums, small contracts and shared value objects.
- `tERP.Data` contains schema files, generated registry code, data modules, document handlers, sample data and business services.
- `tERP.Desktop` contains desktop-specific forms and UI extensions.
- `tERP.Tests` contains automated tests for database and business workflows.
- `Docs` contains working notes and future documentation material.

## Application Project

The `tERP` project is the desktop application entry point.

Important files and folders:

- `Program.cs`
- `App.axaml`
- `App.axaml.cs`
- `HiddenMainWindow.cs`
- `MainWindow.axaml`
- `MainWindow.axaml.cs`
- `DashboardForm.axaml`
- `ReadOnlyViewForm.axaml`
- `AppHost`
- `tester-guide.txt`
- `tester-guide-gr.txt`

The application follows the same startup pattern as the smaller samples.

- A hidden startup window is created first.
- Early dialogs have a valid owner window.
- The database and descriptors are initialized.
- The real main window is shown only after startup is complete.
- `AppHost` owns the startup flow, commands, views and UI initialization.

## Common Project

The `tERP.Common` project contains shared types used by data, desktop and tests.

It includes:

- Business enums.
- Price resolver contracts.
- Tax resolver contracts.
- Address value objects.
- Tax result and tax component objects.
- Shared price result objects.

Examples:

- `TradeType`
- `TradeStatus`
- `StockTradeOperation`
- `NormalBalance`
- `AccountType`
- `TaxType`
- `TaxTreatment`
- `ProductType`
- `IPriceResolver`
- `ITaxResolver`
- `PersonAddress`

This project keeps domain-neutral contracts and enums outside the data and desktop projects.

## Data Project

The `tERP.Data` project is the largest part of the sample.

It contains:

- SQL schema files.
- Generated registry files.
- Data modules.
- Document handlers.
- Price and tax resolvers.
- Sample data.
- Data defaults.
- Application default properties.
- Database log listener.

Important files and folders:

- `Schema01.sql`
- `Schema02.sql`
- `Registry`
- `DataModules`
- `DocumentHandlers`
- `SampleData`
- `PriceResolver.cs`
- `TaxResolver.cs`
- `SalesDefaults.cs`
- `PurchaseDefaults.cs`
- `AppDefaultProperties.cs`

## Desktop Project

The `tERP.Desktop` project contains desktop-specific extensions.

It depends on `Tripous.Desktop` and the tERP data projects.

It contains:

- Base application data form.
- Custom document forms.
- Payment forms.
- Journal entry form.
- Stock document forms.
- Trade item page customization.
- User password dialog.

Important files and folders:

- `DataForms`
- `TradeItemPage.cs`
- `DesktopLib.cs`

The generated form descriptors point to these forms when a document needs custom UI behavior.

## Tests Project

The `tERP.Tests` project contains automated tests.

It includes:

- Database smoke tests.
- Sales document tests.
- Purchase document tests.
- Stock count tests.
- Stock trade tests.
- Journal entry tests.
- Shared test database fixture.

Important files:

- `database-smoke-tests.cs`
- `sales-document-tests.cs`
- `purchase-document-tests.cs`
- `stock-count-tests.cs`
- `stock-trade-tests.cs`
- `journal-entry-tests.cs`
- `test-database-fixture.cs`

The tests exercise core business workflows without relying only on UI behavior.

## Database

The current SQLite database path used during development is:

```text
/home/teo/.config/tERP/Data/tERP.db3
```

TinyERP uses Tripous database tokens and provider-neutral schema SQL.

Examples:

- `@NVARCHAR(40)`
- `@NBLOB_TEXT`
- `@BOOL`
- `@DATE`
- `@DATE_TIME`
- `@NOT_NULL`
- `@NULL`

Tripous providers translate these tokens to the target RDBMS SQL dialect.

The current sample is primarily exercised with SQLite.

## Schema Files

TinyERP uses schema files as the source for automatic registration.

Current schema files:

- `tERP.Data/Schema01.sql`
- `tERP.Data/Schema02.sql`

Schema files contain:

- `CREATE TABLE` statements.
- Foreign keys.
- Unique constraints.
- RDBMS-neutral SQL tokens.
- Metadata comments for modules, forms, groups, lookups, locators, filters and code providers.

Schema metadata comments are used by the Registration Builder.

The schema file is not only a database script.

It is also the main declaration source for generated Tripous descriptors.

## Schema Version Rule

TinyERP currently changes the existing schema files and rebuilds the database.

Do not create a new schema file such as `SchemaVersion3` unless explicitly requested.

Current rule:

- Make schema changes in the existing schema file.
- Regenerate registry files with the Registration Builder.
- Recreate the database for the sample.

This keeps the sample simple while the schema is still actively evolving.

## Registration Builder

TinyERP is the main sample that uses the Registration Builder.

The Registration Builder reads schema files and metadata comments.

It generates C# registry files that contain the same descriptor declarations a developer could write by hand.

Generated registry files include:

- `RegistryVersionN.Modules.cs`
- `RegistryVersionN.Forms.cs`
- `RegistryVersionN.Lookups.cs`
- `RegistryVersionN.Locators.cs`
- `RegistryVersionN.CodeProviders.cs`
- `SchemaVersionN.cs`

Generated files live under:

```text
tERP.Data/Registry
```

## RegBuilder Workflow

Use the RegBuilder console tool to regenerate registry files.

Useful commands from the repository root:

```text
dotnet run --project Tools/RegBuilderConsole -- --project tERP.Version2
dotnet run --project Tools/RegBuilderConsole -- --project tERP.Version2 --no-build
```

The console tool:

- Builds configured projects unless `--no-build` is used.
- Loads configured assemblies.
- Runs the Registration Builder.
- Copies generated `.cs` files to `SampleApps/TinyERP/tERP.Data/Registry`.

It does not copy `Schema.sql` to the destination registry folder.

## Generated Files Rule

Do not manually edit generated registry files.

Generated files contain the warning:

```text
<auto-generated>
This file was generated by Tripous RegBuilder.
Do not edit this file manually.
</auto-generated>
```

When generated output needs to change:

- Change schema metadata.
- Run the Registration Builder.
- Review the generated diff.
- Keep handwritten logic in non-generated files.

## Registry Startup

TinyERP registration is coordinated by `tERP.Data/Registry/Registry.cs`.

Schema registration:

```csharp
static public void RegisterSchemas()
{
    foreach (SchemaVersionDef Version in SchemaVersionList)
        Version.Register();
}
```

Descriptor registration:

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

The order is important.

- Lookups and lookup sources are registered before fields reference them.
- Locators are registered before fields reference them.
- Code providers are registered before generated code fields use them.
- Modules are registered before forms reference them.
- References are resolved after all descriptor names exist.

## Manual Extensions

Generated registration is only part of the application.

TinyERP also contains handwritten code for:

- Data modules.
- Document handlers.
- Price resolution.
- Tax resolution.
- Sample data.
- UI forms.
- UI item pages.
- Tests.
- Application commands.
- Dashboard and read-only views.

This is the intended Tripous pattern.

The Registration Builder writes the repetitive declaration code.

The developer writes the business behavior and UI-specific extensions.

## Main Business Areas

TinyERP includes many ERP-style areas.

Setup and master data:

- Companies.
- Persons.
- Customers and suppliers through person roles.
- Products.
- Product categories and groups.
- Warehouses.
- Units of measure.
- Currencies.
- Payment methods.
- Payment terms.
- Price lists.
- Tax groups, tax rates, tax rules and tax jurisdictions.
- Projects.
- Fixed assets.
- Cash and bank accounts.
- Chart of accounts.

Documents:

- Sales orders.
- Sales delivery notes.
- Sales invoices.
- Sales credit notes.
- Sales returns.
- Sales cancellations.
- Purchase orders.
- Purchase delivery notes.
- Purchase invoices.
- Purchase credit notes.
- Purchase returns.
- Purchase cancellations.
- Stock counts.
- Stock trades.
- Journal entries.
- Customer receipts.
- Customer receipt cancellations.
- Supplier payments.
- Supplier payment cancellations.

Read-only or system views:

- Stock movements.
- Stock balances.
- Stock reservations.
- Finance movements.
- Finance balances.
- Logs.
- System configuration.
- Number series.

## Document Types

The `DocumentType` table defines document behavior.

It controls:

- Module name.
- Number series.
- Stock effect.
- Financial effect.
- Accounting effect.
- Cancellation behavior.
- Display order.
- Appearance metadata.
- Output metadata.

Examples:

- Sales Invoice.
- Purchase Invoice.
- Sales Credit Note.
- Purchase Credit Note.
- Stock Trade.
- Customer Receipt.
- Supplier Payment.

Document type metadata lets generic document code handle multiple document modules while still allowing each module to keep specific behavior.

## Number Series And Code Providers

TinyERP uses `SYS_NUMBER_SERIES` and Tripous code providers.

Generated code fields use `SetCodeProviderName()`.

Documents often use draft and final numbering.

Examples:

- `DRAFT-SalesInvoice`
- `SalesInvoice`
- `DRAFT-PurchaseInvoice`
- `PurchaseInvoice`

During normal editing, a draft provider can assign a draft code.

During posting, the document module can replace it with a final code from the final provider.

Final code assignment happens inside the same transaction that saves the posted document.

## Data Module Hierarchy

Document behavior is implemented in data modules.

The main hierarchy is:

```text
DataModule
    AppDataModule
        DocumentDataModule
            JournalEntryDataModule
            PaymentDataModule
            StockCountDataModule
            StockTradeDataModule
            TradeDataModule
                SalesDataModule
                    SalesOrderDataModule
                    SalesStockDataModule
                        SalesDeliveryNoteDataModule
                        SalesReturnDataModule
                    SalesInvoiceDataModule
                    SalesCreditNoteDataModule
                    SalesCancellationDataModule
                PurchaseDataModule
                    PurchaseOrderDataModule
                    PurchaseStockDataModule
                        PurchaseDeliveryNoteDataModule
                        PurchaseReturnDataModule
                    PurchaseInvoiceDataModule
                    PurchaseCreditNoteDataModule
                    PurchaseCancellationDataModule
```

`DocumentDataModule` provides common document infrastructure.

`TradeDataModule` adds commercial calculations, pricing, taxes, discounts and totals.

Sales and purchase modules add defaults and validations for their document families.

Stock modules add stock-specific posting behavior.

Payment modules add finance and settlement behavior.

Journal entry modules add accounting validation and posting behavior.

## Document Handlers

Document handlers contain posting, cancellation and transformation behavior.

The handler hierarchy is:

```text
DocumentHandler
    JournalEntryDocumentHandler
    PaymentDocumentHandler
    StockCountDocumentHandler
    StockTradeDocumentHandler
    TradeDocumentHandler
        SalesDocumentHandler
            SalesOrderDocumentHandler
            SalesDeliveryNoteDocumentHandler
            SalesInvoiceDocumentHandler
            SalesCreditNoteDocumentHandler
            SalesReturnDocumentHandler
            SalesCancellationDocumentHandler
        PurchaseDocumentHandler
            PurchaseOrderDocumentHandler
            PurchaseDeliveryNoteDocumentHandler
            PurchaseInvoiceDocumentHandler
            PurchaseCreditNoteDocumentHandler
            PurchaseReturnDocumentHandler
            PurchaseCancellationDocumentHandler
```

The base handlers contain shared behavior.

Concrete handlers provide extension points for document-specific rules.

## Posting

Posting turns a draft document into a posted document.

The common posting flow:

- Creates a document context.
- Enables posting mode.
- Validates the document.
- Runs the document handler.
- Saves changes in a transaction.
- Assigns final document number when required.
- Locks the document.
- Writes posting metadata.
- Restores previous values if posting fails.

Posting rules generally require:

- The document is draft.
- The document is unlocked.
- The document is not cancelled.
- Required lines and totals are valid.
- Business-specific constraints pass.

## Cancellations

TinyERP uses explicit cancellation documents.

Cancellation documents do not simply delete or mutate the original document.

They represent a new business document that cancels the source document.

Examples:

- Sales Invoice Cancellation.
- Purchase Invoice Cancellation.
- Payment Cancellation.
- Stock Trade Cancellation.

The sample includes validation to prevent duplicate cancellation and invalid cancellation sequences.

## Credit Notes

Sales and purchase credit notes are implemented separately from cancellations.

Credit notes support partial credit quantities.

They track independent credited quantities.

They do not affect stock in the current implementation.

Credit notes and cancellations have separate business meaning.

## Stock

TinyERP includes stock-oriented modules.

Implemented areas include:

- Stock trades.
- Stock counts.
- Stock movements.
- Stock balances.
- Stock reservations.
- Stock document posting.
- Cancellation behavior for stock documents.

Sales and purchase delivery/return documents can affect stock through their document handlers.

Credit notes and cancellations currently do not affect stock.

## Finance

TinyERP includes finance movements and balances.

Implemented areas include:

- Partner finance movements.
- Partner balances.
- Cash and bank finance movements.
- Customer receipts.
- Supplier payments.
- Payment cancellations.
- Payment settlement links.
- Applied and unapplied payment amounts.

Sales and purchase documents can produce finance movements when posted.

Payments can settle open partner balances.

## Accounting

TinyERP includes a basic accounting cycle.

Implemented areas include:

- Chart of accounts.
- Journal entries.
- Manual journal posting.
- Auto accounting posting from business documents.
- Debit and credit validation.
- Journal entry unit tests.

The current accounting cycle uses fixed posting accounts.

Configurable posting profiles are future work.

## Pricing And Taxes

TinyERP includes price and tax resolver services.

Important types:

- `IPriceResolver`
- `PriceResolver`
- `PriceResolveArgs`
- `PriceResult`
- `ITaxResolver`
- `TaxResolver`
- `TaxResolveArgs`
- `TaxResult`
- `TaxComponent`

Trade document modules use these services to calculate commercial line values and tax values.

The implementation is educational and intentionally limited.

## Users And Security

TinyERP includes standard Tripous application user support.

Relevant areas:

- `SYS_APP_USER`
- `AppUserDataModule`
- Password dialogs in `tERP.Desktop`
- User-related application settings
- User-level security metadata on modules

The sample demonstrates application user infrastructure.

It is not a complete security model for production.

## Configuration

TinyERP uses `SYS_CONFIG` and Tripous configuration property descriptors.

Configuration values may be stored at scopes such as:

- System.
- Company.
- User.

`Registry.RegisterSycConfigProperties()` registers tERP configuration properties after descriptors are registered.

Application settings are then available through the Tripous configuration system and the desktop configuration dialogs.

## Sample Data

TinyERP includes sample data code under:

```text
tERP.Data/SampleData
```

Sample data is split by version.

Examples:

- `SampleData.Version1.cs`
- `SampleData.Version2.cs`

Sample data creates enough setup records for the educational workflows to run.

Examples include document types, number series, products, persons, tax setup, finance setup and other master data.

## UI Structure

The desktop UI follows the Tripous pattern used by the smaller samples.

The application includes:

- Main window.
- Dashboard form.
- Command groups.
- Sidebar commands.
- Content area forms.
- Generated data forms.
- Custom document forms.
- Custom item pages.
- Read-only views.

The generated form descriptors connect modules to forms.

Custom form classes add application-specific behavior where needed.

## Important UI Files

In `tERP`:

- `MainWindow.axaml`
- `DashboardForm.axaml`
- `ReadOnlyViewForm.axaml`
- `AppHost/AppHost.Commands.cs`
- `AppHost/AppHost.Startup.cs`
- `AppHost/AppHost.Ui.cs`
- `AppHost/AppHost.Views.cs`

In `tERP.Desktop`:

- `DataForms/DocumentDataForm.cs`
- `DataForms/JournalEntryForm.cs`
- `DataForms/StockCountForm.cs`
- `DataForms/StockTradeForm.cs`
- `DataForms/CustomerReceiptForm.cs`
- `DataForms/SupplierPaymentForm.cs`
- `TradeItemPage.cs`

## Tests

TinyERP contains automated tests in `tERP.Tests`.

Test areas include:

- Database smoke tests.
- Sales document tests.
- Purchase document tests.
- Stock count tests.
- Stock trade tests.
- Journal entry tests.

The tests use shared fixture infrastructure.

They are important because document posting changes multiple tables and must remain transactional.

## Build And Run

Do not assume a full solution build is needed when working on TinyERP.

Useful project targets:

```text
SampleApps/TinyERP/tERP/tERP.csproj
SampleApps/TinyERP/tERP.Data/tERP.Data.csproj
SampleApps/TinyERP/tERP.Desktop/tERP.Desktop.csproj
SampleApps/TinyERP/tERP.Tests/tERP.Tests.csproj
```

The application project is:

```text
SampleApps/TinyERP/tERP/tERP.csproj
```

The test project is:

```text
SampleApps/TinyERP/tERP.Tests/tERP.Tests.csproj
```

## Tester Guides

The executable project contains tester guide files:

- `tERP/tester-guide.txt`
- `tERP/tester-guide-gr.txt`

These files are intended for manual workflow testing.

They complement automated tests.

## Current Status

The current tERP cycle includes:

- Sales and purchase documents.
- Credit notes.
- Cancellations.
- Stock trades.
- Stock counts.
- Manual journal entries.
- Auto accounting posting.
- Partner finance movements and balances.
- Customer receipts.
- Supplier payments.
- Payment cancellations.
- Payment settlement links.
- Unit tests and UI tests for many workflows.

Known remaining work includes:

- Tester guide and release documentation.
- Supporting-module smoke tests.
- More documentation.
- Future extensions around currency rates, stock availability, configurable accounting posting profiles and production hardening.

## Current Limitations

Current limitations include:

- `ExchangeRate` is entered manually.
- Price lists and sales documents should currently use the same currency.
- Currency conversion is not performed automatically.
- Payment settlement is amount-based for the first cycle.
- Exchange-rate differences are not yet allocated.
- The first accounting cycle uses fixed posting accounts.
- Stock availability and reservation services need future expansion.
- Accounting posting profiles are not yet configurable.

## Future Extensions

Possible future extensions:

- Currency-rate table with dated exchange rates.
- Service for importing and updating exchange rates.
- Exchange-rate resolution by currency and document date.
- Stock availability service.
- Reservation service.
- Configurable accounting posting profiles.
- Reporting layer.
- Output templates.
- Stronger user and security model.
- More UI smoke tests.
- More tester documentation.

## How To Read The Code

Suggested reading path:

- Start with `tERP.Data/Schema01.sql`.
- Continue with `tERP.Data/Schema02.sql`.
- Inspect generated registry files under `tERP.Data/Registry`.
- Read `tERP.Data/Registry/Registry.cs`.
- Read `tERP.Data/DataModules/documents.md`.
- Inspect the document data module hierarchy.
- Inspect the document handler hierarchy.
- Open `tERP/AppHost`.
- Open the custom forms in `tERP.Desktop/DataForms`.
- Review tests in `tERP.Tests`.

This order shows the main Tripous idea:

- Schema metadata declares the application structure.
- The Registration Builder generates descriptors.
- The registry wires descriptors together.
- Data modules and handlers implement business behavior.
- Desktop forms expose the workflows.
- Tests verify the important flows.

## Relationship To Other Samples

TinyERP should be read after the smaller samples.

Recommended order:

- `01-hello-tripous` for the smallest desktop shell.
- `02-notes` for the first database module.
- `03-todo` for lookups, filters and configuration.
- `04-mini-crm` for manual master/detail, locators and code providers.
- `05-password-manager` for services and encrypted fields.
- `TinyERP` for automatic declaration and larger application architecture.

## Summary

TinyERP is the Tripous scale sample.

It demonstrates how the Tripous declaration model can grow from small manual samples into a larger generated application.

Its most important lesson is that automatic declaration is not a different architecture.

The Registration Builder generates the same descriptors a developer can write by hand, while the developer keeps control of business modules, document handlers, services, tests and UI extensions.
