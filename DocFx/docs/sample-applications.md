# Sample Applications

Tripous includes sample applications that show the framework in progressively larger steps.

The first samples are small and manually declared.

`TinyERP` is the larger sample and uses the Registration Builder for automatic declaration.

It is delivered as both a desktop ERP sample and a desktop-like web ERP sample over the same data and business layer.

## Disclaimer

- These applications are educational samples.
- They are not production applications.
- They are not audited or certified for real business, security, accounting or operational use.
- Do not use them to store real data or real secrets.
- They favor readability and framework demonstration over hardening and completeness.

## 01-hello-tripous

Folder:

- `SampleApps/01-hello-tripous`

Purpose:

- Shows the smallest useful Tripous desktop application.
- Uses a main window, menu, toolbar, dialog, message box and log box.
- Does not use a database.
- Does not use the Registration Builder.

Look at this sample first when you want to understand the basic Avalonia and `Tripous.Desktop` startup shape.

## 02-notes

Folder:

- `SampleApps/02-notes`

Purpose:

- Adds SQLite.
- Creates one table.
- Registers one module.
- Registers one form.
- Uses one `DataModule`.
- Defines schema and descriptors by hand.
- Does not use the Registration Builder.

This is the first database sample.

It shows the manual declaration path with `SchemaVersionDef`, `ModuleDef`, `TableDef`, `SelectDef` and `FormDef`.

## 03-todo

Folder:

- `SampleApps/03-todo`

Purpose:

- Uses a master table.
- Uses a status lookup table.
- Registers multiple modules and forms.
- Registers table-backed lookups.
- Registers list filters.
- Uses the default Tripous list and item editing UI.
- Does not use the Registration Builder.

This sample is useful for understanding lookups, filters, configuration values and a more realistic application startup flow.

## 04-mini-crm

Folder:

- `SampleApps/04-mini-crm`

Purpose:

- Uses multiple application tables.
- Uses master/detail forms.
- Uses lookup fields.
- Uses locator fields.
- Uses system modules.
- Uses a code provider for customer codes.
- Defines the application manually.
- Does not use the Registration Builder.

This is the main manual declaration sample.

It shows customers, contacts, activities, detail grids, locators, lookups, code providers and application settings.

## 05-password-manager

Folder:

- `SampleApps/05-password-manager`

Purpose:

- Uses SQLite.
- Uses a main credential module.
- Uses a category lookup table.
- Uses `SYS_CONFIG` for settings and vault metadata.
- Uses service classes for encryption and import/export.
- Uses a `DataModule` for encrypting and decrypting secret fields.
- Avoids app users, locators and code providers.
- Avoids plaintext secrets in the database.

This sample shows how application-specific services can work together with Tripous modules.

It is an educational sample, not an audited production password manager.

## TinyERP

Folder:

- `SampleApps/TinyERP`

Purpose:

- Shows a larger multi-project Tripous application.
- Uses `tERP.Common`, `tERP.Data`, `tERP.Desktop`, `tERP.Tests`, the executable desktop `tERP` project and the ASP.NET Core MVC `tERPWeb` project.
- Demonstrates a desktop ERP and a desktop-like web ERP using shared data modules, schema, sample data and business services.
- Uses the Registration Builder for automatic declaration.
- Uses schema metadata comments to generate registry code.
- Demonstrates many modules, forms, lookups, locators, select definitions and code providers.
- Includes sales, purchase, warehouse, finance, payments and accounting-oriented workflows.
- Includes document posting, cancellation, transformation and settlement workflows.
- Includes multilingual application text through `SYS_LANG`, `SYS_STR_RES` and the shared `SysStrRes` service.
- Includes administrator-maintained resource translations with English fallback.
- Includes desktop and web Resource Translations forms.
- Includes desktop and web Database Explorer and Interactive SQL tooling.
- Includes dashboards, read-only views, generated data forms and custom workflow forms.
- Includes unit and UI test projects.

This is the main automatic declaration sample.

It is the best sample for understanding how schema comments, the Registration Builder and generated declarations scale to a larger application.

It is also the main sample for studying how Tripous can share one SQL-first business layer between a native desktop client and a desktop-like browser client.

When testing `tERPWeb`, run a full rebuild of the `tERPWeb` project before starting it. The rebuild generates the `tp*.js` bundles used by the MVC layout and demos.

## Suggested Reading Order

- Start with `01-hello-tripous` for the smallest desktop shell.
- Continue with `02-notes` for the first database-backed module.
- Use `03-todo` to understand lookups, filters and configuration.
- Use `04-mini-crm` as the main manual declaration sample.
- Use `05-password-manager` to see application services and encrypted fields.
- Use `TinyERP` to study automatic declaration and a larger application structure.
