# Tripous.Avalon

Tripous.Avalon is a .NET framework for building data-centric business applications on desktop, service, and database platforms.

It combines SQL-first data access, database metadata, application descriptors, reusable data modules, generated or manual application registration, and an Avalonia-based desktop layer.

Documentation:

- https://tbebekis.github.io/Tripous.Avalon/
- https://tbebekis.github.io/Tripous.Avalon/web-demos/

> First public release of the sixth-generation Tripous framework family, built on .NET and Avalonia UI.

## What Is Tripous?

Tripous is an application framework.

It is neither a UI toolkit nor an ORM.

The framework provides infrastructure commonly needed by business applications:

- Database schema registration.
- SQL-oriented data access.
- Metadata descriptors and registries.
- Data modules.
- Modules, tables, fields and forms.
- Lookups and locators.
- Select definitions and filters.
- Code providers and number series.
- Master/detail data management.
- Application configuration.
- Logging infrastructure.
- Avalonia desktop application infrastructure.

The goal is to let developers describe the application structure explicitly while keeping full control over business logic and application behavior.

## Main Libraries

The repository contains the following framework libraries.

- `Tripous`: Core utilities, configuration, type services, collections, descriptors and shared infrastructure.
- `Tripous.Data`: Database access, schema execution, SQL providers, data modules, table sets, lookups, locators and data descriptors.
- `Tripous.Logging`: Logging infrastructure and diagnostics.
- `Tripous.Desktop`: Avalonia-based desktop layer with forms, menus, toolbars, commands, data forms and application UI infrastructure.

## Application Declaration

Tripous applications are built from descriptors.

Descriptors define:

- Modules.
- Tables and fields.
- Forms.
- Lookups.
- Locators.
- Select definitions.
- Code providers.
- Configuration properties.

There are two declaration paths.

- Manual declaration: the developer writes the descriptors directly in C#.
- Automatic declaration: the Registration Builder reads schema files and metadata comments, then generates the same descriptors automatically.

Both paths use the same runtime model.

The Registration Builder is not a separate architecture. It generates the same declarations a developer could otherwise write by hand.

## Sample Applications

The `SampleApps` folder contains progressively larger samples.

- `01-hello-tripous`: Smallest desktop shell. No database.
- `02-notes`: First SQLite-backed module and form.
- `03-todo`: Lookups, filters, configuration and a more realistic startup flow.
- `04-mini-crm`: Main manual declaration sample with master/detail forms, locators, lookups and code providers.
- `05-password-manager`: Services, encrypted fields, import/export and vault locking. Educational sample only and not intended for production security use.
- `TinyERP`: Larger multi-project ERP-style sample using the Registration Builder and generated declarations.

Sample applications are educational material and reference implementations, not production applications.

## TinyERP

`SampleApps/TinyERP` is the largest sample application.

It demonstrates:

- Multi-project application structure.
- Automatic registration with the Registration Builder.
- Schema metadata comments.
- Generated modules, forms, lookups, locators, select definitions and code providers.
- Sales and purchase documents.
- Stock documents.
- Journal entries.
- Finance movements and balances.
- Customer receipts and supplier payments.
- Unit tests and workflow-oriented tests.

See:

- `SampleApps/TinyERP/ReadMe.md`

## Documentation Project

The documentation site is located under:

- `DocFx`

Important files:

- `DocFx/index.md`
- `DocFx/toc.yml`
- `DocFx/docs/toc.yml`
- `DocFx/docfx.json`

Conceptual documentation lives under:

- `DocFx/docs`

Generated API documentation is produced by DocFX from XML comments.

Do not manually edit generated output under:

- `DocFx/_site`
- `DocFx/api`

## Tripous.Web Demos

The repository includes a standalone static demo site for the Tripous.Web JavaScript runtime and controls.

- Source: `WebDemos`
- Published URL: https://tbebekis.github.io/Tripous.Avalon/web-demos/

The WebDemos site is plain HTML, CSS and JavaScript. It is independent from the ASP.NET Core demo application and can be published as part of the DocFX GitHub Pages site.

tERPWeb is the ASP.NET Core MVC TinyERP Web sample under `SampleApps/TinyERP/tERPWeb`.

When testing tERPWeb, run a full rebuild of the `tERPWeb` project first. The rebuild creates the generated Tripous Web bundles such as `tp.js`, `tp-Data.js`, `tp-UI.js`, `tp-Grid.js` and `tp-WebDesk.js` from the source fragments under `wwwroot/js-src`.

## Tools

The `Tools` folder contains developer tools.

The most important tool is the Registration Builder console.

Example:

```text
dotnet run --project Tools/RegBuilderConsole -- --project tERP.Version2
```

The tool reads configured schema files and generates Tripous registry code.

## Database Support

Tripous uses SQL providers and database-neutral schema tokens.

Supported relational database engines include:

- SQLite.
- Microsoft SQL Server.
- MySQL.
- PostgreSQL.
- Firebird SQL.
- Oracle Database.

Schema SQL can use provider-neutral tokens such as:

- `@NVARCHAR(size)`
- `@DATE`
- `@DATE_TIME`
- `@BOOL`
- `@NBLOB_TEXT`
- `@NOT_NULL`
- `@NULL`

Providers translate those tokens into the target RDBMS dialect.

## Development Notes

The solution file is:

- `Tripous.Avalon.sln`

Useful folders:

- `Tripous`
- `Tripous.Data`
- `Tripous.Logging`
- `Tripous.Desktop`
- `SampleApps`
- `Tools`
- `UnitTests`
- `DocFx`

For Tripous.Desktop-focused work, a scoped build is usually sufficient.

```text
dotnet build Tripous.Desktop/Tripous.Desktop.csproj
```

Do not edit generated Registration Builder output directly.

For TinyERP schema changes, edit the schema files and regenerate the registry source code.

## Status

Tripous.Avalon is actively developed.

Current priorities include:

- Framework stabilization.
- Conceptual documentation.
- Sample applications.
- Registration Builder workflow.
- XML documentation cleanup.
- Automated testing.
- Preparation for the future Tripous.Web platform.

## License

Tripous.Avalon is licensed under the Tripous.Avalon Community License v1.0.

See:

- `LICENSE.txt`
